using LibH2A.Common;
using LibH2A.Saber3D.Materials;
using static Haus.Assertions;

namespace LibH2A.Saber3D
{

  public class S3D_GeometryData
  {

    #region Data Members

    // TODO: This is a bit of a hack to deal with a potential offset when the 1SERpak header is present.
    private long _tplOffset;
    // TODO: This is also a hack.
    private ulong _unkSeekFlag;

    private ushort _rootNodeIndex;
    private uint _nodeCount;
    private uint _bufferCount;
    private uint _meshCount;
    private uint _subMeshCount;
    private S3D_MeshData[][] _meshData;
    private Unk_MeshInfo[] _meshInfo;
    private List<short[]> _boneMap;

    private List<S3D_GeometryBuffer> _buffers;

    private List<S3D_SubMesh> _submeshes;

    #endregion

    #region Properties

    public IReadOnlyList<S3D_GeometryBuffer> Buffers
    {
      get => _buffers;
    }

    public IReadOnlyList<S3D_SubMesh> SubMeshes
    {
      get => _submeshes;
    }

    public S3D_MeshData[][] MeshData { get => _meshData; }

    public List<S3D_Face> Faces { get; set; }
    public List<S3D_Vertex> Vertices { get; set; }


    #endregion

    #region Constructor

    private S3D_GeometryData()
    {
      _buffers = new List<S3D_GeometryBuffer>();
      _submeshes = new List<S3D_SubMesh>();
    }

    public static S3D_GeometryData Read( EndianBinaryReader reader )
    {
      /* NOTE:
       *  Each chunk reads a chunkTag and a chunkEnd.
       *  The chunkEnd points to the next chunk, but it's offset
       *  by the 1SERpak header. If that header is present, you
       *  need to subtract the current stream position by the number of
       *  bytes between the start of the file and the 1SERtpl header.
       */

      var data = new S3D_GeometryData();

      ReadChunk_01( data, reader );
      ReadChunk_02( data, reader );
      ReadChunk_03( data, reader );
      ReadChunk_04( data, reader );
      ReadChunk_05( data, reader );
      ReadChunk_06( data, reader );
      ReadChunk_07( data, reader );
      ReadChunk_08( data, reader );
      ReadChunk_09( data, reader );
      ReadChunk_10( data, reader );
      ReadChunk_11( data, reader );
      ReadChunk_12( data, reader );
      ReadChunk_13( data, reader );
      ReadChunk_14( data, reader );
      ReadChunk_15( data, reader );
      ReadChunk_16( data, reader );
      ReadChunk_17( data, reader );

      ParseSubMeshData( data, reader );

      return data;
    }

    #endregion

    #region Private Methods

    private static void ReadChunk_01( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Model Info Chunk?
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      data._rootNodeIndex = reader.ReadUInt16();
      data._nodeCount = reader.ReadUInt32();
      data._bufferCount = reader.ReadUInt32();
      data._meshCount = reader.ReadUInt32();
      data._subMeshCount = reader.ReadUInt32();
      _ = reader.ReadUInt64(); // Unk

      data._tplOffset = reader.BaseStream.Position - chunkEnd; // TODO

      // TODO: This is a hack for now
      var currentPos = reader.BaseStream.Position;
      reader.BaseStream.Position = data._tplOffset + 0x10;
      data._unkSeekFlag = reader.ReadUInt64();
      reader.BaseStream.Position = currentPos;
    }

    private static void ReadChunk_02( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Unknown Chunk
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      //Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 02" );
    }

    private static void ReadChunk_03( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Buffer Type Info Chunk
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var buffers = data._buffers;
      var bufferTypes = Enum.GetValues<S3D_BufferType>().Cast<byte>().ToHashSet();

      for ( uint i = 0; i < data._bufferCount; i++ )
      {
        var typeData = new S3D_BufferInfo
        {
          Unk_01 = reader.ReadUInt16(),
          Unk_02 = reader.ReadByte(),
          Type = ( S3D_BufferType ) reader.ReadByte(),
          Unk_04 = reader.ReadByte(),
          Unk_05 = reader.ReadByte(),
          Unk_06 = reader.ReadUInt32()
        };

        //Assert( bufferTypes.Contains( ( byte ) typeData.Type ),
        //  $"Unknown buffer type: {( byte ) typeData.Type:X}" );

        buffers.Add( new S3D_GeometryBuffer
        {
          Index = i,
          BufferInfo = typeData
        } );
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 03" );
    }

    private static void ReadChunk_04( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Buffer Elem Sizes?
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var buffers = data.Buffers;
      for ( var i = 0; i < buffers.Count; i++ )
        buffers[ i ].ElementSize = reader.ReadUInt16();

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 04" );
    }

    private static void ReadChunk_05( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Buffer Lengths
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var buffers = data.Buffers;
      for ( var i = 0; i < buffers.Count; i++ )
        buffers[ i ].BufferLength = reader.ReadUInt32();

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 05" );
    }

    private static void ReadChunk_06( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Buffer Offsets
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var buffers = data.Buffers;
      for ( var i = 0; i < buffers.Count; i++ )
      {
        var buffer = buffers[ i ];
        buffer.DataOffset = reader.BaseStream.Position;

        // Seek to next buffer
        reader.BaseStream.Position += buffer.BufferLength;
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 06" );
    }

    private static void ReadChunk_07( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Unknown Chunk
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      //Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 07" );
    }

    private static void ReadChunk_08( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Mesh Data?
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var meshData = data._meshData = new S3D_MeshData[ data._meshCount ][];
      for ( var i = 0; i < meshData.Length; i++ )
      {
        var count = reader.ReadByte();
        meshData[ i ] = new S3D_MeshData[ count ];
        for ( var j = 0; j < count; j++ )
        {
          meshData[ i ][ j ] = new S3D_MeshData
          {
            BufferId = reader.ReadUInt32(),
            SubBufferOffset = reader.ReadUInt32(),
          };
        }
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 08" );
    }

    private static void ReadChunk_09( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Unknown Chunk
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var meshInfo = data._meshInfo = new Unk_MeshInfo[ data._meshCount ];
      for ( var i = 0; i < meshInfo.Length; i++ )
      {
        meshInfo[ i ] = new Unk_MeshInfo
        {
          Unk_01 = reader.ReadUInt16(),
          Unk_02 = reader.ReadByte(),
          Unk_03 = reader.ReadByte(),
          Unk_04 = reader.ReadByte(),
          Unk_05 = reader.ReadByte(),
          Unk_06 = reader.ReadUInt32(),
        };
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 09" );
    }

    private static void ReadChunk_10( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Unknown Chunk
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      //Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 10" );
    }

    private static void ReadChunk_11( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // SubMesh Data
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var submeshes = data._submeshes;
      for ( var i = 0; i < data._subMeshCount; i++ )
      {
        submeshes.Add( new S3D_SubMesh
        {
          VertOffset = reader.ReadUInt16(),
          VertCount = reader.ReadUInt16(),
          FaceOffset = reader.ReadUInt16(),
          FaceCount = reader.ReadUInt16(),
          NodeId = reader.ReadUInt16(),
          SkinCompoundId = reader.ReadUInt16()
        } );

        if ( data._unkSeekFlag == 0x9 )
        {
          var unk_seekFlag = reader.ReadInt32();
          switch ( unk_seekFlag )
          {
            case 0:
              break;
            case 1:
              reader.BaseStream.Position += 0x08;
              break;
            case 3:
              reader.BaseStream.Position += 0x10;
              break;
            case 7:
              reader.BaseStream.Position += 0x18;
              break;
            default:
              throw new Exception( $"Unknown seek flag thing in GeometryData chunk 11: {unk_seekFlag}" );
          }
        }
      }

      // TODO: When _unkSeekFlag is not 9, this assert fails
      //Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 11" );
      reader.BaseStream.Position = chunkEnd;
    }

    private static void ReadChunk_12( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // SubMesh-Mesh Id
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var submeshes = data._submeshes;
      for ( var i = 0; i < submeshes.Count; i++ )
        submeshes[ i ].Unk_MeshId = reader.ReadUInt32();

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 12" );
    }

    private static void ReadChunk_13( S3D_GeometryData data, EndianBinaryReader reader )
    {
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      // SubMesh Materials
      if ( chunkTag == 0x08 )
      {
        var submeshes = data._submeshes;
        for ( var i = 0; i < submeshes.Count; i++ )
        {
          _ = reader.ReadInt16();
          var material = S3D_Material.Read( reader );
          submeshes[ i ].Material = material;
        }
      }
      else
      {
        // TODO: Unknown Material Format. Just one string w/ some additional data.
        reader.BaseStream.Position = chunkEnd;
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 13" );
    }

    private static void ReadChunk_14( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // Bone Map?
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var boneMap = data._boneMap = new List<short[]>();
      while ( reader.BaseStream.Position < chunkEnd )
      {
        var count = reader.ReadByte();
        var subArray = new short[ count ];
        for ( var i = 0; i < count; i++ )
          subArray[ i ] = reader.ReadInt16();

        boneMap.Add( subArray );
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 14" );
    }

    private static void ReadChunk_15( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // SubMesh Unknown Info
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var submeshes = data._submeshes;
      for ( var i = 0; i < submeshes.Count; i++ )
      {
        submeshes[ i ].UnkInfo = new Unk_SubMeshInfo
        {
          Unk_01 = reader.ReadByte(),
          Unk_02 = reader.ReadByte(),
          Unk_03 = reader.ReadByte(),
          Unk_04 = reader.ReadByte(),
        };
      }

      // TODO: This isn't always reading to the end. Fix.
      //Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 15" );
      reader.BaseStream.Position = chunkEnd;
    }

    private static void ReadChunk_16( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // SubMesh Transform Info
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var submeshes = data._submeshes;
      for ( var i = 0; i < submeshes.Count; i++ )
      {
        submeshes[ i ].TransformInfo = new S3D_SubMeshTransformInfo
        {
          Position = new short[]
          {
            reader.ReadInt16(),
            reader.ReadInt16(),
            reader.ReadInt16()
          },
          Scale = new short[]
          {
            reader.ReadInt16(),
            reader.ReadInt16(),
            reader.ReadInt16()
          }
        };
      }

      Assert( reader.BaseStream.Position == chunkEnd, "Did not read all of Chunk 16" );
    }

    private static void ReadChunk_17( S3D_GeometryData data, EndianBinaryReader reader )
    {
      // SubMesh Transform Info
      var chunkTag = reader.ReadUInt16();
      var chunkEnd = reader.ReadUInt32() + data._tplOffset;

      var fileSize = reader.BaseStream.Length;
      var currentPos = reader.BaseStream.Position;

      Assert( fileSize == currentPos, $"Finished reading, but the file still has {fileSize - currentPos} more bytes." );
    }

    private static void ParseSubMeshData( S3D_GeometryData data, EndianBinaryReader reader )
    {
      var vertbuf = data.Buffers.First( x => x.BufferType == S3D_BufferType.StaticVert );
      reader.Seek( vertbuf.DataOffset, SeekOrigin.Begin );
      var verts = data.Vertices = new List<S3D_Vertex>();
      for ( var i = 0; i < vertbuf.BufferLength; i += vertbuf.ElementSize )
      {
        verts.Add( new S3D_Vertex { X = reader.ReadInt16(), Y = reader.ReadInt16(), Z = reader.ReadInt16(), W = reader.ReadInt16() } );
        reader.BaseStream.Position += vertbuf.ElementSize - 8;
      }

      var facebuf = data.Buffers.First( x => x.BufferType == S3D_BufferType.Face );
      reader.Seek( facebuf.DataOffset, SeekOrigin.Begin );
      var faces = data.Faces = new List<S3D_Face>();
      for ( var i = 0; i < facebuf.BufferLength; i += 6 )
      {
        faces.Add( new S3D_Face { A = reader.ReadInt16(), B = reader.ReadInt16(), C = reader.ReadInt16() } );
      }

      foreach ( var submesh in data._submeshes )
      {
        ParseSubMeshVertices( submesh, reader );
        ParseSubMeshFaces( submesh, reader );
      }
    }

    private static void ParseSubMeshVertices( S3D_SubMesh submesh, EndianBinaryReader reader )
    {
    }

    private static void ParseSubMeshFaces( S3D_SubMesh submesh, EndianBinaryReader reader )
    {

    }

    #endregion

    #region Embedded Types

    public enum S3D_BufferType : byte
    {
      Face = 0x00,
      Unk_Face = 0x02,
      StaticVert = 0x0C,
      SkinnedVert = 0x0F,
      VertNormalAndUV = 0x10,
      Unk_Vert30 = 0x30,
      Unk_Vert70 = 0x70,

      Unk_42 = 0x42,
      Unk_5E = 0x5E,
      Unk_C6 = 0xC6,
    }

    public struct S3D_BufferInfo
    {
      public ushort Unk_01;
      public byte Unk_02;
      public S3D_BufferType Type;
      public byte Unk_04;
      public byte Unk_05;
      public uint Unk_06;
    }

    public class S3D_GeometryBuffer
    {
      public uint Index { get; set; }
      public S3D_BufferInfo BufferInfo { get; set; }
      public ushort ElementSize { get; set; }
      public uint BufferLength { get; set; }
      public long DataOffset { get; set; }

      public S3D_BufferType BufferType
      {
        get => BufferInfo.Type;
      }

    }

    public class S3D_MeshData
    {
      public uint BufferId { get; set; }
      public uint SubBufferOffset { get; set; }
      public Unk_MeshInfo Unk_03_MeshInfo { get; set; }
    }

    public struct Unk_MeshInfo
    {
      public ushort Unk_01;
      public byte Unk_02;
      public byte Unk_03;
      public byte Unk_04;
      public byte Unk_05;
      public uint Unk_06;
    }

    public class S3D_SubMesh
    {
      public uint Unk_MeshId { get; set; } //links subMesh to mes
      public ushort VertOffset { get; set; }
      public ushort VertCount { get; set; }
      public ushort FaceOffset { get; set; }
      public ushort FaceCount { get; set; }
      public ushort NodeId { get; set; }
      public ushort SkinCompoundId { get; set; }
      public S3D_Material Material { get; set; }
      public Unk_SubMeshInfo UnkInfo { get; set; }
      public S3D_SubMeshTransformInfo TransformInfo { get; set; }

      public IReadOnlyList<S3D_Face> Faces { get; set; }
      public IReadOnlyList<S3D_Vertex> Vertices { get; set; }
    }

    public struct Unk_SubMeshInfo
    {
      public byte Unk_01;
      public byte Unk_02;
      public byte Unk_03;
      public byte Unk_04;
    }

    public struct S3D_SubMeshTransformInfo
    {
      public short[] Position;
      public short[] Scale;
    }

    #endregion

  }

}

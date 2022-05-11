using System.Collections;
using System.Diagnostics;
using System.Numerics;
using LibH2A.Common;
using LibH2A.Saber3D.Materials;
using static Haus.Assertions;

namespace LibH2A.Saber3D.Geometry
{

  public class S3D_SubMeshCollection : IEnumerable<S3D_SubMesh>
  {

    #region Data Members

    private S3D_SubMesh[] _submeshes;

    #endregion

    #region Properties

    public S3D_GeometryData Parent { get; }

    public int Count
    {
      get => _submeshes.Length;
    }

    public S3D_SubMesh this[ int index ]
    {
      get => _submeshes[ index ];
    }

    #endregion

    #region Constructor

    private S3D_SubMeshCollection( S3D_GeometryData parent )
    {
      Parent = parent;

      _submeshes = new S3D_SubMesh[ parent.SubMeshCount ];
      for ( var i = 0; i < _submeshes.Length; i++ )
        _submeshes[ i ] = new S3D_SubMesh( parent );
    }

    public static S3D_SubMeshCollection Read( S3D_GeometryData parent, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      var submeshes = new S3D_SubMeshCollection( parent );

      while ( reader.Position < blockEndOffset )
      {
        var blockType = ReadBlockType( reader );

        switch ( blockType )
        {
          case BlockType.BufferInfo:
            ReadBlock_BufferInfo( submeshes, reader );
            break;
          case BlockType.MeshIds:
            ReadBlock_MeshIds( submeshes, reader );
            break;
          case BlockType.Unk_MaterialScript:
            ReadBlock_Unk_MaterialScript( submeshes, reader );
            break;
          case BlockType.BoneMap:
            ReadBlock_BoneMap( submeshes, reader );
            break;
          case BlockType.SubMeshInfo:
            ReadBlock_SubMeshInfo( submeshes, reader );
            break;
          case BlockType.TransformInfo:
            ReadBlock_TransformInfo( submeshes, reader );
            break;
          case BlockType.Materials_String:
            ReadBlock_MaterialString( submeshes, reader );
            break;
          case BlockType.Materials_Static:
            ReadBlock_MaterialsStatic( submeshes, reader );
            break;
          case BlockType.Materials_Dynamic:
            ReadBlock_MaterialsDynamic( submeshes, reader );
            break;
          default:
            Fail( $"Unhandled BlockType in S3D_SubMeshCollection: {blockType}" );
            break;
        }
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Block." );

      return submeshes;
    }

    #endregion

    #region Private Methods

    [DebuggerHidden]
    private static BlockType ReadBlockType( EndianBinaryReader reader )
    {
      var blockType = reader.ReadUInt16();
      var blockTypeAsEnum = ( BlockType ) blockType;
      Assert( Enum.IsDefined<BlockType>( blockTypeAsEnum ),
        $"Encountered an unknown BlockType: {blockType:X}" );

      return blockTypeAsEnum;
    }

    private static void ReadBlock_BufferInfo( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < submeshes.Count; i++ )
      {
        var bufferInfo = new S3D_SubMeshBufferInfo
        {
          VertexOffset = reader.ReadUInt16(),
          VertexCount = reader.ReadUInt16(),
          FaceOffset = reader.ReadUInt16(),
          FaceCount = reader.ReadUInt16(),
          NodeId = reader.ReadUInt16(),
          SkinCompoundId = reader.ReadUInt16(),
        };

        submeshes[ i ].BufferInfo = bufferInfo;

        // TODO:
        var flag = reader.ReadInt32();
        switch ( flag )
        {
          case 0:
            break;
          case 1:
            reader.Position += 0x8;
            break;
          case 3:
            reader.Position += 0x10;
            break;
          case 7:
            reader.Position += 0x18;
            break;
          default:
            //throw new Exception( "Unknown seek." );
            reader.Position = blockEndOffset;
            return;
        }

      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh BufferInfo Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh BufferInfo Block." );
    }

    private static void ReadBlock_MeshIds( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < submeshes.Count; i++ )
        submeshes[ i ].MeshId = reader.ReadUInt32();

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh MeshId Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh MeshId Block." );
    }

    private static void ReadBlock_Unk_MaterialScript( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      // TODO
      // Unknown data. Skipping.
      reader.Position = blockEndOffset;

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Materials Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Materials Block." );
    }

    private static void ReadBlock_BoneMap( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      // TODO: Implement
      reader.Position = blockEndOffset;

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh BoneMap Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh BoneMap Block." );
    }

    private static void ReadBlock_SubMeshInfo( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      //for ( var i = 0; i < submeshes.Count; i++ )
      //{
      //  var info = new S3D_SubMeshInfo
      //  {
      //    Unk_01 = reader.ReadByte(),
      //    Unk_02 = reader.ReadByte(),
      //    Unk_03 = reader.ReadByte(),
      //    Unk_04 = reader.ReadByte(),
      //  };

      //  submeshes[ i ].SubMeshInfo = info;
      //}

      // TODO: This is inconsistent. Investigate
      // Skipping for now since it's all unknowns
      reader.Position = blockEndOffset;

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Info Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Info Block." );
    }

    private static void ReadBlock_TransformInfo( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < submeshes.Count; i++ )
      {
        // TODO
        // This can sometimes overrun. I'm just exiting the loop early if that happens.
        if ( reader.Position == blockEndOffset )
          break;

        var position = new Vector3(
          x: reader.ReadInt16().ToSNorm(),
          y: reader.ReadInt16().ToSNorm(),
          z: reader.ReadInt16().ToSNorm() );
        submeshes[ i ].Position = position;

        var scale = new Vector3(
        x: reader.ReadInt16(),
        y: reader.ReadInt16(),
        z: reader.ReadInt16() );
        submeshes[ i ].Scale = scale;
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Transform Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Transform Block." );
    }

    private static void ReadBlock_MaterialString( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < submeshes.Count; i++ )
      {
        _ = reader.ReadUInt16();
        var stringLength = reader.ReadUInt16();
        _ = reader.ReadUInt16();

        var materialString = reader.ReadString( stringLength );
        submeshes[ i ].MaterialString = materialString;
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Materials Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Materials Block." );
    }

    private static void ReadBlock_MaterialsStatic( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < submeshes.Count; i++ )
      {
        _ = reader.ReadUInt16(); // Unk

        // TODO: This is pretty much identical to dynamic, except there is no length for the property names.
        // Skipping for now.
      }

      reader.Position = blockEndOffset;

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Materials Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Materials Block." );
    }

    private static void ReadBlock_MaterialsDynamic( S3D_SubMeshCollection submeshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < submeshes.Count; i++ )
      {
        _ = reader.ReadUInt16(); // Unk
        submeshes[ i ].Material = S3D_Material.Read( reader );
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the SubMesh Materials Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire SubMesh Materials Block." );
    }

    #endregion

    #region Embedded Types

    private enum BlockType : UInt16
    {
      BufferInfo = 0x0000,
      MeshIds = 0x0001,
      Unk_MaterialScript = 0x0002,
      BoneMap = 0x0003,
      SubMeshInfo = 0x0004,
      TransformInfo = 0x0005,
      Materials_String = 0x0006,
      Materials_Static = 0x0007,
      Materials_Dynamic = 0x0008,
    }

    #endregion

    #region IEnumerable Methods

    public IEnumerator<S3D_SubMesh> GetEnumerator()
      => _submeshes.GetEnumerator() as IEnumerator<S3D_SubMesh>;

    IEnumerator IEnumerable.GetEnumerator()
      => _submeshes.GetEnumerator();

    #endregion

  }

}

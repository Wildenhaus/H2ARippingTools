using System.Collections;
using System.Diagnostics;
using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D.Geometry
{

  public class S3D_MeshCollection : IEnumerable<S3D_Mesh>
  {

    #region Data Members

    private S3D_Mesh[] _meshes;

    #endregion

    #region Properties

    public S3D_GeometryData Parent { get; }

    public int Count
    {
      get => _meshes.Length;
    }

    public S3D_Mesh this[ int index ]
    {
      get => _meshes[ index ];
    }

    #endregion

    #region Constructor

    private S3D_MeshCollection( S3D_GeometryData parent )
    {
      Parent = parent;
      _meshes = new S3D_Mesh[ parent.MeshCount ];
      for ( var i = 0; i < _meshes.Length; i++ )
        _meshes[ i ] = new S3D_Mesh( parent, i );
    }

    public static S3D_MeshCollection Read( S3D_GeometryData parent, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      var meshes = new S3D_MeshCollection( parent );

      while ( reader.Position < blockEndOffset )
      {
        var blockType = ReadBlockType( reader );

        switch ( blockType )
        {
          case BlockType.MeshInfo:
            ReadBlock_MeshInfo( meshes, reader );
            break;
          case BlockType.BufferInfo:
            ReadBlock_BufferInfo( meshes, reader );
            break;
          default:
            Fail( $"Unhandled BlockType in S3D_MeshCollection: {blockType}" );
            break;
        }
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the Mesh Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire Mesh Block." );

      return meshes;
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

    private static void ReadBlock_MeshInfo( S3D_MeshCollection meshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < meshes.Count; i++ )
      {
        var meshInfo = new S3D_MeshInfo
        {
          Unk_01 = reader.ReadUInt16(),
          Unk_02 = reader.ReadByte(),
          Unk_03 = reader.ReadByte(),
          Unk_04 = reader.ReadByte(),
          Unk_05 = reader.ReadByte(),
          Unk_06 = reader.ReadUInt32(),
        };

        meshes[ i ].MeshInfo = meshInfo;
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the MeshInfo Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire MeshInfo Block." );
    }

    private static void ReadBlock_BufferInfo( S3D_MeshCollection meshes, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < meshes.Count; i++ )
      {
        var count = reader.ReadByte();
        var buffers = meshes[ i ].Buffers = new S3D_MeshBufferInfo[ count ];

        for ( var j = 0; j < count; j++ )
        {
          buffers[ j ] = new S3D_MeshBufferInfo
          {
            BufferId = reader.ReadUInt32(),
            SubBufferOffset = reader.ReadUInt32(),
          };
        }
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the Mesh BufferInfo Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire Mesh BufferInfo Block." );
    }

    #endregion

    #region Embedded Types

    private enum BlockType : UInt16
    {
      MeshInfo = 0x0000,
      BufferInfo = 0x0002
    }

    #endregion

    #region IEnumerable Methods

    public IEnumerator<S3D_Mesh> GetEnumerator()
    {
      foreach ( var mesh in _meshes )
        yield return mesh;
    }

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    #endregion

  }

}

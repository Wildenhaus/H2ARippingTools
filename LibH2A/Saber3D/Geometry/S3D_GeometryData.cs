using System.Diagnostics;
using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D.Geometry
{

  public class S3D_GeometryData
  {

    #region Properties

    public UInt16 RootNodeIndex { get; private set; }
    public UInt32 NodeCount { get; private set; }
    public UInt32 BufferCount { get; private set; }
    public UInt32 MeshCount { get; private set; }
    public UInt32 SubMeshCount { get; private set; }

    public S3D_GeometryBufferCollection Buffers { get; private set; }
    public S3D_MeshCollection Meshes { get; private set; }
    public S3D_SubMeshCollection SubMeshes { get; private set; }

    #endregion

    #region Constructor

    private S3D_GeometryData()
    {
    }

    public static S3D_GeometryData Read( EndianBinaryReader reader )
    {
      var data = new S3D_GeometryData();

      BlockType blockType = ReadBlockType( reader );
      while ( blockType != BlockType.Footer )
      {
        switch ( blockType )
        {
          case BlockType.Header:
            ReadHeaderBlock( reader, data );
            break;
          case BlockType.Buffers:
            data.Buffers = S3D_GeometryBufferCollection.Read( data, reader );
            break;
          case BlockType.Meshes:
            data.Meshes = S3D_MeshCollection.Read( data, reader );
            break;
          case BlockType.SubMeshes:
            data.SubMeshes = S3D_SubMeshCollection.Read( data, reader );
            break;
          default:
            Fail( $"Unhandled BlockType in S3D_GeometryData: {blockType}" );
            break;
        }

        blockType = ReadBlockType( reader );
      }

      return data;
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

    private static void ReadHeaderBlock( EndianBinaryReader reader, S3D_GeometryData data )
    {
      var blockEndOffset = reader.ReadUInt32();

      data.RootNodeIndex = reader.ReadUInt16();
      data.NodeCount = reader.ReadUInt32();
      data.BufferCount = reader.ReadUInt32();
      data.MeshCount = reader.ReadUInt32();
      data.SubMeshCount = reader.ReadUInt32();
      _ = reader.ReadUInt64(); // Unk

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the Geometry Header Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire Geometry Header Block." );
    }

    #endregion

    #region Embedded Types

    private enum BlockType : UInt16
    {
      Header = 0x0000,
      Buffers = 0x0002,
      Meshes = 0x0003,
      SubMeshes = 0x0004,
      Footer = 0xFFFF
    }

    #endregion

  }

}

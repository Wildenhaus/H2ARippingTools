using System.Diagnostics;
using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D.Geometry
{

  public class S3D_GeometryBufferCollection
  {

    #region Data Members

    private S3D_GeometryBuffer[] _buffers;

    #endregion

    #region Properties

    public S3D_GeometryData Parent { get; }

    public int Count
    {
      get => _buffers.Length;
    }

    public S3D_GeometryBuffer this[ int index ]
    {
      get => _buffers[ index ];
    }

    #endregion

    #region Constructor

    private S3D_GeometryBufferCollection( S3D_GeometryData parent )
    {
      Parent = parent;

      _buffers = new S3D_GeometryBuffer[ parent.BufferCount ];
      for ( var i = 0; i < _buffers.Length; i++ )
        _buffers[ i ] = new S3D_GeometryBuffer( parent );
    }

    public static S3D_GeometryBufferCollection Read( S3D_GeometryData parent, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      var buffers = new S3D_GeometryBufferCollection( parent );

      while ( reader.Position < blockEndOffset )
      {
        var blockType = ReadBlockType( reader );

        switch ( blockType )
        {
          case BlockType.BufferTypeInfo:
            ReadBlock_BufferTypeInfo( buffers, reader );
            break;
          case BlockType.BufferElementSizes:
            ReadBlock_BufferElementSizes( buffers, reader );
            break;
          case BlockType.BufferLengths:
            ReadBlock_BufferLengths( buffers, reader );
            break;
          case BlockType.BufferData:
            ReadBlock_BufferData( buffers, reader );
            break;
          default:
            Fail( $"Unhandled BlockType in S3D_GeometryBufferCollection: {blockType}" );
            break;
        }
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the Buffer Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire Buffer Block." );

      return buffers;
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

    private static void ReadBlock_BufferTypeInfo( S3D_GeometryBufferCollection buffers, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < buffers.Count; i++ )
      {
        var typeInfo = new S3D_GeometryBufferTypeInfo
        {
          Unk_01 = reader.ReadUInt16(),
          Unk_02 = reader.ReadByte(),
          BufferType = ( S3D_GeometryBufferType ) reader.ReadByte(),
          Unk_04 = reader.ReadByte(),
          Unk_05 = reader.ReadByte(),
          Unk_06 = reader.ReadUInt32()
        };

        Assert( Enum.IsDefined<S3D_GeometryBufferType>( typeInfo.BufferType ),
          $"Unknown S3D_GeometryBufferType: {( byte ) typeInfo.BufferType:X}" );

        buffers[ i ].TypeInfo = typeInfo;
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the BufferTypeInfo Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire BufferTypeInfo Block." );
    }

    private static void ReadBlock_BufferElementSizes( S3D_GeometryBufferCollection buffers, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < buffers.Count; i++ )
        buffers[ i ].ElementSize = reader.ReadUInt16();

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the BufferElementSize Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire BufferElementSize Block." );
    }

    private static void ReadBlock_BufferLengths( S3D_GeometryBufferCollection buffers, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < buffers.Count; i++ )
        buffers[ i ].BufferLength = reader.ReadUInt32();

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the BufferLength Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire BufferLength Block." );
    }

    private static void ReadBlock_BufferData( S3D_GeometryBufferCollection buffers, EndianBinaryReader reader )
    {
      var blockEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < buffers.Count; i++ )
      {
        buffers[ i ].StartOffset = reader.Position;
        reader.Position += buffers[ i ].BufferLength;
        buffers[ i ].EndOffset = reader.Position;
      }

      if ( reader.Position > blockEndOffset )
        Fail( "Over-read the BufferLength Block!" );
      else if ( reader.Position < blockEndOffset )
        Fail( "Did not read entire BufferLength Block." );
    }

    #endregion

    #region Embedded Types

    private enum BlockType : UInt16
    {
      BufferTypeInfo = 0x0000,
      BufferElementSizes = 0x0001,
      BufferLengths = 0x0002,
      BufferData = 0x0003
    }

    #endregion

  }

}

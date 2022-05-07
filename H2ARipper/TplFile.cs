namespace H2ARipper
{

  public class TplFile
  {

    private string _filePath;

    public static TplFile Open( string filePath )
    {
      var file = new TplFile();
      file.Init( filePath );

      return file;
    }

    private void Init( string filePath )
    {
      _filePath = filePath;
      using var fs = File.OpenRead( _filePath );
      using var reader = new BinaryReader( fs );

      //ReadHeader( reader );
      //ReadTPL1Block( reader );
      //ReadTextureNames( reader );
      ReadOgmBlock( reader );
      //ReadChunks( reader );
    }

    #region Read Methods

    private void ReadHeader( BinaryReader reader )
    {
      const long MAGIC_ISERPAK = 0x006B617052455331;
      Assert( reader.ReadInt64(), MAGIC_ISERPAK, "Invalid magic." );

      /*0x08*/
      _ = reader.ReadInt64(); // Unk: some sort of size
      /*0x10*/
      _ = reader.ReadInt64(); // Unk: Some sort of size
      /*0x18*/
      _ = reader.ReadInt64(); // Unk: Address?

      /*0x20*/
      _ = reader.ReadInt32(); // Unk: Count?
      /*0x24*/
      reader.Seek( 0x10, SeekOrigin.Current ); // 0x10 bytes of idk
      /*0x34*/
      reader.Seek( 0x1A, SeekOrigin.Current ); // 0x1E bytes of idk

      /*0x52*/
      var modelPathLength = reader.ReadInt32();
      var modelPath = reader.ReadFixedLengthString( modelPathLength );
      reader.Seek( 0x13, SeekOrigin.Current );

      //===============================================================

      const long MAGIC_1SERTPL = 0x006C707452455331;
      Assert( reader.ReadInt64(), MAGIC_1SERTPL, "Invalid 1SERTPL Magic." );

      _ = reader.ReadInt64(); // Unk: Address?
      _ = reader.ReadInt64(); // Unk: Flags/Count?
      _ = reader.ReadInt64(); // Unk: Address?
      _ = reader.ReadInt32(); // Unk: Null/reserved?

      reader.Seek( 0x10, SeekOrigin.Current ); // Unk data: guid?

      _ = reader.ReadInt32(); // Unk: Count?
      _ = reader.ReadInt32(); // Unk: Count?

      // I think this is some sort of metadata. Skipping.
      var metadataBytes = reader.ReadInt32();
      reader.Seek( metadataBytes, SeekOrigin.Current );
    }

    private void ReadTPL1Block( BinaryReader reader )
    {
      const int MAGIC_TPL1 = 0x314C5054;
      Assert( reader.ReadInt32(), MAGIC_TPL1, "Invalid TPL1 Magic." );

      _ = reader.ReadInt32(); // Unk: count?
      _ = reader.ReadInt16(); // Unk: count?

      var modelNameLength = reader.ReadInt32();
      var modelName = reader.ReadFixedLengthString( modelNameLength );

      _ = reader.ReadInt16(); // Unk: Type?
      _ = reader.ReadInt32(); // Unk: Length/count?

      var exportStringLength = reader.ReadInt32();
      var exportString = reader.ReadFixedLengthString( exportStringLength );

      var typeStringLength = reader.ReadInt32();
      var typeString = reader.ReadFixedLengthString( typeStringLength );

      _ = reader.ReadInt32(); // Unk: Count?
      _ = reader.ReadInt16(); // Unk: Count?
      _ = reader.ReadByte(); // Unk
    }

    private void ReadTextureNames( BinaryReader reader )
    {
      var numEntries = reader.ReadInt32();
      _ = reader.ReadInt16(); // Unk
      _ = reader.ReadInt32(); // Unk

      var names = new List<string>();

      for ( var i = 0; i < numEntries; i++ )
      {
        var nameLen = reader.ReadInt16();
        var name = reader.ReadFixedLengthString( nameLen );
        names.Add( name );
      }

      _ = reader.ReadInt16(); // Unk
      _ = reader.ReadInt32(); // Unk: count?

      return;
    }

    private void ReadOgmBlock( BinaryReader reader )
    {
      /* Notes:
       * Each section starting with 0x00 is empty. Skip.
       * Each section starting with 0x08 seems to be related to scripting.
       */

      FindOGM1( reader.BaseStream ); // TODO: Not this

      const int MAGIC_OGM1 = 0x314D474F;
      Assert( reader.ReadInt32(), MAGIC_OGM1, "Invalid OGM Magic" );

      // Scale/Pos XYZ?
      _ = reader.ReadInt16();
      _ = reader.ReadInt16();
      _ = reader.ReadInt16();

      _ = reader.ReadByte();

      var nodeCount = reader.ReadInt16();

      // Scale/POS XYZ?
      _ = reader.ReadInt16();
      _ = reader.ReadInt16();
      _ = reader.ReadInt16();

      // Section - Node Indices
      if ( reader.ReadBoolean() )
      {
        var nodeIndices = new List<short>();
        for ( var i = 0; i < nodeCount; i++ )
          nodeIndices.Add( reader.ReadInt16() );
      }

      // Unknown Section 1 - Bone Names?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          var boneNameLength = reader.ReadInt32();
          var boneName = reader.ReadFixedLengthString( boneNameLength );
        }
      }

      // Unknown Section 2
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          reader.ReadInt32();
          reader.ReadInt16();
          reader.ReadByte();
        }
      }

      // Unknown Section 3
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
          _ = reader.ReadInt16(); // Unk: Index
      }

      // Unknown Section 4
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
          _ = reader.ReadInt16(); // Unk: Index
      }

      // Unknown Section 5
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
          _ = reader.ReadInt16(); // Unk: Index
      }

      // Unknown Section 6
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
          _ = reader.ReadInt16();
      }

      // Unknown Section 7
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
          _ = reader.ReadInt16();
      }

      // Unknown Section 8 - Export Strings?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          var exportStringLength = reader.ReadInt32();
          var exportString = reader.ReadFixedLengthString( exportStringLength );
        }
      }

      // Unknown Section 9 - Bone Matrices?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
        }
      }

      // Unknown Section 10 - Another Matrix?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
          reader.ReadSingle();
        }
      }

      // The Fuck-You Section
      // I have no idea what this is or how to properly parse it
      // It has 0x0700000003 every 0x20 bytes followed by some floats
      if ( reader.ReadByte() == 0x02 )
      {
        for ( var i = 0; i < 0x100; i++ )
        {
          var peek = reader.PeekInt32();
          if ( peek == 0x03 )
            break;

          reader.BaseStream.Position++;
        }

        while ( true )
        {
          while ( reader.PeekInt32() == 0x03 )
            reader.BaseStream.Position += 0x25;

          var originalPos = reader.BaseStream.Position;

          var peek = 0;
          for ( var i = 0; i < 0x20; i++ )
          {
            peek = reader.PeekInt32();
            if ( peek == 0x03 )
              break;

            reader.BaseStream.Position++;
          }

          if ( peek != 0x03 )
          {
            reader.BaseStream.Position = originalPos;
            break;
          }
        }
      }

      // Section 12 - Bone Chains?
      if ( reader.ReadBoolean() )
      {
        var x = 0;
        for ( var i = 0; i < nodeCount; i++ )
        {
          x++;
          var chainStringLength = reader.ReadInt32();
          var chainString = reader.ReadFixedLengthString( chainStringLength );
        }
      }

      // Section 13 - Padding?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          for ( var j = 0; j < 60; j++ )
            reader.ReadByte();
        }
      }

      // Section 14 - Bone Names? Strings?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          var boneNameLen = reader.ReadInt16();
          var boneName = reader.ReadFixedLengthString( boneNameLen );
        }
      }

      // Section 15 - Export Strings?
      if ( reader.ReadBoolean() )
      {
        for ( var i = 0; i < nodeCount; i++ )
        {
          var exportNameLen = reader.ReadInt16();
          var exportName = reader.ReadFixedLengthString( exportNameLen );
        }
      }

    }

    private void ReadChunks( BinaryReader reader )
    {
      var chunkTag = reader.ReadInt16();
      var chunkEnd = reader.ReadInt32();

      var rootNodeIndex = reader.ReadInt16() + 1;
      var nodeCount = reader.ReadInt32();
      var bufferCount = reader.ReadInt32();
      var meshCount = reader.ReadInt32();
      var subMeshCount = reader.ReadInt32();

      _ = reader.ReadInt64();

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();

      var bufferTypes = new BufferType[ bufferCount ];
      for ( var i = 0; i < bufferCount; i++ )
        bufferTypes[ i ] = BufferType.Read( reader );

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();
      var bufferElemSizeArray = new ushort[ bufferCount ];
      for ( var i = 0; i < bufferCount; i++ )
        bufferElemSizeArray[ i ] = reader.ReadUInt16();

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();
      var bufferLengthArray = new uint[ bufferCount ];
      for ( var i = 0; i < bufferCount; i++ )
        bufferLengthArray[ i ] = reader.ReadUInt32();

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();
      var bufferInfo = new BufferInfo();
      for ( var i = 0; i < bufferCount; i++ )
      {
        switch ( bufferTypes[ i ].Type )
        {
          case 0x00:
            bufferInfo.FaceBufferOffset = reader.BaseStream.Position;
            break;
          case 0x02:
            bufferInfo.Unk_FaceOffset = reader.BaseStream.Position;
            break;
          case 0x0C:
            bufferInfo.StaticVertBufferOffset = reader.BaseStream.Position;
            break;
          case 0x0F:
            bufferInfo.SkinnedVertBufferOffset = reader.BaseStream.Position;
            break;
          case 0x10:
            bufferInfo.NormalTextureVertBufferOffset = reader.BaseStream.Position;
            break;
          case 0x30:
            bufferInfo.Unk_VertexOffset30 = reader.BaseStream.Position;
            break;
          case 0x70:
            bufferInfo.Unk_VertexOffset70 = reader.BaseStream.Position;
            break;
          default:
            throw new Exception( $"Unparsed buffer type: 0x{bufferTypes[ i ].Type:X}" );
        }

        reader.Seek( bufferLengthArray[ i ], SeekOrigin.Current );
      }

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();
      var meshDataArray = new MeshData[ meshCount ][];
      for ( var i = 0; i < meshCount; i++ )
      {
        var subArraySize = reader.ReadByte();
        meshDataArray[ i ] = new MeshData[ subArraySize ];
        for ( var j = 0; j < subArraySize; j++ )
          meshDataArray[ i ][ j ] = MeshData.Read( reader );
      }

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();
      var unkMeshTableArray = new UnkMeshTable[ meshCount ];
      for ( var i = 0; i < unkMeshTableArray.Length; i++ )
        unkMeshTableArray[ i ] = UnkMeshTable.Read( reader );

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();

      chunkTag = reader.ReadInt16();
      chunkEnd = reader.ReadInt32();

    }

    #endregion

    private static void Assert<T>( T actual, T expected, string message = null )
    {
      if ( !actual.Equals( expected ) )
        throw new Exception( message );
    }

    private void FindOGM1( Stream Stream )
    {
      Stream.Position = 0;
      long found = -1;
      int curr;
      while ( ( curr = Stream.ReadByte() ) > -1 )
      {
        if ( curr == 'O' )
        {
          byte[] buffer = new byte[ 3 ];
          Stream.Read( buffer, 0, 3 );
          if ( buffer[ 0 ] == 'G' && buffer[ 1 ] == 'M' && buffer[ 2 ] == '1' )
          {
            found = Stream.Position - 4;
            break;
          }
        }
      }

      if ( found > -1 )
        Stream.Position = found;
    }

    #region Embedded Types

    internal struct BufferType
    {
      public ushort Unk_01;
      public byte Unk_02;
      public byte Type;
      public byte Unk_04;
      public byte Unk_05;
      public uint Unk_06;

      public static BufferType Read( BinaryReader reader )
      {
        return new BufferType
        {
          Unk_01 = reader.ReadUInt16(),
          Unk_02 = reader.ReadByte(),
          Type = reader.ReadByte(),
          Unk_04 = reader.ReadByte(),
          Unk_05 = reader.ReadByte(),
          Unk_06 = reader.ReadUInt32()
        };
      }
    }

    internal class BufferInfo
    {
      public long FaceBufferOffset;
      public long Unk_BufferOffset;
      public long StaticVertBufferOffset;
      public long SkinnedVertBufferOffset;
      public long NormalTextureVertBufferOffset;
      public long Unk_FaceOffset;
      public long Unk_VertexOffset30;
      public long Unk_VertexOffset70;
    }

    internal struct MeshData
    {
      public uint BufferId;
      public uint SubBufferOffset;

      public static MeshData Read( BinaryReader reader )
      {
        return new MeshData
        {
          BufferId = reader.ReadUInt32(),// + 1
          SubBufferOffset = reader.ReadUInt32()
        };
      }
    }

    internal struct UnkMeshTable
    {
      public ushort Unk_1;
      public byte Unk_2;
      public byte Unk_3;
      public byte Unk_4;
      public byte Unk_5;
      public uint Unk_6;

      public static UnkMeshTable Read( BinaryReader reader )
      {
        return new UnkMeshTable
        {
          Unk_1 = reader.ReadUInt16(),
          Unk_2 = reader.ReadByte(),
          Unk_3 = reader.ReadByte(),
          Unk_4 = reader.ReadByte(),
          Unk_5 = reader.ReadByte(),
          Unk_6 = reader.ReadUInt32(),
        };
      }
    }

    #endregion

  }

}

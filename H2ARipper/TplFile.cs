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

      _ = reader.ReadByte();

      var nodeIndices = new List<short>();
      for ( var i = 0; i < nodeCount; i++ )
        nodeIndices.Add( reader.ReadInt16() );

      _ = reader.ReadByte(); // sep?

      // Unknown 1
      for ( var i = 0; i < nodeCount; i++ )
      {
        reader.ReadInt32();
        reader.ReadInt16();
        reader.ReadByte();
      }

      _ = reader.ReadByte(); // sep?
      _ = reader.ReadByte(); // sep?

      // Unknown 2
      for ( var i = 0; i < nodeCount; i++ )
      {
        _ = reader.ReadInt16();
        _ = reader.ReadInt16();
        _ = reader.ReadInt16();
      }

      reader.ReadByte();

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

  }

}

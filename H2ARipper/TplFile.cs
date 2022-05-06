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

      ReadHeader( reader );
      ReadTextureNames( reader );
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
      reader.Seek( 0x1C, SeekOrigin.Current ); // Unk data

      //===============================================================

      const int MAGIC_TPL1 = 0x314C5054;
      Assert( reader.ReadInt32(), MAGIC_TPL1, "Invalid TPL1 Magic." );

      _ = reader.ReadInt32(); // Unk: count?
      _ = reader.ReadInt16(); // Unk: count?

      var modelNameLength = reader.ReadInt32();
      var modelName = reader.ReadFixedLengthString( modelNameLength );

      reader.Seek( 0x1E, SeekOrigin.Current ); // Unknown Data
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
      const int MAGIC_OGM1 = 0x314D474F;
      Assert( reader.ReadInt32(), MAGIC_OGM1, "Invalid OGM Magic" );

      reader.Seek( 0x7, SeekOrigin.Current ); // Unk data
      var nodeCount = reader.ReadInt32();
      _ = reader.ReadInt32(); // Unk: count?

      var nodeIdArray = new short[ nodeCount ];
      var boneNameArray = new string[ nodeCount ];

      // Read NodeId Array
      //if ( reader.ReadByte() > 0 )
      //{
      //  for ( var i = 0; i < nodeCount; i++ )
      //    nodeIdArray[ i ] = reader.ReadInt16();
      //}
      for ( var j = 0; j < 11; j++ )
      {
        if ( reader.ReadByte() > 0 )
          for ( var i = 0; i < nodeCount; i++ )
            _ = reader.ReadByte();
      }


    }

    #endregion

    private static void Assert<T>( T actual, T expected, string message = null )
    {
      if ( !actual.Equals( expected ) )
        throw new Exception( message );
    }

  }

}

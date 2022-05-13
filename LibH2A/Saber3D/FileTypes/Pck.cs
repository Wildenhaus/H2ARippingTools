// Edit: 5/13 - Zatarita
//   Changes:
//     Created the class
//     Removed caching data over memory paging issues
//   Todo:
//      MEMORY MANAGEMENT


namespace Saber3D.FileTypes
{
  public class SERPak
  {
    public class PckEntry
    {
      public long Offset { get; set; }
      public long Size { get; set; }

      public PckEntry( long offset, long size )
      {
        Offset = offset;
        Size = size;
      }

      // Loads the data into memory if it hasn't already, then returns the data.
      public byte[] Data( in BinaryReader stream )
      {
        var data = new byte[ Size ];

        stream.BaseStream.Seek( Offset, SeekOrigin.Begin );
        stream.Read( data, 0, ( int ) Size );

        return data;
      }
    }

    private BinaryReader _parser;
    private Dictionary<string, PckEntry> _entries;

    private const int CHILDREN_START = 0x45;


    public SERPak( in Stream strm )
    {
      _parser = new BinaryReader( strm );
      _entries = new Dictionary<string, PckEntry>();

      Initialize();
    }

    public byte[] GetData( in string name )
    {
      if ( !_entries.ContainsKey( name ) )
        throw new ArgumentException();
      return _entries[ name ].Data( _parser );
    }

    public List<string> GetNames()
    {
      List<string> names = new List<string>();

      foreach ( var entry in _entries )
        names.Add( entry.Key );

      return names;
    }

    // These be the three important ones really
    public List<string> getTpls()
    {
      return GetExtension( "tpl" );
    }

    public List<string> getPcts()
    {
      return GetExtension( "pct" );
    }
    public List<string> getScn()
    {
      return GetExtension( "scn" );
    }

    // Generalized
    public List<string> GetExtension( in string ext )
    {
      List<string> names = GetNames();
      List<string> ret = new List<string>();

      foreach ( var name in names )
        if ( name.ToLower().EndsWith( $".{ext}" ) )
          ret.Add( name );

      return ret;
    }

    private void Initialize()
    {
      // For extraction purposes, I don't care about any data that comes before this offset.
      _parser.BaseStream.Seek( CHILDREN_START, SeekOrigin.Begin );

      int count = _parser.ReadInt32();

      _ = _parser.ReadInt32();        // Burn unknown
      _ = _parser.ReadChar();         // Burn delimiter

      List<string> names = ParseNames( count );
      _ = _parser.ReadChar();         // Burn delimiter

      List<long> offsets = ParseOffsets( count );
      _ = _parser.ReadChar();         // Burn delimiter

      List<int> sizes = ParseSizes( count );

      for ( int i = 0; i < count; i++ )
        _entries.Add( names[ i ], new PckEntry( offsets[ i ], sizes[ i ] ) );
    }

    private List<string> ParseNames( int count )
    {
      List<string> ret = new List<string>();

      for ( int i = 0; i < count; i++ )
        ret.Add( new string( _parser.ReadChars( _parser.ReadInt32() ) ) );

      return ret;
    }

    private List<long> ParseOffsets( int count )
    {
      List<long> ret = new List<long>();

      for ( int i = 0; i < count; i++ )
        ret.Add( _parser.ReadInt64() );

      return ret;
    }

    private List<int> ParseSizes( int count )
    {
      List<int> ret = new List<int>();

      for ( int i = 0; i < count; i++ )
        ret.Add( _parser.ReadInt32() );

      return ret;
    }

  }

  public class Pck : SERPak
  {
    public Pck( in Stream strm ) : base( strm ) { }
  }

}

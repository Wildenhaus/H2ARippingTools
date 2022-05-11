using LibH2A.Common;
using LibH2A.Saber3D.Geometry;
using static Haus.Assertions;

namespace LibH2A.Saber3D
{

  public class S3D_Template
  {

    #region Data Members

    private Stream _stream;

    #endregion

    #region Properties

    public S3D_TemplateHeader Header { get; private set; }

    public S3D_Geometry Geometry { get; private set; }

    public Stream Stream
    {
      get => _stream; //TODO
    }

    #endregion

    #region Constructor

    private S3D_Template( Stream stream )
    {
      Assert( stream != null, "Stream cannot be null." );

      _stream = stream;
    }

    public static S3D_Template Open( Stream stream )
    {
      var tplFile = new S3D_Template( stream );
      tplFile.Init();

      return tplFile;
    }

    #endregion

    #region Private Methods

    private void Init()
    {
      Assert( _stream != null, "File stream is null." );

      // TODO: Remove these hacks once Zata's PAK streaming is ready
      var stream = _stream = CreateTplStreamSegment();
      var reader = new EndianBinaryReader( stream, leaveOpen: true );

      stream.Position = FindDataOffset( stream, MAGIC_TPL1 );

      Header = S3D_TemplateHeader.Read( reader );

      stream.Position = FindDataOffset( stream, MAGIC_OGM1 );

      Geometry = S3D_Geometry.Read( reader );
    }

    #endregion

    #region Private Methods


    #endregion

    private static readonly byte[] MAGIC_1SERtpl = new byte[]
    {
      0x31, 0x53, 0x45, 0x52, 0x74, 0x70, 0x6C, 0x00
    };

    private static readonly byte[] MAGIC_TPL1 = new byte[]
    {
      0x54, 0x50, 0x4C, 0x31
    };

    private static readonly byte[] MAGIC_OGM1 = new byte[]
    {
      0x4F, 0x47, 0x4D, 0x31
    };

    private static long FindDataOffset( Stream stream, byte[] data )
    {
      stream.Position = 0;
      long found = -1;
      int curr;
      while ( ( curr = stream.ReadByte() ) > -1 )
      {
        if ( curr == data[ 0 ] )
        {
          stream.Position--;
          byte[] buffer = new byte[ data.Length ];
          stream.Read( buffer, 0, data.Length );
          if ( buffer.SequenceEqual( data ) )
          {
            found = stream.Position - data.Length;
            break;
          }
          else
            stream.Position -= data.Length - 1;
        }
      }

      return found;
    }

    private Stream CreateTplStreamSegment()
    {
      var memoryStream = new MemoryStream();
      _stream.Position = FindDataOffset( _stream, MAGIC_1SERtpl );
      _stream.CopyTo( memoryStream );

      memoryStream.Position = 0;
      return memoryStream;
    }

  }

}

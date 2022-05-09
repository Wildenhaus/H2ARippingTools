using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D
{

  public class S3D_Template
  {

    #region Data Members

    private readonly Stream _stream;

    #endregion

    #region Properties

    public S3D_TemplateHeader Header { get; private set; }

    public S3D_Geometry Geometry { get; private set; }

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
      var reader = new EndianBinaryReader( _stream, leaveOpen: true );

      // TODO: Remove this hack once Zata's PAK streaming is ready
      SeekToTpl1();

      Header = S3D_TemplateHeader.Read( reader );

      // TODO: Remove this hack once TPL1 parsing is ready
      SeekToOGM1();

      Geometry = S3D_Geometry.Read( reader );
    }

    #endregion

    #region Private Methods


    #endregion

    private void SeekToTpl1()
    {
      _stream.Position = 0;
      long found = -1;
      int curr;
      while ( ( curr = _stream.ReadByte() ) > -1 )
      {
        if ( curr == 'T' )
        {
          byte[] buffer = new byte[ 3 ];
          _stream.Read( buffer, 0, 3 );
          if ( buffer[ 0 ] == 'P' && buffer[ 1 ] == 'L' && buffer[ 2 ] == '1' )
          {
            found = _stream.Position - 4;
            break;
          }
        }
      }

      if ( found > -1 )
        _stream.Position = found;
    }

    private void SeekToOGM1()
    {
      _stream.Position = 0;
      long found = -1;
      int curr;
      while ( ( curr = _stream.ReadByte() ) > -1 )
      {
        if ( curr == 'O' )
        {
          byte[] buffer = new byte[ 3 ];
          _stream.Read( buffer, 0, 3 );
          if ( buffer[ 0 ] == 'G' && buffer[ 1 ] == 'M' && buffer[ 2 ] == '1' )
          {
            found = _stream.Position - 4;
            break;
          }
          else
            _stream.Position -= 3;
        }
      }

      if ( found > -1 )
        _stream.Position = found;
    }

    private void CreateTplStreamSegment()
    {

    }

  }

}

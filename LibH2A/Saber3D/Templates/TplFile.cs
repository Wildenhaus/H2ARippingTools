using LibH2A.Saber3D.Metadata;
using static Haus.Assertions;

namespace LibH2A.Saber3D.Templates
{

  public class TplFile : IDisposable
  {

    #region Data Members

    private readonly string _filePath;
    private readonly Stream _stream;

    private bool _isDisposed;

    #endregion

    #region Properties

    public S3D_PakMetadata PakMetadata { get; private set; }
    public S3D_TplMetadata TplMetadata { get; private set; }

    #endregion

    #region Constructor

    private TplFile( string filePath )
      : this( File.OpenRead( filePath ) )
    {
      Assert( File.Exists( filePath ), $"File not found: {filePath}" );

      _filePath = filePath;
    }

    private TplFile( Stream stream )
    {
      Assert( stream != null, "Stream cannot be null." );

      _stream = stream;
    }

    public static TplFile Open( string filePath )
    {
      var tplFile = new TplFile( filePath );
      tplFile.Init();

      return tplFile;
    }

    public static TplFile Open( Stream stream )
    {
      var tplFile = new TplFile( stream );
      tplFile.Init();

      return tplFile;
    }

    #endregion

    #region Private Methods

    private void Init()
    {
      Assert( _stream != null, "File stream is null." );

      PakMetadata = S3D_PakMetadata.Read( _stream );
      TplMetadata = S3D_TplMetadata.Read( _stream );
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      if ( _isDisposed )
        return;

      _stream?.Dispose();

      _isDisposed = true;
    }

    #endregion

  }

}

using Saber3D.Compression;
using Saber3D.FileTypes;

namespace H2ARipper.Commands
{

  public abstract class Command<TOptions>
  {

    public abstract int Execute( TOptions options );

    protected void EnsureDirectoryExists( ref string directoryPath )
    {
      if ( directoryPath.EndsWith( "\"" ) )
        directoryPath = directoryPath.Substring( 0, directoryPath.Length - 1 );

      if ( !Path.IsPathFullyQualified( directoryPath ) )
      {
        var currentDir = Environment.CurrentDirectory;
        var dirName = Path.GetDirectoryName( directoryPath );
        directoryPath = Path.Combine( currentDir, dirName );
      }

      if ( !Directory.Exists( directoryPath ) )
        Directory.CreateDirectory( directoryPath );
    }

    protected void EnsureFileExists( ref string filePath )
    {
      if ( !Path.IsPathFullyQualified( filePath ) )
      {
        var currentDir = Environment.CurrentDirectory;
        var fileName = Path.GetFileName( filePath );
        filePath = Path.Combine( currentDir, fileName );
      }

      if ( !File.Exists( filePath ) )
        throw new FileNotFoundException( "File does not exist.", filePath );
    }

    protected Pck GetPckFile( string filePath )
    {
      EnsureFileExists( ref filePath );

      var decompressionStream = new H2ADecompressionStream( filePath );
      return new Pck( decompressionStream );
    }

    protected IEnumerable<string> GetPckFiles( Pck pck, IEnumerable<string> filters = null )
    {
      foreach ( var fileName in pck.GetNames() )
      {
        if ( filters == null || !filters.Any() )
          yield return fileName;

        foreach ( var filter in filters )
        {
          if ( fileName.Contains( filter, StringComparison.InvariantCultureIgnoreCase ) )
          {
            yield return fileName;
            break;
          }
        }
      }
    }

    protected string SanitizeFileName( string fileName )
    {
      // Remove ':' delimiter
      if ( fileName.Contains( ':' ) )
        fileName = fileName.Split( ':' ).Last();

      // Some paths are fully qualified
      fileName = Path.GetFileName( fileName );

      return fileName;
    }

    protected void Log( string message, ConsoleColor color = ConsoleColor.White )
    {
      Console.ForegroundColor = color;
      Console.Write( message );
    }

    protected void LogLine( string message, ConsoleColor color = ConsoleColor.White )
    {
      Console.ForegroundColor = color;
      Console.WriteLine( message );
    }

  }

}

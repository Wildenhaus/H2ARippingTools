using CommandLine;
using Saber3D.FileTypes;

namespace H2ARipper.Commands
{

  [Verb( "extract", HelpText = "Extracts a .pck file to a directory." )]
  public class ExtractCommandOptions
  {

    [Value( 0,
      MetaName = "path",
      HelpText = "The path of the .pck file" )]
    public string FilePath { get; set; }

    [Value( 0,
      MetaName = "dest",
      HelpText = "The path to extract files to" )]
    public string DestinationPath { get; set; }

    [Option( "filter",
      Separator = '|',
      Required = false,
      HelpText = "Filter by patterns, delimited by '|'. Wildcard not supported." )]
    public IEnumerable<string> Filters { get; set; }

  }

  public class ExtractCommand : Command<ExtractCommandOptions>
  {

    public override int Execute( ExtractCommandOptions options )
    {
      var destDirPath = options.DestinationPath;
      EnsureDirectoryExists( ref destDirPath );

      var pck = GetPckFile( options.FilePath );
      var results = GetPckFileNames( pck, options.Filters );

      if ( !results.Any() )
      {
        Console.WriteLine( "Nothing to extract." );
        return 0;
      }

      var count = 0;
      var successful = 0;
      foreach ( var result in results )
      {
        count++;
        var success = ExtractFile( pck, result, destDirPath );
        if ( success )
          successful++;
      }

      LogLine( $"{successful}/{count} files successfully extracted." );

      return 0;
    }

    private bool ExtractFile( Pck pck, string fileName, string destDirPath )
    {
      var destPath = Path.Combine( destDirPath, SanitizeFileName( fileName ) );

      try
      {
        Log( $"Extracting {fileName}..." );
        var fileData = pck.GetData( fileName );
        using ( var outStream = File.Create( destPath ) )
        {
          outStream.Write( fileData );
          outStream.Flush();
        }

        LogLine( "DONE", ConsoleColor.Green );
        return true;
      }
      catch ( Exception ex )
      {
        LogLine( "FAILED", ConsoleColor.Red );
        LogLine( $"\tReason: {ex.Message}", ConsoleColor.Red );
        return false;
      }
    }

  }

}

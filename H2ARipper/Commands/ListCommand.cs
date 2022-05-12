using CommandLine;

namespace H2ARipper.Commands
{

  [Verb( "list", HelpText = "List the contents of a .pck file" )]
  public class ListCommandOptions
  {

    [Value( 0,
      MetaName = "path",
      HelpText = "The path of the .pck file" )]
    public string FilePath { get; set; }

    [Option( "filter",
      Separator = '|',
      Required = false,
      HelpText = "Filter by patterns, delimited by '|'. Wildcard not supported." )]
    public IEnumerable<string> Filters { get; set; }

  }

  public class ListCommand : Command<ListCommandOptions>
  {

    public override int Execute( ListCommandOptions options )
    {
      var pck = GetPckFile( options.FilePath );
      var results = GetPckFileNames( pck, options.Filters );
      PrintResults( results, options );

      Console.WriteLine();
      return 0;
    }

    private void PrintResults( IEnumerable<string> results, ListCommandOptions options )
    {
      LogLine( Path.GetFileName( options.FilePath ) );
      LogLine( new String( '-', 40 ) );

      var count = 0;
      foreach ( var result in results )
      {
        count++;
        LogLine( $"  - {result}" );
      }

      if ( count == 0 )
        LogLine( "  No results found." );
    }

  }

}

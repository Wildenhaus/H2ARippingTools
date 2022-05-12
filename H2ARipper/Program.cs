using CommandLine;
using CommandLine.Text;
using H2ARipper.Commands;

namespace H2ARipper
{

  public class App
  {

    public static int Main( string[] args )
    {
      PrintHeader();

      var parser = new CommandLine.Parser( with => with.HelpWriter = null );

      var result = parser
        .ParseArguments<
          ConvertCommandOptions,
          ExtractCommandOptions,
          ListCommandOptions>( args );

      result.WithNotParsed( err => DisplayHelp( result, err ) );

      return result.MapResult(
        ( ConvertCommandOptions opts ) => new ConvertCommand().Execute( opts ),
        ( ExtractCommandOptions opts ) => new ExtractCommand().Execute( opts ),
        ( ListCommandOptions opts ) => new ListCommand().Execute( opts ),
        errors => 1
      );

    }

    private static void PrintHeader()
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine( "H2ARipper v0.1a" );
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine( " by Haus, Zatarita, Unordinal, sleepyzay, et. al." );
      Console.WriteLine();
    }

    private static void DisplayHelp( ParserResult<object> result, IEnumerable<Error> errors )
    {
      var helpText = HelpText.AutoBuild( result, h =>
      {
        h.AdditionalNewLineAfterOption = false;
        return HelpText.DefaultParsingErrorsHandler( result, h );
      }, e => e );

      // Skip Copyright
      var lines = helpText.ToString().Split( "\n" );
      foreach ( var line in lines.Skip( 3 ) )
        Console.WriteLine( line );
    }

  }

}
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

      var result = Parser.Default
        .ParseArguments<
          ConvertCommandOptions,
          ExtractCommandOptions,
          ListCommandOptions>( args );

      HelpText.AutoBuild( result, help =>
      {
        help.AdditionalNewLineAfterOption = false;
        help.Heading = "";
        help.Copyright = "";
        return help;
      }, error =>
      {
        return error;
      } );

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

  }

}
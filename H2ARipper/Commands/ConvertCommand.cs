using CommandLine;

namespace H2ARipper.Commands
{

  [Verb( "convert", HelpText = "Converts the specified file." )]
  public class ConvertCommandOptions
  {

  }

  public class ConvertCommand : Command<ConvertCommandOptions>
  {

    public override int Execute( ConvertCommandOptions options )
    {
      throw new NotImplementedException();
    }

  }

}

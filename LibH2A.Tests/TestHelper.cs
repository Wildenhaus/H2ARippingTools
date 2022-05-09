using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibH2A.Tests
{

  public static class TestHelper
  {

    // Make this the path to your game data!
    public static readonly string GameDataPath = @"G:\h2a\d\";

    public static IEnumerable<string> GetAllFilesWithExtension( string extension )
      => Directory.EnumerateFiles( GameDataPath, extension, SearchOption.AllDirectories );

    public static string GetFile( string fileName )
      => Directory.EnumerateFiles( GameDataPath, fileName, SearchOption.AllDirectories ).First();

  }

}

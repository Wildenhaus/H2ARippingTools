using CommandLine;
using H2ARipper.Converters;
using LibH2A.Saber3D;
using Saber3D.FileTypes;

// Edit: 05/13 - Zatarita
//   Changes:
//     Added directory command to parse files in directory
//     Added short names to commands
//     Make scene Pcks load from the Scn instead of the global pck scope
//     Added pct support
//     Fixed directory structure.
//        Now creates directories if they dont exist
//        Now mimics internal file structure in pck
//     Added an overwrite flag to overwrite existing tags

namespace H2ARipper.Commands
{

  [Verb( "convert", HelpText = "Converts the specified file." )]
  public class ConvertCommandOptions
  {

    [Value( 0,
      MetaName = "inpath",
      HelpText = "The file to convert" )]
    public string InPath { get; set; }

    [Option( shortName: 'o', longName: "outpath",
      HelpText = "The output path. If not specified, it will be placed in the current directory." )]
    public string OutPath { get; set; }

    [Option( shortName: 'f', longName: "filter",
      HelpText = "Filter files to convert. Delimited by '|'. Only used when converting .pck files." )]
    public IEnumerable<string> Filters { get; set; }

    [Option( shortName: 'd', longName: "directory", HelpText = "Convert the files in a directory." )]
    public bool Recursive { get; set; }

    [Option( shortName: 'w', longName: "overwrite", HelpText = "Overwrite existing files." )]
    public bool Overwrite { get; set; }

  }

  public class ConvertCommand : Command<ConvertCommandOptions>
  {

    public override int Execute( ConvertCommandOptions options )
    {

      LogLine( $"Converting:\t{options.InPath}\nTo:\t\t{options.OutPath}", ConsoleColor.Yellow );
      if(options.Recursive)
        AsyncParseDirectory(options);
      else
        DoExecute(options);

      return 0;
    }

    private void DoExecute( ConvertCommandOptions options )
    {
      var inPath = options.InPath;

      if ( string.IsNullOrWhiteSpace( inPath ) )
        inPath = Environment.CurrentDirectory;

      EnsureFileExists( ref inPath );

      var outPath = options.OutPath;
      if ( string.IsNullOrWhiteSpace( outPath ) )
        outPath = Environment.CurrentDirectory;
      else if ( !IsPathFile( outPath ) )
        EnsureDirectoryExists( ref outPath );

      DelegateCommand( options.InPath, outPath, options );
    }

    private void AsyncParseDirectory( ConvertCommandOptions options )
    {
      var directory = options.InPath;
      if ( !Directory.Exists( directory ) )
        throw new ArgumentException("Unknown Directory Requested!");

      var outPath = options.OutPath;
      if ( string.IsNullOrWhiteSpace( outPath ) )
        outPath = Environment.CurrentDirectory;
      else if ( !IsPathFile( outPath ) )
        EnsureDirectoryExists( ref outPath );

      string[] files = Directory.GetFiles( directory );
      Parallel.For( 0, files.Length,
                    index => {
                      if ( Path.GetExtension( files[ index ] ) is var ext && ext == ".pct" || ext == ".pck" )
                        DelegateCommand( files[ index ], outPath, options );
                   } );

    }

    private void DelegateCommand( in string inPath, in string outPath, in ConvertCommandOptions options )
    {
      if ( IsPckFile( inPath ) )
        ConvertFromPckFile( GetPckFile(inPath), outPath + "\\" + Path.GetFileNameWithoutExtension(inPath), options );
      else
        ConvertRawFile( inPath, outPath );
    }

    private void ConvertFromPckFile( in Pck pck, string outPath, in ConvertCommandOptions options)
    {
      var abort = false;
      //var pck = GetPckFile( inPath );
      var targetFiles = GetPckFileNames( pck, options.Filters );

      foreach(var targetFile in targetFiles)
      {
        if ( !Directory.Exists( outPath ) )
          Directory.CreateDirectory( outPath );
        var outFileName = targetFile.Replace( "<", "" ).Replace( ">", "" ).Replace( ":", "\\" );
        var outFilePath = Path.Combine( outPath, outFileName );

        if ( Path.HasExtension( outFilePath ) && Path.GetExtension( outFilePath ) == ".tpl" )
          outFilePath = Path.ChangeExtension( outFilePath, "fbx" );
        else if ( Path.GetExtension( outFilePath ) == ".pct" )
          outFilePath = Path.ChangeExtension( outFilePath, "dds" );
        else if ( Path.GetExtension( outFilePath ) != ".scn" )
          continue;

        if ( File.Exists( outFilePath ) && !options.Overwrite )
        {
          LogLine( $"Skipping Existing: {targetFile}...", ConsoleColor.Yellow );
          continue;
        }

        try
        {
          if ( targetFile.EndsWith( ".scn" ) )
            LogLine( $"Decompressing Scene {targetFile}...", ConsoleColor.Cyan );

          var data = pck.GetData( targetFile );
          if ( data.Length == 0 )
            continue;

          if ( targetFile.EndsWith( ".scn" ) )
          {
            var scnData = new MemoryStream( data );
            var stream = new Pck( scnData );
            LogLine( $"Converting Scene {targetFile}...", ConsoleColor.Cyan );
            ConvertFromPckFile( stream, outPath, options );
            continue;
          }

          var result = ConvertFile( targetFile, new MemoryStream( data ) );
          if ( !result.Successful )
          {
            abort = result.ShouldAbort;
            throw new Exception( result.Message );
          }

          WriteStreamToFile( result.ConvertedStream, outFilePath );
          LogLine( $"Written to {outFilePath}", ConsoleColor.Cyan );
        }
        catch ( Exception ex )
        {
          LogLine( $"FAILED: {outFilePath}", ConsoleColor.Red );
          LogLine( $"  Reason: {ex.Message}", ConsoleColor.Red );
        }
      }
    }

    private void ConvertRawFile( string inPath, string outPath )
    {
      var data = File.OpenRead( inPath );
      if ( data.Length == 0 )
        throw new Exception( "File contains no data." );

      var outFilePath = outPath;
      if ( !IsPathFile( outFilePath ) )
        outFilePath = Path.Combine( outFilePath, Path.GetFileName( inPath ) );

      if(Path.HasExtension( outFilePath ) && Path.GetExtension(outFilePath) == ".tpl")
        outFilePath = Path.ChangeExtension( outFilePath, "fbx" );
      else
        outFilePath = Path.ChangeExtension( outFilePath, "dds" );

      try
      {
        Log( $"Converting {Path.GetFileName( inPath )}..." );

        var result = ConvertFile( inPath, data );
        if ( !result.Successful )
          throw new Exception( result.Message );

        WriteStreamToFile( result.ConvertedStream, outFilePath );
        LogLine( "DONE", ConsoleColor.Green );
        LogLine( $"  Written to {outFilePath}", ConsoleColor.Yellow );
      }
      catch ( Exception ex )
      {
        LogLine( "FAILED", ConsoleColor.Red );
        LogLine( $"  Reason: {ex.Message}", ConsoleColor.Red );
      }
    }

    private ConversionResult ConvertFile( string inPath, Stream data )
    {
      var extension = Path.GetExtension( inPath ).ToLower();
      switch ( extension )
      {
        case ".tpl":
          return ConvertTplFile( data );
        case ".pct":
          return ConvertPctFile( data );
        default:
          return ConversionResult.Failure( $"Unsupported file type: {extension}" );
      }
    }

    private ConversionResult ConvertTplFile( Stream data )
    {
      try
      {
        var tpl = S3D_Template.Open( data );
        var convertedStream = TplToFbxConverter.Convert( tpl.Stream );
        return ConversionResult.Success( convertedStream );
      }
      catch ( Aspose.ThreeD.ExportException exportEx )
      {
        var message = "The FBX library we're currently using is a trial and is limited to 50 exports.\n" +
                      "  Re-run the program. It will pick up where it left off.";
        return ConversionResult.Abort( message );
      }
      catch ( Exception ex )
      {
        return ConversionResult.Failure( ex.Message );
      }
    }

    private ConversionResult ConvertPctFile( Stream data )
    {
      var pct = new Pct( data );
      var convertedStream = Texture.ConvertToStream( pct );
      return ConversionResult.Success( convertedStream );
    }

    private void WriteStreamToFile( Stream stream, string outPath )
    {
      var pathDir = Path.GetDirectoryName( outPath );
      if ( !Directory.Exists( pathDir ) )
        Directory.CreateDirectory( pathDir );

      using ( var outStream = File.Create( outPath ) )
      {
        stream.Seek( 0, SeekOrigin.Begin );
        stream.CopyTo( outStream );
        outStream.Flush();
      }
    }

  }

  public class ConversionResult
  {

    public bool ShouldAbort { get; }
    public bool Successful { get; }
    public string Message { get; }
    public Stream ConvertedStream { get; set; }

    public ConversionResult( bool successful, string message = null, Stream convertedStream = null, bool abort = false )
    {
      Successful = successful;
      Message = message;
      ConvertedStream = convertedStream;
      ShouldAbort = abort;
    }

    public static ConversionResult Success( Stream convertedStream )
      => new ConversionResult( true, convertedStream: convertedStream );

    public static ConversionResult Failure( string message )
      => new ConversionResult( false, message: message );

    public static ConversionResult Abort( string message )
      => new ConversionResult( false, message, abort: true );

  }

}

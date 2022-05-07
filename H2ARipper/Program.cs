using CliWrap;
using H2ARipper;
using H2ARipper.Converters;

const string IN_PATH = @"G:\Steam\steamapps\common\Halo The Master Chief Collection\halo2\";
const string OUT_PATH = @"G:\h2a\d\";

//DecompressAll();
//await ExtractAll();
//ConvertAllTextures();
//ReadTpl( @"G:\h2a\d\shared\_database_\ss_prop__h.tpl" );
ReadTpl( @"G:\h2a\d\shared\_database_\masterchief_fp__h.tpl" );
ReadTpl( @"G:\h2a\d\shared\_database_\banshee__h.tpl" );
ReadTpl( @"G:\h2a\d\01b_spacestation\_scene_\tpl\sm_geom_00008.tpl" );

void DecompressAll()
{
  var pckFiles = Directory.GetFiles( IN_PATH, "*.pck", SearchOption.AllDirectories );
  foreach ( var pckFile in pckFiles )
  {
    var outPath = Path.Combine( OUT_PATH, $"{Path.GetFileNameWithoutExtension( pckFile )}.pck" );
    Decompress( pckFile, outPath );
  }
}

void Decompress( string inPath, string outPath )
{
  using ( var us = File.Create( outPath ) )
  {
    PckDecompresser.DecompressPck( File.OpenRead( inPath ), us );
    us.Flush();
  }
}

async Task ExtractAll()
{
  var pckFiles = Directory.GetFiles( OUT_PATH, "*.pck", SearchOption.AllDirectories );
  foreach ( var pckFile in pckFiles )
    await Extract( pckFile );
}

async Task Extract( string pckFile )
{
  bool isShared = pckFile.Contains( "shared.pck" );
  var args = pckFile;
  if ( isShared )
    args += $" -s";

  await Cli.Wrap( @".\Binaries\unpck.exe" )
    .WithArguments( args )
    .WithWorkingDirectory( Path.GetDirectoryName( pckFile ) )
    .ExecuteAsync();
}

void ConvertAllTextures()
{
  var pctFiles = Directory.EnumerateFiles( OUT_PATH, "*.pct", SearchOption.AllDirectories );

  Parallel.ForEach( pctFiles,
    new ParallelOptions { MaxDegreeOfParallelism = 30 },
    pctFile =>
  {
    try
    {
      PctToDdsConverter.Convert( pctFile );
    }
    catch ( Exception ex )
    {
      Console.WriteLine( "{0} - {1}", ex.Message, pctFile );
    }
  } );
}

void ReadTpl( string path )
{
  var tpl = TplFile.Open( path );
}

void ReadAllTpls()
{
  foreach ( var tplFile in Directory.EnumerateFiles( OUT_PATH, "*.tpl", SearchOption.AllDirectories ) )
  {
    try
    {
      ReadTpl( tplFile );
    }
    catch ( Exception ex )
    {
      Console.WriteLine( "Failed to read {0}", tplFile );
    }
  }
}
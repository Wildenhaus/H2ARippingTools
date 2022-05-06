// Research done by Zatarita
// https://opencarnage.net/index.php?/topic/8385-textures-s3dpak-format-spec/

using CliWrap;
using DirectXTexNet;

namespace H2ARipper.Converters
{

  public static class PctToDdsConverter
  {

    public static void Convert( string inFile )
    {
      using var fs = File.OpenRead( inFile );
      using var reader = new BinaryReader( fs );

      var header = ReadHeader( reader );
      ConvertToDds( header, inFile );
    }

    private static PctHeader ReadHeader( BinaryReader reader )
    {
      const short SENTINEL_SIGNATURE = 0xF0;
      const short SENTINEL_DIMENSIONS = 0x0102;
      const short SENTINEL_FORMAT = 0xF2;
      const short SENTINEL_MIP_COUNT = 0xF9;
      const short SENTINEL_PIXEL_DATA = 0xFF;
      const int PCT_HEADER = 0x50494354; // TCIP


      var header = new PctHeader();

      short sentinel;
      int endOfBlock;

      // Signature
      sentinel = reader.ReadInt16();
      endOfBlock = reader.ReadInt32();
      Assert( sentinel, SENTINEL_SIGNATURE, "Invalid Signature Sentinel" );
      Assert( reader.ReadInt32(), PCT_HEADER, "Not a PCT file" );
      Assert( reader.BaseStream.Position, endOfBlock, "Not all Signature block data read!" );

      // Dimensions
      sentinel = reader.ReadInt16();
      endOfBlock = reader.ReadInt32();
      Assert( sentinel, SENTINEL_DIMENSIONS, "Invalid Dimensions Sentinel" );

      header.Width = reader.ReadInt32();
      header.Height = reader.ReadInt32();
      header.Depth = reader.ReadInt32();
      header.Faces = reader.ReadInt32();

      Assert( reader.BaseStream.Position, endOfBlock, "Not all Dimensions block data read!" );

      // Format
      sentinel = reader.ReadInt16();
      endOfBlock = reader.ReadInt32();
      Assert( sentinel, SENTINEL_FORMAT, "Invalid Format Sentinel" );

      header.Format = ( TextureFormat ) reader.ReadInt32();
      AssertIsValidTextureFormat( header.Format );

      Assert( reader.BaseStream.Position, endOfBlock, "Not all Format block data read!" );

      // Mipmap Count
      sentinel = reader.ReadInt16();
      endOfBlock = reader.ReadInt32();
      Assert( sentinel, SENTINEL_MIP_COUNT, "Invalid MIP Sentinel" );

      header.MipCount = reader.ReadInt32();

      Assert( reader.BaseStream.Position, endOfBlock, "Not all MIP block data read!" );

      // Pixel Data
      sentinel = reader.ReadInt16();
      endOfBlock = reader.ReadInt32();
      Assert( sentinel, SENTINEL_PIXEL_DATA, "Invalid Pixel Data Sentinel" );

      header.PixelDataOffset = reader.BaseStream.Position;
      header.PixelDataSize = endOfBlock - reader.BaseStream.Position;

      return header;
    }

    private static void ConvertToDds( PctHeader header, string inFile )
    {
      if ( File.Exists( Path.ChangeExtension( inFile, "png" ) ) )
        return;

      var format = GetDxgiFormat( header.Format );
      Console.WriteLine( "{0}>{1} - {2} - {3}", header.Format.ToString(), format.ToString(), header.Width, inFile );

      var task = Cli.Wrap( @".\Binaries\RawtexCmd.exe" )
        .WithWorkingDirectory( @".\Binaries\" )
        .WithArguments( x => x
          .Add( inFile )
          .Add( ( int ) GetDxgiFormat( header.Format ) )
          .Add( $"{header.PixelDataOffset:X}" )
          .Add( header.Width )
          .Add( header.Height )
          )
        .ExecuteAsync();

      task.Task.Wait();
    }

    private static void Assert<T>( T actual, T expected, string message = null )
      where T : unmanaged
    {
      if ( !Equals( actual, expected ) )
        throw new Exception( message ?? "Invalid data detected." );

    }

    private static void AssertIsValidTextureFormat( TextureFormat format )
      => Assert( TextureFormatValues.Contains( format ), true, $"Invalid texture format! ({( int ) format:X})" );

    internal class PctHeader
    {
      // Dimensions
      public int Width;
      public int Height;
      public int Depth;
      public int Faces;

      // Format
      public TextureFormat Format;

      // Mipmap Count
      public int MipCount;

      // Pixel Data
      public long PixelDataOffset;
      public long PixelDataSize;
    }

    static TextureFormat[] TextureFormatValues = Enum.GetValues<TextureFormat>();

    internal enum TextureFormat
    {
      A8R8G8B8 = 0x00,
      A8L8 = 0x0A,

      OXT1 = 0x0C,
      AXT1 = 0x0D,
      DXT3 = 0x0F,
      DXT5 = 0x11,

      X8R8G8B8 = 0x16,

      DXN = 0x24,
      DXT5A = 0x25, // AKA ATI1

      A16R16G16B16_F = 0x26, // Report to Zatarita, shared\_textures_\shore_03a_oldmombasa.pct
      R9G9B9E6_SHAREDEXP = 0x2D // Report to Zatarita, shared\_project_\prebuild\scenes\01a_tutorial.scn\lm\01a_tutorial_tex_rnm0.pct
    }

    private static DXGI_FORMAT GetDxgiFormat( TextureFormat format )
    {
      switch ( format )
      {
        case TextureFormat.A8R8G8B8:
          return DXGI_FORMAT.B8G8R8A8_UNORM;
        case TextureFormat.A8L8:
          return DXGI_FORMAT.R8G8_UNORM; // https://searchcode.com/file/71892491/Source/Toolkit/SharpDX.Toolkit.Graphics/DDSHelper.cs/

        case TextureFormat.OXT1:
          return DXGI_FORMAT.BC1_UNORM;
        case TextureFormat.AXT1:
          return DXGI_FORMAT.BC1_UNORM;
        case TextureFormat.DXT3:
          return DXGI_FORMAT.BC2_UNORM;
        case TextureFormat.DXT5:
          return DXGI_FORMAT.BC3_UNORM;

        case TextureFormat.X8R8G8B8:
          return DXGI_FORMAT.B8G8R8X8_UNORM;

        case TextureFormat.DXN:
          return DXGI_FORMAT.BC5_UNORM;
        case TextureFormat.DXT5A:
          return DXGI_FORMAT.BC4_UNORM;

        case TextureFormat.A16R16G16B16_F:
          return DXGI_FORMAT.R16G16B16A16_FLOAT;
        case TextureFormat.R9G9B9E6_SHAREDEXP:
          return DXGI_FORMAT.R9G9B9E5_SHAREDEXP;

        default:
          throw new Exception( $"Could not convert DXGI Format {format}" );

      }
    }

  }

}

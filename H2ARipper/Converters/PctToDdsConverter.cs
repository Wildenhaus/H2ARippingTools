// Research done by Zatarita
// https://opencarnage.net/index.php?/topic/8385-textures-s3dpak-format-spec/

/*using CliWrap;
using DirectXTexNet;
using System.IO;*/

using DirectXTexNet;

using Saber3D.FileTypes;
using System.Drawing.Dds;
using System.IO;

namespace H2ARipper.Converters
{
    public static class Texture
    {

        // This uses System.Drawing.Dds library created by GraveMind2401 https://github.com/Gravemind2401/System.Drawing.Dds
        public static Stream ConvertToStream( in Pct pct )
        {
            var image = GetImage( pct );

            var ret = new MemoryStream();
            image.WriteToStream(ret);

            return ret;
        }
    
    
        public static void PctToDDS(in Pct pct, in string outpath)
        {
            var image = GetImage(pct);
            WriteDDS(image, outpath);
        }

        private static DdsImage GetImage(in Pct pct)
        {
            DdsImage image;
            switch(pct.DdsFormat)
            {
                // non-dx 10
                case Pct.Format.DXN:
                case Pct.Format.A16B16G16R16_F:
                case Pct.Format.DXT5A:
                case Pct.Format.DXT5:
                case Pct.Format.DXT3:
                case Pct.Format.OXT1:
                case Pct.Format.AXT1:
                    image = GenerateDDS(pct);
                    break;
                // dx-10
                case Pct.Format.R9G9B9E5_SHAREDEXP:
                case Pct.Format.A8R8G8B8:
                case Pct.Format.X8R8G8B8:
                    image = GenerateDDSDX10(pct);
                    break;
                default:
                    throw new ArgumentException();
            }

            return image;
        }

        private static DdsImage GenerateDDS( in Pct pct)
        {              
            if( GetFourCC( pct ) is var fourCC && fourCC.HasValue)
                return new DdsImage(pct.Width, pct.Height, fourCC.Value, pct.Pixels);
            throw new ArgumentException();
        }

        private static DdsImage GenerateDDSDX10( in Pct pct)
        {
            if( GetDxgiFormat( pct ) is var format && format.HasValue)
                return new DdsImage(pct.Width, pct.Height, format.Value, pct.Pixels);
            throw new ArgumentException();
        }

        private static DxgiFormat? GetDxgiFormat( in Pct pct )
        {
            switch(pct.DdsFormat)
            {
                case Pct.Format.R9G9B9E5_SHAREDEXP:
                    return DxgiFormat.R9G9B9E5_SharedExp;
                case Pct.Format.A8R8G8B8:
                    return DxgiFormat.B8G8R8A8_UNorm;
                case Pct.Format.X8R8G8B8:
                    return DxgiFormat.B8G8R8X8_UNorm;
            }
            return null;
        }

        private static FourCC? GetFourCC(in Pct pct)
        { 
            switch(pct.DdsFormat)
            {
                case Pct.Format.DXN:
                    return FourCC.ATI2;

                case Pct.Format.A16B16G16R16_F:
                    return null;  // Special case

                case Pct.Format.DXT5A:
                    return FourCC.ATI1;

                case Pct.Format.DXT5:
                    return FourCC.DXT5;

                case Pct.Format.DXT3:
                    return FourCC.DXT3;

                case Pct.Format.OXT1:
                case Pct.Format.AXT1:
                    return FourCC.DXT1;

                case Pct.Format.R9G9B9E5_SHAREDEXP:
                case Pct.Format.A8R8G8B8:
                case Pct.Format.X8R8G8B8:
                    return FourCC.DX10;
            }
            return null;
        }

        private static void WriteDDS(in DdsImage image, in string outpath)
        {
            var stream = File.OpenWrite(outpath);
            image.WriteToStream(stream);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saber3D.FileTypes
{
    public class Pct
    {
        public enum Sentinels : short
        {
            FOOTER     = 0x01,
            HEADER     = 0xf0,
            FORMAT     = 0xf2,
            MIPMAPS    = 0xf9,
            PIXELS     = 0xff,
            DIMENSIONS = 0x0102,
        }

        public enum Format : int
        {
            A8R8G8B8           = 0x00,
            AI88               = 0x0A,
            OXT1               = 0x0C,
            AXT1               = 0x0D,
            DXT3               = 0x0F,
            DXT5               = 0x11,
            X8R8G8B8           = 0x16,
            DXN                = 0x24,
            DXT5A              = 0x25,
            A16B16G16R16_F     = 0x26,
            R9G9B9E5_SHAREDEXP = 0x2D
        }

        public int    Width     { get; private set; }
        public int    Height    { get; private set; }
        public int    Depth     { get; private set; }  
        public int    Faces     { get; private set; }
        public int    MipMaps   { get; private set; }
        public byte[] Pixels    { get; private set; }
        public Format DdsFormat { get; private set; }

        public Pct(in Stream stream)
        {
            Pixels = new byte[0];

            Parse(stream);
        }

        private void Parse(in Stream stream)
        {
            var Parser = new BinaryReader(stream);

            while(Parser.BaseStream.Position != Parser.BaseStream.Length)
            {
                var (sentinel, size) = ParseSentinel(Parser);
                switch(sentinel)
                {
                case (short) Sentinels.HEADER:
                    if(!ReadHeader(Parser)) 
                        throw new DataMisalignedException();
                    continue;
                case (short) Sentinels.DIMENSIONS:
                    ReadDimensions(Parser);
                    continue;
                case (short) Sentinels.FORMAT:
                    ReadFormat(Parser);
                    continue;
                case (short) Sentinels.MIPMAPS:
                    ReadMips(Parser);
                    continue;
                case (short) Sentinels.PIXELS:
                    ReadPixels(Parser, size);
                    continue;
                case (short) Sentinels.FOOTER:
                    ReadFooter(Parser);
                    continue;
                default:
                    throw new ArgumentException();
                }
            }
        }

        private static (short, int) ParseSentinel(in BinaryReader stream)
        {
            short sent  = stream.ReadInt16();
            int   eob   = stream.ReadInt32();

            //byte[] data = stream.ReadBytes( (int)(eob - stream.BaseStream.Position) );

            return (sent, (int)(eob - stream.BaseStream.Position));
        }

        private bool ReadHeader(in BinaryReader stream)
        {
            if(new string(stream.ReadChars(4)) is var TCIP && TCIP == "TCIP")
                return true;
            else
                return false;
        }

        private void ReadDimensions(in BinaryReader stream)
        {
            Height = stream.ReadInt32();
            Width  = stream.ReadInt32();
            Depth  = stream.ReadInt32();
            Faces  = stream.ReadInt32();
        }

        private void ReadFormat(in BinaryReader stream)
        {
            DdsFormat = (Format) stream.ReadInt32();
        }
        
        private void ReadMips(in BinaryReader stream)
        {
            MipMaps = stream.ReadInt32();
        }

        private void ReadPixels(in BinaryReader stream, int size)
        {
            Pixels = stream.ReadBytes(size);
        }

        private void ReadFooter(in BinaryReader stream)
        {
            // There actually isnt anything in the footer c:
        }


    }
}

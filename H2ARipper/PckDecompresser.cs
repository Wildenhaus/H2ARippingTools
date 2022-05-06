// Research by Zatarita
// https://opencarnage.net/index.php?/topic/8064-the-s3dpak-format-rewrite/
// https://opencarnage.net/index.php?/topic/8181-mcc-compression-formats-pc/
// Pretty much translated (stole) this code from their python script. All credit goes to them.
// Their code: https://opencarnage.net/index.php?/topic/8160-quicript-11-context-menu-decompressor/

using Ionic.Zlib;

namespace H2ARipper
{

  public class PckDecompresser
  {

    public static void DecompressPck( Stream inStream, Stream outStream )
    {
      var reader = new BinaryReader( inStream );

      var count = reader.ReadInt32();
      var flags = reader.ReadInt32();

      var offsets = new List<long>();
      for ( var i = 0; i < count; i++ )
        offsets.Add( reader.ReadInt64() );
      offsets.Add( reader.BaseStream.Length );

      //Write each offset
      for ( var i = 0; i < offsets.Count - 1; i++ )
      {
        var offset = offsets[ i ];
        var size = offsets[ i + 1 ] - offsets[ i ];
        var buf = new byte[ size ];

        reader.BaseStream.Seek( offset, SeekOrigin.Begin );
        reader.Read( buf, 0, ( int ) size );

        if ( flags > 0 )
        {
          outStream.Write( buf );
        }
        else
        {
          var decompressed = ZlibStream.UncompressBuffer( buf );
          outStream.Write( decompressed );
        }
      }
    }

  }

}

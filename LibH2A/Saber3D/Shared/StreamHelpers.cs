// Creation Zatarita: 05/14

namespace LibH2A.Saber3D.Shared
{
  public static class StreamHelpers
  {
    // Helper function to create a byte array of appropriate size, and get data from offset
    public static byte[] ReadBytesFromStream(in Stream stream, in int size, in long? offset = null)
    {
      if(offset.HasValue)
        stream.Seek(offset.Value, SeekOrigin.Begin);

      byte[] data = new byte[ size ];
      stream.Read( data, 0, data.Length );

      return data;
    }

    // Verifies if the stream is alligned.
    // If pedantic errors are enabled this will cause an exception on failure.
    public static bool StreamIsAlligned(in Stream stream)
    {
      if ( stream.ReadByte() is int alignmentBit && alignmentBit == 1 )
        return true;

      if(Settings.PedanticErrors)
        throw new DataMisalignedException( "Stream failed alignment check!" );

      // This wont always be unreachable.
      return false;
    }
  }
}

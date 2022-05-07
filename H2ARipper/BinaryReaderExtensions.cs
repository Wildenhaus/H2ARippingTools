using System.Text;

namespace H2ARipper
{

  public static class BinaryReaderExtensions
  {

    public static void Seek( this BinaryReader reader, long count, SeekOrigin origin = SeekOrigin.Begin )
      => reader.BaseStream.Seek( count, origin );

    public static string ReadFixedLengthString( this BinaryReader reader, int length )
    {
      if ( length <= 0 )
        return null;

      var sb = new StringBuilder();

      for ( var i = 0; i < length; i++ )
        sb.Append( ( char ) reader.ReadByte() );

      return sb.ToString();
    }

    public static int PeekInt32( this BinaryReader reader )
    {
      var startPos = reader.BaseStream.Position;
      var value = reader.ReadInt32();
      reader.BaseStream.Position = startPos;

      return value;
    }

  }

}

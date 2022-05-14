// Creation Zatarita: 05/14

namespace LibH2A.Saber3D.Shared
{

  public interface ISentinelParser
  {
    public short ID { get; }
    public void Load( in Stream stream );
  }

  public class Sentinel
  {
    public int   Size{ get { return EOB - SOB; } }
    public int   SOB { get; private set; }        // Start of block (not REALLY needed, but useful for size calculation)
    public short ID  { get; protected set; }      // Sentinel Identifier
    public int   EOB { get; protected set; }      // End of block

    public Sentinel(in short id, in int eob, in int sob)
    {
      ID = id;
      EOB = eob;
      SOB = sob;
    }
    
    // Ignores the data in the sentinel
    public void Ignore( in Stream stream)
    {
      stream.Seek(EOB, SeekOrigin.Begin);
    }

    // Parses a sentinel and returns the entire block of data it represents
    public static (Sentinel, byte[]) Get( in Stream stream )
    {
      Sentinel sent = FromStream( stream );
      byte[]   data = StreamHelpers.ReadBytesFromStream( stream, sent.Size );
      return   (sent, data);    // sorry I like tuples c:
    }

    // Seek to start of block read to end, return data
    public byte[] DataFromStream( in Stream stream )
    {
      return StreamHelpers.ReadBytesFromStream( stream, Size, SOB );
    }

    // Reads a Sentinel object from stream
    public static Sentinel FromStream( in Stream basestream )
    {
      var stream = new BinaryReader( basestream );

      return FromStream( stream );
    }

    // Second overload for binary reader
    public static Sentinel FromStream( in BinaryReader stream )
    {
      var id = stream.ReadInt16();
      var eob = stream.ReadInt32();
      var sob = ( int ) stream.BaseStream.Position;

      return new Sentinel( id, eob, sob );
    }

    public static void ReadInto(in Stream stream, ref ISentinelParser data)
    {
      Sentinel sent = FromStream( stream );

      if ( sent.ID != data.ID && Settings.PedanticErrors)
        throw new TypeLoadException("Unable to load into sentinel. ID Mismatch");

      data.Load( stream );
    }
  }
}

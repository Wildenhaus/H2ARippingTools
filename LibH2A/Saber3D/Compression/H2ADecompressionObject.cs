using System.IO.Compression;        // for ZlibStream

namespace Saber3D.Compression
{

  public class H2ADecompressionStream : Stream
  {
    // Required for Stream --------------------------------------------------------
    public override bool CanRead { get { return true; } }
    public override bool CanSeek { get { return true; } }
    public override bool CanWrite { get { return false; } }
    public override long Length { get { return CHUNK_SIZE * Offsets.Count; } }
    public override long Position { get; set; }
    // ----------------------------------------------------------------------------


    protected BinaryReader Parser;
    protected FileStream FileHandle;
    protected List<long> Offsets = new List<long>();

    private byte[] _cache = { };
    private List<bool> _alreadyDecompressed = new List<bool>();
    private bool _isCompressed = true;

    private const byte FLAG_POSITION = 0x4;
    private const byte OFFSET_POSITION = 0x8;
    private const int HEADER_SIZE = 0x600000;
    private const int CHUNK_SIZE = 0x8000;
    private const int MAX_CHUNKS_CNT = ( HEADER_SIZE - sizeof( long ) ) / sizeof( long );         // 0xBFFFF = (Header size - (count & compressed flag)) / sizeof Offsets

    // Load from file
    public H2ADecompressionStream( in string file )
    {
      // Initialize the file/reader
      FileHandle = LoadPath( file );
      Parser = new BinaryReader( FileHandle );

      // Read the header & get the offsets.
      Initialize();
    }


    // Stream Object Overrides -----------------------------------------------------------------------------------------------------------------
    // Reading from the stream requires a few steps.
    //     1.  We want to ensure that the requested data is in the bounds of the file
    //     2.  See if the chunks have already been decompressed. (or if the file is even compressed in the first place)
    //     3.  If the file is compressed, and the requested data resides in chunks not yet decompressed, decompress the range of chunks needed.
    //     4a. If file compressed, Return the section containing the data from the decompressed buffer
    //     4b. if the file isn't compressed, just return the data read straight from stream; adjusted for size of header
    public override int Read( Byte[] buffer, int offset, int size )
    {
      if ( size == 0 )
        return -1;

      // Bounds checking and decompressing the chunks if they haven't been already
      DecompressRange( ( int ) Position, ( int ) Position + size );

      if ( _isCompressed )  // If it was compressed we want to copy the freshly decompressed data from the cache
        Array.Copy( _cache, ( int ) Position, buffer, 0, buffer.Length );
      else                // Else no decompression needed. Just read from the file adjust for header
      {
        Parser.BaseStream.Seek( Position, SeekOrigin.Begin );
        // Read and validate the amount of read bytes
        if ( Parser.BaseStream.Read( buffer, 0, size ) is int readData && readData != size )
          throw new OverflowException();          // Pedantic error. We should NEVER recover partial data.
      }

      Position += size;
      return size;      // Will always be CHUNK_SIZE; however, to abide by standard
    }

    public override long Seek( long offset, SeekOrigin origin )
    {
      // Bounds checking
      if ( offset > ( _isCompressed ? _cache.Length : Parser.BaseStream.Length ) )
        throw new OverflowException();

      switch ( origin )
      {
        case SeekOrigin.Begin:
          Position = _isCompressed ? offset : offset + Offsets[ 0 ];
          return Position;
        case SeekOrigin.Current:
          Position += _isCompressed ? offset : offset + Offsets[ 0 ];
          return Position;
        case SeekOrigin.End:
          Position += _isCompressed ? Length - offset : Length - offset + Offsets[ 0 ];
          return Position;
      }

      return 0;
    }

    public override void Write( Byte[] buffer, int offset, int size )
    {
      return;     // not implimented
    }

    public override void SetLength( long value )
    {
      return;     // This isn't possible
    }

    public override void Flush()
    {
      return;     // no buffer to flush
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------

    // I don't know c# well enough to thread this; however it should be possible.
    // Requires shared memory for _cache; however, there wont be a race condition as the same
    // memory won't be written to at the same time. it shouldn't be too tough.
    private void DecompressRange( long startOffset, long endOffset )
    {
      // Bounds checking
      // Note Bounds checking is strict here. I can set soft boundaries where if start of bounds is in file
      // and end is out of file I read remaining data to EOF; however, all sizes and offsets are known and must
      // be valid. In the attempt to avoid potentially garbage data (or if someone opens a non-H2A Compressed file)
      // I'm going to be pedantic about bounds checking
      if ( _isCompressed )
      {
        if ( startOffset > Length || endOffset > Length )
          throw new OverflowException();
      }
      else
        return;

      // Find which chunk indices the requested data should be in, and decompress them if they haven't been already.
      var (indexStart, indexEnd) = GetIndicesFromRange( startOffset, endOffset );
      for ( int i = ( int ) indexStart; i < indexEnd; i++ )
      {
        if ( _alreadyDecompressed[ i ] || !_isCompressed )
          continue;

        // Decompressed chunks get written straight to _cache.
        DecompressChunk( i );

        // Set it so we don't re-decompress in the future.
        _alreadyDecompressed[ i ] = true;
      }
    }

    private void DecompressChunk( int index )
    {
      // -1 because of the extra "filesize offset" 
      if ( index > Offsets.Count - 1 )
        throw new OverflowException();

      // Calculate the chunksize
      var chunkSize = Offsets[ index + 1 ] - Offsets[ index ];

      // Seek to the chunk boundary & (Using the chunksize) read the bytes
      Parser.BaseStream.Seek( Offsets[ index ], SeekOrigin.Begin );
      var chunk = Parser.ReadBytes( ( int ) chunkSize );               // Potential Downcast

      // Use the data to decompress the chunk
      // note this can throw exceptions; I don't plan to catch them at all in this scope.
      using ( var chunkStream = new MemoryStream( chunk ) )
      {
        // Create zlib object, and a view into the right section of our cache.
        ZLibStream zobj = new ZLibStream( chunkStream, CompressionMode.Decompress, false );
        Span<byte> cacheSpan = new Span<byte>( _cache, index * CHUNK_SIZE, CHUNK_SIZE );

        // Decompress the zlib chunk straight into the cache
        zobj.Read( cacheSpan );
      }
    }

    // Convertes offset range into index range
    private (long, long) GetIndicesFromRange( in long start, in long end )
    {
      return (start / CHUNK_SIZE, ( int ) Math.Ceiling( ( double ) end / CHUNK_SIZE ));
    }

    // Validate the path, and initialize the FileStream
    private FileStream LoadPath( in string file )
    {
      // Verify path exists
      if ( !System.IO.File.Exists( file ) )
        throw new DirectoryNotFoundException();

      // Can throw exceptions
      return System.IO.File.Open( file, FileMode.Open );
    }

    protected void AllocateMemory( in int count )
    {
      if ( count > 0 )
        _cache = new byte[ count * CHUNK_SIZE ];
      _alreadyDecompressed = new List<bool>( new bool[ count ] );
    }

    private int GetCount()
    {
      // We can't assume we're at the beginning of the file.
      Parser.BaseStream.Seek( 0, SeekOrigin.Begin );

      // Return the count
      return Parser.ReadInt32();
    }

    private void GetCompressedFlag()
    {
      // Seek to position
      Parser.BaseStream.Seek( FLAG_POSITION, SeekOrigin.Begin );

      // Actually read the flag
      switch ( Parser.ReadInt32() )
      {
        case 0:
          _isCompressed = true;
          break;
        case 4:
          _isCompressed = false;
          break;
        default:    // Pedantic error, shouldn't be possible unless garbage data.
          throw new InvalidDataException();
      }
    }

    private void Initialize()
    {
      ValidateStream();

      // Retrieve chunk count & validate it
      int count = GetCount();
      if ( count > MAX_CHUNKS_CNT )
        throw new OverflowException();

      // Load the isCompressed flag from stream
      GetCompressedFlag();

      // We SHOULD be at the right position in stream to read after everything; however, just in case.
      Parser.BaseStream.Seek( OFFSET_POSITION, SeekOrigin.Begin );

      // If we assign the size now we can avoid reallocating & moving around memory
      Offsets = new List<long>( new long[ count + 1 ] );

      // Iterate over all the offsets & add filesize for last chunk -> eof calculation
      for ( int i = 0; i < count; i++ )
        Offsets[ i ] = Parser.ReadInt64();
      Offsets[ count ] = FileHandle.Length;

      if ( _isCompressed )
        AllocateMemory( count );
    }

    private void ValidateStream()
    {
      if ( !Parser.BaseStream.CanRead )
        throw new InvalidOperationException();
      if ( !Parser.BaseStream.CanSeek )
        throw new InvalidOperationException();
    }
  }

}
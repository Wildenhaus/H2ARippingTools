using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibH2A.Common
{

  [DebuggerDisplay( "BinaryReader({Position})" )]
  public unsafe sealed class EndianBinaryReader : BinaryReader
  {

    #region Data Members

    private Endianness _endianness;

    #endregion

    #region Properties

    public Endianness Endianness
    {
      get => _endianness;
      set => _endianness = value;
    }

    public long Position
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => BaseStream.Position;
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      set => BaseStream.Position = value;
    }

    private bool NeedsByteOrderSwap
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => BitConverter.IsLittleEndian ^ _endianness == Endianness.LittleEndian;
    }

    #endregion

    #region Constructor

    public EndianBinaryReader( Stream input, Endianness endianness = Endianness.LittleEndian, bool leaveOpen = true )
      : this( input, endianness, Encoding.UTF8, leaveOpen )
    {
    }

    public EndianBinaryReader( Stream input, Endianness endianness, Encoding encoding, bool leaveOpen )
      : base( input, encoding, leaveOpen )
    {
      _endianness = endianness;
    }

    #endregion

    #region Public Methods

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override char ReadChar()
    {
      var alloc = stackalloc byte[ sizeof( char ) ];
      var buffer = new Span<byte>( alloc, sizeof( char ) );
      ReadAndSwap( buffer );

      return BitConverter.ToChar( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override short ReadInt16()
    {
      var alloc = stackalloc byte[ sizeof( short ) ];
      var buffer = new Span<byte>( alloc, sizeof( short ) );
      ReadAndSwap( buffer );

      return BitConverter.ToInt16( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override ushort ReadUInt16()
    {
      var alloc = stackalloc byte[ sizeof( ushort ) ];
      var buffer = new Span<byte>( alloc, sizeof( ushort ) );
      ReadAndSwap( buffer );

      return BitConverter.ToUInt16( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override int ReadInt32()
    {
      var alloc = stackalloc byte[ sizeof( int ) ];
      var buffer = new Span<byte>( alloc, sizeof( int ) );
      ReadAndSwap( buffer );

      return BitConverter.ToInt32( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override uint ReadUInt32()
    {
      var alloc = stackalloc byte[ sizeof( uint ) ];
      var buffer = new Span<byte>( alloc, sizeof( uint ) );
      ReadAndSwap( buffer );

      return BitConverter.ToUInt32( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override long ReadInt64()
    {
      var alloc = stackalloc byte[ sizeof( long ) ];
      var buffer = new Span<byte>( alloc, sizeof( long ) );
      ReadAndSwap( buffer );

      return BitConverter.ToInt64( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override ulong ReadUInt64()
    {
      var alloc = stackalloc byte[ sizeof( ulong ) ];
      var buffer = new Span<byte>( alloc, sizeof( ulong ) );
      ReadAndSwap( buffer );

      return BitConverter.ToUInt64( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override float ReadSingle()
    {
      var alloc = stackalloc byte[ sizeof( float ) ];
      var buffer = new Span<byte>( alloc, sizeof( float ) );
      ReadAndSwap( buffer );

      return BitConverter.ToSingle( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override double ReadDouble()
    {
      var alloc = stackalloc byte[ sizeof( double ) ];
      var buffer = new Span<byte>( alloc, sizeof( double ) );
      ReadAndSwap( buffer );

      return BitConverter.ToDouble( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public override decimal ReadDecimal()
      => throw new NotImplementedException();

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public string ReadString( uint length )
    {
      var sb = new StringBuilder();

      for ( var i = 0; i < length; i++ )
        sb.Append( ( char ) ReadByte() );

      return sb.ToString();
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public string ReadPascalString16()
    {
      var stringLength = ReadUInt16();
      if ( stringLength == 0 )
        return null;

      return ReadString( stringLength );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public string ReadPascalString32()
    {
      var stringLength = ReadUInt32();
      if ( stringLength == 0 )
        return null;

      return ReadString( stringLength );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public Matrix4x4 ReadMatrix4x4()
    {
      return new Matrix4x4(
        m11: ReadSingle(),
        m12: ReadSingle(),
        m13: ReadSingle(),
        m14: ReadSingle(),
        m21: ReadSingle(),
        m22: ReadSingle(),
        m23: ReadSingle(),
        m24: ReadSingle(),
        m31: ReadSingle(),
        m32: ReadSingle(),
        m33: ReadSingle(),
        m34: ReadSingle(),
        m41: ReadSingle(),
        m42: ReadSingle(),
        m43: ReadSingle(),
        m44: ReadSingle()
      );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public Vector3 ReadVector3()
    {
      return new Vector3(
        x: ReadSingle(),
        y: ReadSingle(),
        z: ReadSingle()
        );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public Guid ReadGuid()
    {
      var alloc = stackalloc byte[ sizeof( Guid ) ];
      var buffer = new Span<byte>( alloc, sizeof( Guid ) );

      ReadAndSwap( buffer );

      return new Guid( buffer );
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public void Seek( long offset, SeekOrigin origin = SeekOrigin.Begin )
      => BaseStream.Seek( offset, origin );

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte PeekByte()
    {
      var value = ReadByte();
      BaseStream.Position--;
      return value;
    }

    //[DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public short PeekInt16()
    {
      var value = ReadInt16();
      BaseStream.Position -= sizeof( short );
      return value;
    }

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public int PeekInt32()
    {
      var value = ReadInt32();
      BaseStream.Position -= sizeof( int );
      return value;
    }

    #endregion

    #region Private Methods

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ReadAndSwap( in Span<byte> buffer )
    {
      var bytesRead = base.Read( buffer );
      if ( bytesRead != buffer.Length )
        ThrowEndOfStreamException();

      if ( NeedsByteOrderSwap )
        buffer.Reverse();

      return;
    }

    #region Throw Helpers

    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.NoInlining )]
    private static void ThrowEndOfStreamException()
      => throw new EndOfStreamException();

    #endregion

    #endregion

  }

}

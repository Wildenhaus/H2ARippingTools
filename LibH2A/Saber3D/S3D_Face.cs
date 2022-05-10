using System.Runtime.InteropServices;

namespace LibH2A.Saber3D
{
  [StructLayout( LayoutKind.Explicit, Size = 0x6 )]
  public struct S3D_Face
  {

    [FieldOffset( 0x0 )]
    public short A;

    [FieldOffset( 0x2 )]
    public short B;

    [FieldOffset( 0x4 )]
    public short C;

  }
}

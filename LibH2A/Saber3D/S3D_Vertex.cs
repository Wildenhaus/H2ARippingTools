using System.Runtime.InteropServices;

namespace LibH2A.Saber3D
{

  [StructLayout( LayoutKind.Explicit, Size = 0x8 )]
  public struct S3D_Vertex
  {

    [FieldOffset( 0x0 )]
    public short X;

    [FieldOffset( 0x2 )]
    public short Y;

    [FieldOffset( 0x4 )]
    public short Z;

    [FieldOffset( 0x6 )]
    public short W;

  }

}

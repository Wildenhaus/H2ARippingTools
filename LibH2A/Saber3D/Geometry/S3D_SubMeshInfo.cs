using System.Runtime.InteropServices;

namespace LibH2A.Saber3D.Geometry
{

  [StructLayout( LayoutKind.Explicit, Size = 0x04 )]
  public struct S3D_SubMeshInfo
  {

    [FieldOffset( 0x00 )]
    public Byte Unk_01;

    [FieldOffset( 0x02 )]
    public Byte Unk_02;

    [FieldOffset( 0x03 )]
    public Byte Unk_03;

    [FieldOffset( 0x04 )]
    public Byte Unk_04;

  }

}

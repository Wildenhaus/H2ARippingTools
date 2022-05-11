using System.Runtime.InteropServices;

namespace LibH2A.Saber3D.Geometry
{

  [StructLayout( LayoutKind.Explicit, Size = 0xA )]
  public struct S3D_MeshInfo
  {

    [FieldOffset( 0x00 )]
    public UInt16 Unk_01;

    [FieldOffset( 0x02 )]
    public Byte Unk_02;

    [FieldOffset( 0x03 )]
    public Byte Unk_03;

    [FieldOffset( 0x04 )]
    public Byte Unk_04;

    [FieldOffset( 0x05 )]
    public Byte Unk_05;

    [FieldOffset( 0x06 )]
    public UInt32 Unk_06;

  }

}

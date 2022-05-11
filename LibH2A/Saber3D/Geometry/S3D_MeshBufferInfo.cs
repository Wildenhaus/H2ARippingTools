using System.Runtime.InteropServices;

namespace LibH2A.Saber3D.Geometry
{

  [StructLayout( LayoutKind.Explicit, Size = 0x8 )]
  public struct S3D_MeshBufferInfo
  {

    [FieldOffset( 0x0 )]
    public UInt64 BufferId;

    [FieldOffset( 0x4 )]
    public UInt64 SubBufferOffset;

  }

}

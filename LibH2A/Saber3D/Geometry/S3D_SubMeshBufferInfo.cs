using System.Runtime.InteropServices;

namespace LibH2A.Saber3D.Geometry
{

  [StructLayout( LayoutKind.Explicit, Size = 0x0C )]
  public struct S3D_SubMeshBufferInfo
  {

    [FieldOffset( 0x00 )]
    public UInt16 VertexOffset;

    [FieldOffset( 0x02 )]
    public UInt16 VertexCount;

    [FieldOffset( 0x04 )]
    public UInt16 FaceOffset;

    [FieldOffset( 0x06 )]
    public UInt16 FaceCount;

    [FieldOffset( 0x08 )]
    public UInt16 NodeId;

    [FieldOffset( 0x0A )]
    public UInt16 SkinCompoundId;

  }

}

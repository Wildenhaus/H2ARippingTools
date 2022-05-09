using System.Numerics;

namespace LibH2A.Saber3D
{

  public struct S3D_BoundingBox
  {
    public uint Unk_01;
    public byte Unk_02;
    public uint SubMeshIndex;
    public uint SubMeshRangeCount;
    public Vector3 Min;
    public Vector3 Max;
  }

}

using System.Diagnostics;
using System.Numerics;

namespace LibH2A.Saber3D.Geometry
{

  [DebuggerDisplay( "Node[{Index}]({Unk_15_BoneName})" )]
  public class S3D_GeometryNode
  {
    public ushort Index { get; set; }
    public string NodeName { get; set; }
    public S3D_Geometry_UnkSection03 Unk_03 { get; set; }
    public ushort Unk_04_ParentId { get; set; }
    public ushort Unk_05_SiblingId { get; set; }
    public ushort Unk_06_SiblingId { get; set; }
    public ushort Unk_07_ChildId { get; set; }
    public ushort Unk_08_BoneId { get; set; }
    public string Unk_09_ExportName { get; set; }
    public Matrix4x4 Unk_10_BoneMatrix { get; set; }
    public Matrix4x4 Unk_11_InverseMatrix { get; set; }
    // Section 12? BoundingBoxes, doesn't seem to correlate with nodecount
    public string Unk_13_BoneHeirarchy { get; set; }
    // Section 14? Mostly zero data
    public string BoneName { get; set; }
    public string Unk_16_ExportName { get; set; }

    public string Name
    {
      get
      {
        if ( !string.IsNullOrEmpty( NodeName ) )
          return NodeName;

        return BoneName;
      }
    }
  }

  public struct S3D_Geometry_UnkSection03
  {
    public ushort Unk_01;
    public ushort Unk_02_IsMesh;
    public ushort Unk_03_IsBone;
    public byte Unk_04_IsHighestLod;
  }

}

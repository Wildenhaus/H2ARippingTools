using System.Diagnostics;
using System.Numerics;

namespace LibH2A.Saber3D
{

  [DebuggerDisplay( "Node[{Index}]({Name})" )]
  public class S3D_GeometryNode
  {
    public ushort Index { get; set; }
    public string Name { get; set; }
    public S3D_Geometry_UnkSection03 Unk_03 { get; set; }
    public ushort Unk_04 { get; set; }
    public ushort Unk_05 { get; set; }
    public ushort Unk_06 { get; set; }
    public ushort Unk_07 { get; set; }
    public ushort Unk_08 { get; set; }
    public string Unk_09 { get; set; }
    public Matrix4x4 Unk_10 { get; set; }
    public Matrix4x4 Unk_11 { get; set; }
    // Section 12?
    public string Unk_13 { get; set; }
    // Section 14?
    public string Unk_15a { get; set; }
    public string Unk_16 { get; set; }
  }

  public struct S3D_Geometry_UnkSection03
  {
    public uint Unk_01;
    public ushort Unk_02;
    public byte Unk_03;
  }

}

using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public struct Unk_MaterialProp_06
  {
    public uint Unk_01;
    public float Unk_02;

    public static Unk_MaterialProp_06 Read( EndianBinaryReader reader )
    {
      return new Unk_MaterialProp_06
      {
        Unk_01 = reader.ReadUInt32(),
        Unk_02 = reader.ReadSingle(),
      };
    }
  }

}

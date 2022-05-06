// WORK IN PROGRESS
// Currently basing this off of the MaxScript provided by sleepyzay

namespace H2ARipper.Converters
{

  public static class TplToDaeConverter
  {

    public static void Convert( string inPath, string outPath )
    {
      using var fs = File.OpenRead( inPath );
      using var reader = new BinaryReader( fs );


    }

    #region Embedded Types

    internal struct BufferInfo
    {
      public int FaceBufferOffset;
      public int Unk_BufferOffset;
      public int StaticVertBufferOffset;
      public int SkinnedVertBufferOffset;
      public int NormalTextureVertBufferOffset;
      public int Unk_FaceOffset;
      public int Unk_VertexOffset30;
      public int Unk_VertexOffset70;
    }

    internal struct BufferType
    {
      public ushort Unk_1;
      public byte Unk_2;
      public byte Type;
      public byte Unk_4;
      public byte Unk_5;
      public ulong Unk_6;
    }

    internal struct MeshData
    {
      public ulong BufferId;
      public ulong SubBufferOffset;
    }

    internal struct UnkMeshTable
    {
      public ushort Unk_1;
      public byte Unk_2;
      public byte Unk_3;
      public byte Unk_4;
      public byte Unk_5;
      public ulong Unk_6;
    }

    internal struct SubMeshData
    {
      public ushort VertOffset;
      public ushort VertCount;
      public ushort FaceOffset;
      public ushort FaceCount;
      public ushort NodeId;
      public ushort SkinCompoundId;
    }

    internal struct UnkNodeTable
    {
      public ushort Unk_1;
      public ushort Unk_2;
      public ushort Unk_3;
      public byte Unk_4;
    }

    internal struct SubMeshScaleData
    {
      public short[] Pos; //X,Y,Z
      public short[] Scale; //X,Y,Z
    }

    internal struct BoundingBox
    {
      public long Unk_1;
      public byte Unk_2;
      public ulong SubMeshIndex;
      public long SubMeshRangeCount;
      public float[] BbMin; //X,Y,Z
      public float[] BbMax; //X,Y,Z
    }

    internal struct Material
    {

    }

    internal struct UnkDataType6
    {
      public long Unk_1;
      public long Unk_2;
    }

    #endregion

  }

}

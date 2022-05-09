using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_NormalMap
  {
    public float End;
    public float Falloff;
    public int IsVisible;
    public float Scale;
    public float Start;
    public string TextureName;
    public float TilingU;
    public float TilingV;
    public int UvSetIndex;

    public static S3D_NormalMap Read( EndianBinaryReader reader )
    {
      var nm = new S3D_NormalMap();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "end":
            nm.End = reader.ReadSingle();
            break;
          case "falloff":
            nm.Falloff = reader.ReadSingle();
            break;
          case "isVisible":
            nm.IsVisible = reader.ReadInt32();
            break;
          case "scale":
            nm.Scale = reader.ReadSingle();
            break;
          case "start":
            nm.Start = reader.ReadSingle();
            break;
          case "textureName":
            nm.TextureName = reader.ReadPascalString32();
            break;
          case "tilingU":
            nm.TilingU = reader.ReadSingle();
            break;
          case "tilingV":
            nm.TilingV = reader.ReadSingle();
            break;
          case "uvSetIdx":
            nm.UvSetIndex = reader.ReadInt32();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_NormalMap: {propertyName}" );
        }
      }

      return nm;
    }
  }

}

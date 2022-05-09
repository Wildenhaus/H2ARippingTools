using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Mask
  {
    public string TextureName;
    public float TilingU;
    public float TilingV;
    public int UvSetIndex;

    public static S3D_Mask Read( EndianBinaryReader reader )
    {
      var mask = new S3D_Mask();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "textureName":
            mask.TextureName = reader.ReadPascalString32();
            break;
          case "tilingU":
            mask.TilingU = reader.ReadSingle();
            break;
          case "tilingV":
            mask.TilingV = reader.ReadSingle();
            break;
          case "uvSetIdx":
            mask.UvSetIndex = reader.ReadInt32();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Mask: {propertyName}" );
        }
      }

      return mask;
    }
  }

}

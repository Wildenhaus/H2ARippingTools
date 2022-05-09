using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Material
  {
    public int Version;
    public string ShadingMaterialTexture;
    public string ShadingMaterialMaterial;
    public S3D_Material_LM LM;
    public S3D_MaterialLayer Layer0;
    public S3D_MaterialLayer Layer1;
    public S3D_MaterialLayer Layer2;
    public S3D_MaterialExtraParams ExtraParams;

    public static S3D_Material Read( EndianBinaryReader reader )
    {
      var material = new S3D_Material();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();
        switch ( propertyName )
        {
          case "version":
            material.Version = reader.ReadInt32();
            break;
          case "shadingMtl_Tex":
            material.ShadingMaterialTexture = reader.ReadPascalString32();
            break;
          case "shadingMtl_Mtl":
            material.ShadingMaterialMaterial = reader.ReadPascalString32();
            break;
          case "lm":
            material.LM = S3D_Material_LM.Read( reader );
            break;
          case "layer0":
            material.Layer0 = S3D_MaterialLayer.Read( reader );
            break;
          case "layer1":
            material.Layer1 = S3D_MaterialLayer.Read( reader );
            break;
          case "layer2":
            material.Layer2 = S3D_MaterialLayer.Read( reader );
            break;
          case "extraParams":
            material.ExtraParams = S3D_MaterialExtraParams.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Material: {propertyName}" );
        }
      }

      return material;
    }
  }

}

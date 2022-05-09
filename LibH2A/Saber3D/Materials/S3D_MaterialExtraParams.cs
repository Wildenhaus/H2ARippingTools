using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_MaterialExtraParams
  {
    public S3D_ReliefNormalMaps ReliefNormalMaps;
    public S3D_AuxiliaryTextures AuxiliaryTextures;
    public S3D_Transparency Transparency;
    public S3D_ExtraVertexColorData ExtraVertexColorData;

    public static S3D_MaterialExtraParams Read( EndianBinaryReader reader )
    {
      var extraParams = new S3D_MaterialExtraParams();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "reliefNormalmaps":
            extraParams.ReliefNormalMaps = S3D_ReliefNormalMaps.Read( reader );
            break;
          case "auxiliaryTextures":
            extraParams.AuxiliaryTextures = S3D_AuxiliaryTextures.Read( reader );
            break;
          case "transparency":
            extraParams.Transparency = S3D_Transparency.Read( reader );
            break;
          case "extraVertexColorData":
            extraParams.ExtraVertexColorData = S3D_ExtraVertexColorData.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_MaterialExtraParams: {propertyName}" );
        }
      }

      return extraParams;
    }
  }

}

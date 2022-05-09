using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Blending
  {
    public string Method;
    public bool UseLayerAlpha;
    public bool UseHeightMap;
    public float WeightMultiplier;
    public float HeightMapSoftness;
    public int TexChannelBlendMask;
    public S3D_Weights Weights;
    public S3D_HeightMap HeightMap;
    public string HeightMapOverride;
    public S3D_UpVector UpVector;
    public S3D_HeightMapUvOverride HeightMapUvOverride;

    public static S3D_Blending Read( EndianBinaryReader reader )
    {
      var blending = new S3D_Blending();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "method":
            blending.Method = reader.ReadPascalString32();
            break;
          case "useLayerAlpha":
            blending.UseLayerAlpha = reader.ReadBoolean();
            break;
          case "useHeightmap":
            blending.UseHeightMap = reader.ReadBoolean();
            break;
          case "weightMultiplier":
            blending.WeightMultiplier = reader.ReadSingle();
            break;
          case "heightmapSoftness":
            blending.HeightMapSoftness = reader.ReadSingle();
            break;
          case "texChannelBlendMask":
            blending.TexChannelBlendMask = reader.ReadInt32();
            break;
          case "weights":
            blending.Weights = S3D_Weights.Read( reader );
            break;
          case "heightmap":
            blending.HeightMap = S3D_HeightMap.Read( reader );
            break;
          case "heightmapOverride":
            blending.HeightMapOverride = reader.ReadPascalString32();
            break;
          case "upVector":
            blending.UpVector = S3D_UpVector.Read( reader );
            break;
          case "heightmapUVOverride":
            blending.HeightMapUvOverride = S3D_HeightMapUvOverride.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Blending: {propertyName}" );
        }
      }

      return blending;
    }
  }

}

using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_ReliefNormalMaps
  {
    public S3D_NormalMap Macro;
    public S3D_NormalMap Micro1;
    public S3D_NormalMap Micro2;

    public static S3D_ReliefNormalMaps Read( EndianBinaryReader reader )
    {
      var reliefNM = new S3D_ReliefNormalMaps();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "macro":
            reliefNM.Macro = S3D_NormalMap.Read( reader );
            break;
          case "micro1":
            reliefNM.Macro = S3D_NormalMap.Read( reader );
            break;
          case "micro2":
            reliefNM.Macro = S3D_NormalMap.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_ReliefNormalMaps: {propertyName}" );
        }
      }

      return reliefNM;
    }
  }

}

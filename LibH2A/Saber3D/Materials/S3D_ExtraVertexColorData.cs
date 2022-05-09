using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_ExtraVertexColorData
  {
    public S3D_Color ColorA;
    public S3D_Color ColorB;
    public S3D_Color ColorG;
    public S3D_Color ColorR;

    public static S3D_ExtraVertexColorData Read( EndianBinaryReader reader )
    {
      var extraVertCol = new S3D_ExtraVertexColorData();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "colorA":
            extraVertCol.ColorA = S3D_Color.Read( reader );
            break;
          case "colorB":
            extraVertCol.ColorB = S3D_Color.Read( reader );
            break;
          case "colorG":
            extraVertCol.ColorG = S3D_Color.Read( reader );
            break;
          case "colorR":
            extraVertCol.ColorR = S3D_Color.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_ExtraVertexColorData: {propertyName}" );
        }
      }

      return extraVertCol;
    }
  }

}

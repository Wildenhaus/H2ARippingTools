using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Transparency
  {
    public int ColorSetIndex;
    public int Enabled;
    public float Multiplier;
    public int Sources;

    public static S3D_Transparency Read( EndianBinaryReader reader )
    {
      var transparency = new S3D_Transparency();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "colorSetIdx":
            transparency.ColorSetIndex = reader.ReadInt32();
            break;
          case "enabled":
            transparency.Enabled = reader.ReadInt32();
            break;
          case "multiplier":
            transparency.Multiplier = reader.ReadSingle();
            break;
          case "sources":
            transparency.Sources = reader.ReadInt32();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Transparency: {propertyName}" );
        }
      }

      return transparency;
    }
  }

}

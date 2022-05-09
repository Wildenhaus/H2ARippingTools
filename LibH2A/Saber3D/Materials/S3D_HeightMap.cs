using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_HeightMap
  {
    public int ColorSetIndex;
    public bool Invert;

    public static S3D_HeightMap Read( EndianBinaryReader reader )
    {
      var heightmap = new S3D_HeightMap();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "colorSetIdx":
            heightmap.ColorSetIndex = reader.ReadInt32();
            break;
          case "invert":
            heightmap.Invert = reader.ReadBoolean();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_HeightMap: {propertyName}" );
        }
      }

      return heightmap;
    }
  }

}

using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Weights
  {
    public int ColorSetIndex;

    public static S3D_Weights Read( EndianBinaryReader reader )
    {
      var weights = new S3D_Weights();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "colorSetIdx":
            weights.ColorSetIndex = reader.ReadInt32();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Weights: {propertyName}" );
        }
      }

      return weights;
    }
  }

}

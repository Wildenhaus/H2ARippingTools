using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_UpVector
  {
    public float Angle;
    public bool Enabled;
    public float Falloff;

    public static S3D_UpVector Read( EndianBinaryReader reader )
    {
      var upVector = new S3D_UpVector();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "angle":
            upVector.Angle = reader.ReadSingle();
            break;
          case "enabled":
            upVector.Enabled = reader.ReadBoolean();
            break;
          case "falloff":
            upVector.Falloff = reader.ReadSingle();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_UpVector: {propertyName}" );
        }
      }

      return upVector;
    }
  }

}

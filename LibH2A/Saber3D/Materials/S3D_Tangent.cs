using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Tangent
  {
    public int UvSetIndex;

    public static S3D_Tangent Read( EndianBinaryReader reader )
    {
      var tangent = new S3D_Tangent();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "uvSetIdx":
            tangent.UvSetIndex = reader.ReadInt32();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Tangent: {propertyName}" );
        }
      }

      return tangent;
    }
  }

}

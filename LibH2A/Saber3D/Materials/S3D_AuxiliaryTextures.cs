using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_AuxiliaryTextures
  {
    public S3D_Mask Mask;

    public static S3D_AuxiliaryTextures Read( EndianBinaryReader reader )
    {
      var aux = new S3D_AuxiliaryTextures();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "mask":
            aux.Mask = S3D_Mask.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_AuxiliaryTextures: {propertyName}" );
        }
      }

      return aux;
    }
  }

}

using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_Material_LM
  {
    public string Source;
    public string TextureName;
    public int UvSetIndex;
    public S3D_Tangent Tangent;

    public static S3D_Material_LM Read( EndianBinaryReader reader )
    {
      var lm = new S3D_Material_LM();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "source":
            lm.Source = reader.ReadPascalString32();
            break;
          case "texName":
            lm.TextureName = reader.ReadPascalString32();
            break;
          case "uvSetIdx":
            lm.UvSetIndex = reader.ReadInt32();
            break;
          case "tangent":
            lm.Tangent = S3D_Tangent.Read( reader );
            break;
          default:
            throw new Exception( $"Unknown property for S3D_Material_LM: {propertyName}" );
        }
      }

      return lm;
    }
  }

}

using LibH2A.Common;

namespace LibH2A.Saber3D.Materials
{

  public class S3D_MaterialLayer
  {
    public string TextureName;
    public string MaterialName;
    public Unk_MaterialProp_06[] Tint;
    public int VcSet;
    public float TilingU;
    public float TilingV;
    public S3D_Blending Blending;
    public object UvSetIndex;

    public static S3D_MaterialLayer Read( EndianBinaryReader reader )
    {
      var layer = new S3D_MaterialLayer();

      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
      {
        var propertyName = reader.ReadPascalString32();
        var dataType = ( S3D_MaterialPropertyDataType ) reader.ReadInt32();

        switch ( propertyName )
        {
          case "texName":
            layer.TextureName = reader.ReadPascalString32();
            break;
          case "mtlName":
            layer.MaterialName = reader.ReadPascalString32();
            break;
          case "tint":
            var count = reader.ReadUInt32();
            layer.Tint = new Unk_MaterialProp_06[ count ];
            for ( var j = 0; j < count; j++ )
              layer.Tint[ j ] = Unk_MaterialProp_06.Read( reader );
            break;
          case "vcSet":
            layer.VcSet = reader.ReadInt32();
            break;
          case "tilingU":
            layer.TilingU = reader.ReadSingle();
            break;
          case "tilingV":
            layer.TilingV = reader.ReadSingle();
            break;
          case "blending":
            layer.Blending = S3D_Blending.Read( reader );
            break;
          case "uvSetIdx":
            layer.UvSetIndex = reader.ReadInt32();
            break;
          default:
            throw new Exception( $"Unknown property for S3D_MaterialLayer: {propertyName}" );
        }
      }

      return layer;
    }
  }

}

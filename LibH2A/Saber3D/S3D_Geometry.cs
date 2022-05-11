using LibH2A.Common;
using LibH2A.Saber3D.Geometry;

namespace LibH2A.Saber3D
{

  public class S3D_Geometry
  {

    #region Properties

    public S3D_GeometryNodeCollection Nodes { get; private set; }
    public S3D_GeometryData Data { get; private set; }

    #endregion

    #region Constructor

    private S3D_Geometry()
    {
    }

    public static S3D_Geometry Read( EndianBinaryReader reader )
    {
      var geometry = new S3D_Geometry();

      geometry.Nodes = S3D_GeometryNodeCollection.Read( reader );
      geometry.Data = S3D_GeometryData.Read( reader );

      return geometry;
    }

    #endregion

    #region Private Methods



    #endregion

  }

}

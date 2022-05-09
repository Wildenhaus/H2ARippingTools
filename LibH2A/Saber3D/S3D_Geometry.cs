using LibH2A.Common;

namespace LibH2A.Saber3D
{

  public class S3D_Geometry
  {

    #region Properties

    public S3D_GeometryNodeCollection Nodes { get; private set; }

    #endregion

    #region Constructor

    private S3D_Geometry()
    {
    }

    public static S3D_Geometry Read( EndianBinaryReader reader )
    {
      var geometry = new S3D_Geometry();

      geometry.Nodes = S3D_GeometryNodeCollection.Read( reader );

      return geometry;
    }

    #endregion

    #region Private Methods



    #endregion

  }

}

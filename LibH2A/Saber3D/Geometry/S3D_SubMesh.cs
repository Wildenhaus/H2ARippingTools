using System.Numerics;
using LibH2A.Saber3D.Materials;

namespace LibH2A.Saber3D.Geometry
{

  public class S3D_SubMesh
  {

    public S3D_GeometryData Parent { get; }

    public UInt32 MeshId { get; set; }
    public S3D_SubMeshInfo SubMeshInfo { get; set; }
    public S3D_SubMeshBufferInfo BufferInfo { get; set; }
    public S3D_Material Material { get; set; }
    public string MaterialString { get; set; }

    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }

    public S3D_SubMesh( S3D_GeometryData parent )
    {
      Parent = parent;
    }

  }

}

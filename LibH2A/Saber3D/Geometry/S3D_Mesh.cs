namespace LibH2A.Saber3D.Geometry
{

  public class S3D_Mesh
  {

    public S3D_GeometryData Parent { get; }

    public S3D_MeshBufferInfo[] Buffers { get; set; }
    public S3D_MeshInfo MeshInfo { get; set; }

    public S3D_Mesh( S3D_GeometryData parent )
    {
      Parent = parent;
    }

  }

}

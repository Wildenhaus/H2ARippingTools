namespace LibH2A.Saber3D.Geometry
{

  public class S3D_Mesh
  {

    public int Id { get; }
    public S3D_GeometryData Parent { get; }

    public S3D_MeshBufferInfo[] Buffers { get; set; }
    public S3D_MeshInfo MeshInfo { get; set; }

    public IEnumerable<S3D_SubMesh> SubMeshes
    {
      get => Parent.SubMeshes.Where( x => x.MeshId == Id );
    }

    public S3D_Mesh( S3D_GeometryData parent, int id )
    {
      Parent = parent;
      Id = id;
    }

  }

}

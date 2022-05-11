namespace LibH2A.Saber3D.Geometry
{

  public class S3D_GeometryBuffer
  {

    #region Properties

    public S3D_GeometryData Parent { get; }

    public Int64 StartOffset { get; set; }
    public Int64 EndOffset { get; set; }
    public UInt32 BufferLength { get; set; }
    public UInt16 ElementSize { get; set; }
    public S3D_GeometryBufferTypeInfo TypeInfo { get; set; }

    public S3D_GeometryBufferType BufferType
    {
      get => TypeInfo.BufferType;
    }

    #endregion

    #region Constructor

    public S3D_GeometryBuffer( S3D_GeometryData parent )
    {
      Parent = parent;
    }

    #endregion

  }

}

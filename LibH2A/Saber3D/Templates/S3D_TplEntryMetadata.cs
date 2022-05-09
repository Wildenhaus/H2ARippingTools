using LibH2A.Common;
using LibH2A.Saber3D.Metadata;

namespace LibH2A.Saber3D.Templates
{

  [MetadataTypeSignature( 0x006C7074 )]
  public class S3D_TplMetadata : S3D_Metadata
  {

    #region Constructor

    public static S3D_TplMetadata Read( Stream stream )
    {
      var metadata = new S3D_TplMetadata();
      var reader = new EndianBinaryReader( stream, leaveOpen: true );

      // 0x00-0x08
      metadata.CheckSignature( reader );

      return metadata;
    }

    #endregion

  }

}

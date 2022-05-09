using System.Text;
using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D.Metadata
{

  public abstract class S3D_Metadata
  {

    #region Constants

    const uint SIGNATURE_SER = 0x52455331; // 1SER

    #endregion

    #region Properties

    public Guid Guid { get; protected set; }

    #endregion

    #region Private Methods

    protected void CheckSignature( EndianBinaryReader reader )
    {
      AssertEqual( reader.ReadUInt32(), SIGNATURE_SER, "Invalid 1SER signature." );

      var expectedTypeSignature = GetTypeSignature();
      var actualType = reader.ReadUInt32();
      AssertEqual( actualType, expectedTypeSignature.Signature,
        $"Wrong metadata type! " +
        $"Expected: `{expectedTypeSignature.Type}`, " +
        $"Actual: `{Encoding.UTF8.GetString( BitConverter.GetBytes( actualType ) )}`." );
    }

    private MetadataTypeSignatureAttribute GetTypeSignature()
      => GetType().GetAttribute<MetadataTypeSignatureAttribute>();

    #endregion

  }

}

using System.Runtime.CompilerServices;
using System.Text;

namespace LibH2A.Saber3D.Metadata
{

  public class MetadataTypeSignatureAttribute : Attribute
  {

    #region Properties

    public uint Signature { get; }
    public string Type
    {
      get => GetTypeString( Signature );
    }

    #endregion

    #region Constructor

    public MetadataTypeSignatureAttribute( uint signature )
    {
      Signature = signature;
    }

    #endregion

    #region Private Methods

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static string GetTypeString( uint type )
      => Encoding.UTF8.GetString( BitConverter.GetBytes( type ) );

    #endregion

  }

}

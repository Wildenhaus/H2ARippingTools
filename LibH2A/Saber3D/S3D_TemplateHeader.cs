using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D
{

  public class S3D_TemplateHeader
  {

    #region Constants

    private const uint SIGNATURE_TPL1 = 0x314C5054; //TPL1

    #endregion

    #region Properties

    public string Name { get; private set; }

    public string ExportString { get; private set; }
    public string TypeString { get; private set; }

    #endregion

    #region Constructor

    private S3D_TemplateHeader()
    {
    }

    public static S3D_TemplateHeader Read( EndianBinaryReader reader )
    {
      var header = new S3D_TemplateHeader();

      // 0x00-0x04
      CheckSignature( reader );

      // 0x04-0x0A
      _ = reader.ReadUInt32(); // 0x04 | Unk: Count?
      _ = reader.ReadUInt16(); // 0x08 | Unk

      // 0x0A
      header.Name = reader.ReadPascalString32();

      _ = reader.ReadUInt16(); // Unk
      _ = reader.ReadUInt32(); // Unk

      // TODO: Everything after this is inconsistent
      //header.ExportString = reader.ReadPascalString32();
      //header.TypeString = reader.ReadPascalString32();

      //_ = reader.ReadUInt32(); // Unk
      //_ = reader.ReadUInt16(); // Unk

      return header;
    }

    #endregion

    #region Private Methods

    private static void CheckSignature( EndianBinaryReader reader )
    {
      AssertEqual( reader.ReadUInt32(), SIGNATURE_TPL1,
        $"Invalid TPL1 Signature (0x{reader.BaseStream.Position:X}" );
    }

    #endregion

  }

}

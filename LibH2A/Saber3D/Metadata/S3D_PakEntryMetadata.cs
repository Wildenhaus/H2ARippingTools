using LibH2A.Common;


namespace LibH2A.Saber3D.Metadata
{

  [MetadataTypeSignature( 0x006B6170 )]
  public class S3D_PakMetadata : S3D_Metadata
  {

    #region Data Members

    private List<string> _fileNames;

    #endregion

    #region Properties

    public IReadOnlyList<string> FileNames
    {
      get => _fileNames;
    }

    #endregion

    #region Constructor

    protected S3D_PakMetadata()
    {
      _fileNames = new List<string>();
    }

    public static S3D_PakMetadata Read( Stream stream )
    {
      var metadata = new S3D_PakMetadata();
      var reader = new EndianBinaryReader( stream, leaveOpen: true );

      // 0x00-0x08
      metadata.CheckSignature( reader );

      // 0x08-0x24
      _ = reader.ReadUInt64(); // 0x08 | Unk: Size?
      _ = reader.ReadUInt64(); // 0x10 | Unk: Size?
      _ = reader.ReadUInt64(); // 0x18 | Unk: Address?
      _ = reader.ReadUInt32(); // 0x20 | Unk: Count?

      // 0x24-0x34
      metadata.Guid = reader.ReadGuid();

      // 0x34-0x4E
      _ = reader.ReadUInt64(); // 0x34 | Unk
      _ = reader.ReadUInt32(); // 0x3C | Unk
      _ = reader.ReadUInt32(); // 0x40 | Unk: Count?
      _ = reader.ReadByte(); // 0x44 | Sentinel
      var nameCount = reader.ReadInt32(); // 0x45 | Number of file names
      _ = reader.ReadUInt32(); // // 0x49 | Unk: Count

      // Names

      // Remainder (0x13)
      _ = reader.ReadByte(); // Unk: Sentinel?
      _ = reader.ReadUInt64(); // Unk
      _ = reader.ReadByte(); // Unk: Sentinel?
      _ = reader.ReadUInt32(); // Unk: Size?
      _ = reader.ReadByte(); // Unk: Sentinel?
      _ = reader.ReadUInt32(); // Unk

      return metadata;
    }

    #endregion



  }

}

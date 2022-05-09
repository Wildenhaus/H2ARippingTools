using LibH2A.Common;
using static Haus.Assertions;

namespace LibH2A.Saber3D
{

  public class S3D_GeometryNodeCollection
  {

    #region Constants

    private const uint SIGNATURE_OGM1 = 0x314D474F; //OGM1

    #endregion

    #region Data Members

    private List<S3D_GeometryNode> _nodes;
    private List<S3D_BoundingBox> _boundingBoxes;
    private List<string> _scripts;

    #endregion

    #region Properties

    public IReadOnlyList<S3D_BoundingBox> BoundingBoxes
    {
      get => _boundingBoxes;
    }

    public IReadOnlyList<S3D_GeometryNode> Nodes
    {
      get => _nodes;
    }

    public IReadOnlyList<string> Scripts
    {
      get => _scripts;
    }

    #endregion

    #region Constructor

    private S3D_GeometryNodeCollection()
    {
      _nodes = new List<S3D_GeometryNode>();
      _boundingBoxes = new List<S3D_BoundingBox>();
      _scripts = new List<string>();
    }

    public static S3D_GeometryNodeCollection Read( EndianBinaryReader reader )
    {
      /* Node Sections:
       *  01: Node Indices
       *  02: Node Names
       *  03: Unknown
       *  04: Unknown
       *  05: Unknown
       *  06: Unknown
       *  07: Unknown
       *  08: Unknown
       *  09: Unknown (Export Strings?)
       *  10: Unknown (Bone Matrices?)
       *  11: Unknown (Another Matrix)
       *  12: Unknown (300000007 section, Bounding Boxes)
       *  13: Unknown (Bone Chains/Hierarchy)
       *  14: Unknown (Padding?)
       *  15: Unknown (Bone Names?)
       *  16: Unknown (Export Strings Again?)
       */

      CheckSignature( reader );

      var nodeData = new S3D_GeometryNodeCollection();

      ReadSection_NodeIndices( nodeData, reader );
      ReadSection_NodeNames( nodeData, reader );
      ReadSection_UnkSection03( nodeData, reader );
      ReadSection_UnkSection04( nodeData, reader );
      ReadSection_UnkSection05( nodeData, reader );
      ReadSection_UnkSection06( nodeData, reader );
      ReadSection_UnkSection07( nodeData, reader );
      ReadSection_UnkSection08( nodeData, reader );
      ReadSection_UnkSection09( nodeData, reader );
      ReadSection_UnkSection10( nodeData, reader );
      ReadSection_UnkSection11( nodeData, reader );
      ReadSection_UnkSection12( nodeData, reader );
      ReadSection_UnkSection13( nodeData, reader );
      ReadSection_UnkSection14( nodeData, reader );
      ReadSection_UnkSection15( nodeData, reader );
      ReadSection_UnkSection16( nodeData, reader );

      return nodeData;
    }

    #endregion

    #region Private Methods

    private static void CheckSignature( EndianBinaryReader reader )
    {
      AssertEqual( reader.ReadUInt32(), SIGNATURE_OGM1,
        $"Invalid OGM1 Signature (0x{reader.BaseStream.Position:X}" );
    }

    private static void ReadSection_NodeIndices( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry._nodes = new List<S3D_GeometryNode>();

      _ = reader.ReadUInt16(); // Unk
      _ = reader.ReadUInt16(); // Unk
      _ = reader.ReadUInt16(); // Unk

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var nodeCount = reader.ReadUInt16();

      _ = reader.ReadUInt16(); // Unk
      _ = reader.ReadUInt16(); // Unk
      _ = reader.ReadUInt16(); // Unk

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodeCount; i++ )
      {
        var index = reader.ReadUInt16();
        nodes.Add( new S3D_GeometryNode { Index = index } );
      }
    }

    private static void ReadSection_NodeNames( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry._nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Name = reader.ReadPascalString32();
    }

    private static void ReadSection_UnkSection03( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
      {
        nodes[ i ].Unk_03 = new S3D_Geometry_UnkSection03
        {
          Unk_01 = reader.ReadUInt16(),
          Unk_02_IsMesh = reader.ReadUInt16(),
          Unk_03_IsBone = reader.ReadUInt16(),
          Unk_04_IsHighestLod = reader.ReadByte()
        };
      }
    }

    private static void ReadSection_UnkSection04( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_04_ParentId = reader.ReadUInt16();
    }

    private static void ReadSection_UnkSection05( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_05_SiblingId = reader.ReadUInt16();
    }

    private static void ReadSection_UnkSection06( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_06_SiblingId = reader.ReadUInt16();
    }

    private static void ReadSection_UnkSection07( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_07_ChildId = reader.ReadUInt16();
    }

    private static void ReadSection_UnkSection08( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_08_BoneId = reader.ReadUInt16();
    }

    private static void ReadSection_UnkSection09( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_09_ExportName = reader.ReadPascalString32();
    }

    private static void ReadSection_UnkSection10( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_10_BoneMatrix = reader.ReadMatrix4x4();
    }

    private static void ReadSection_UnkSection11( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_11_InverseMatrix = reader.ReadMatrix4x4();
    }

    private static void ReadSection_UnkSection12( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var boundingBoxes = geometry._boundingBoxes = new List<S3D_BoundingBox>();

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      // TODO: Find count to this section.
      //Find first bounding box
      while ( true )
      {
        var startPos = reader.BaseStream.Position;
        var a = reader.ReadUInt32();
        var b = reader.ReadByte();

        if ( a == 0x03 && b == 0x07 )
        {
          reader.BaseStream.Position = startPos;
          break;
        }
        else
          reader.BaseStream.Position = startPos + 1;
      }

      while ( true )
      {
        if ( reader.PeekInt32() != 0x03 )
          break;

        boundingBoxes.Add( new S3D_BoundingBox
        {
          Unk_01 = reader.ReadUInt32(),
          Unk_02 = reader.ReadByte(),
          Unk_03_SubMeshIndex = reader.ReadUInt32(),
          Unk_04_SubMeshRangeCount = reader.ReadUInt32(),
          Unk_05_Min = reader.ReadVector3(),
          Unk_06_Max = reader.ReadVector3()
        } );
      }
    }

    private static void ReadSection_UnkSection13( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_13_BoneHeirarchy = reader.ReadPascalString32();
    }

    private static void ReadSection_UnkSection14( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      // TODO: Figure out what this is. I've only seen zero data
      for ( var i = 0; i < nodes.Count; i++ )
        for ( var j = 0; j < 60; j++ )
          reader.ReadByte();
    }

    private static void ReadSection_UnkSection15( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      var sentinel = reader.ReadByte();
      if ( sentinel == 0 )
        return;

      // If sentinel is 1, it's strings
      if ( sentinel == 0x1 )
      {
        for ( var i = 0; i < nodes.Count; i++ )
          nodes[ i ].Unk_15_BoneName = reader.ReadPascalString16();
      }

      // If sentinel is 8, it's some kind of scripting
      else if ( sentinel == 0x8 )
      {
        _ = reader.ReadUInt16();
        _ = reader.ReadUInt16();

        // TODO: is this right (sentinel as count)?
        // The only file that seems to have this is ss_prop__h.tpl

        var scripts = geometry._scripts;
        for ( var i = 0; i < sentinel; i++ )
        {
          _ = reader.ReadUInt32();
          scripts.Add( reader.ReadPascalString32() );
        }
      }
      else
        throw new Exception( $"Unknown sentinel: {sentinel}" );
    }

    private static void ReadSection_UnkSection16( S3D_GeometryNodeCollection geometry, EndianBinaryReader reader )
    {
      var nodes = geometry.Nodes;

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < nodes.Count; i++ )
        nodes[ i ].Unk_16_ExportName = reader.ReadPascalString16();
    }

    #endregion

  }

}

using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using LibH2A.Common;
using LibH2A.Saber3D;

namespace H2ARipper.Converters
{

  public class TplToFbxConverter
  {

    //todo: don't use aspose.3d for release. it's a trial version
    public static void Convert( string inFile, string outFile )
    {
      var reader = new EndianBinaryReader( File.OpenRead( inFile ) );
      var tpl = S3D_Template.Open( File.OpenRead( inFile ) );

      var scene = new Scene();

      foreach ( var submesh in tpl.Geometry.Data.SubMeshes )
        AddSubMesh( submesh, tpl, reader, scene );

      //var mesh = new Mesh();
      //var node = new Node( "", mesh );

      //foreach ( var vert in tpl.Geometry.Data.Vertices )
      //  mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( vert.X, vert.Y, vert.Z ) );
      //foreach ( var face in tpl.Geometry.Data.Faces )
      //  mesh.CreatePolygon( face.A, face.B, face.C );

      //scene.RootNode.AddChildNode( node );

      scene.Save( File.Create( outFile ), FileFormat.FBX7700Binary );
    }

    //private static void AddMesh( S3D_GeometryData.S3D_MeshData[] meshData, Scene scene, EndianBinaryReader reader, S3D_Template tpl )
    //{
    //  var mesh = new Mesh();
    //  var node = new Node( "", mesh );
    //  scene.RootNode.AddChildNode( node );

    //  foreach ( var bufInfo in meshData )
    //  {
    //    var buffer = tpl.Geometry.Data.Buffers[ ( int ) bufInfo.BufferId ];
    //    reader.Seek( buffer.DataOffset + bufInfo.SubBufferOffset, SeekOrigin.Begin );

    //    switch ( buffer.BufferType )
    //    {
    //      case S3D_GeometryData.S3D_BufferType.Face:
    //        while ( reader.BaseStream.Position < buffer.DataOffset + buffer.BufferLength )
    //          mesh.CreatePolygon( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() );
    //        break;
    //      case S3D_GeometryData.S3D_BufferType.Unk_Face:
    //        while ( reader.BaseStream.Position < buffer.DataOffset + buffer.BufferLength )
    //          mesh.CreatePolygon( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() );
    //        break;
    //      case S3D_GeometryData.S3D_BufferType.StaticVert:
    //      case S3D_GeometryData.S3D_BufferType.SkinnedVert:
    //        //case S3D_GeometryData.S3D_BufferType.NormalVert: Broken, probably a different structure
    //        while ( reader.BaseStream.Position < buffer.DataOffset + buffer.BufferLength )
    //        {
    //          mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() ) );
    //          reader.BaseStream.Position += buffer.ElementSize - 6;
    //        }
    //        break;
    //    }

    //  }
    //}

    private static void AddSubMesh( S3D_GeometryData.S3D_SubMesh submesh, S3D_Template tpl, EndianBinaryReader reader, Scene scene )
    {
      var meshData = tpl.Geometry.Data.MeshData[ submesh.Unk_MeshId ];

      var mesh = new Mesh();
      var meshNode = new Node( "", mesh );
      scene.RootNode.AddChildNode( meshNode );

      foreach ( var bufInfo in meshData )
      {
        var buffer = tpl.Geometry.Data.Buffers[ ( int ) bufInfo.BufferId ];
        var bufferEnd = buffer.DataOffset + buffer.BufferLength;
        var startOffset = buffer.DataOffset + bufInfo.SubBufferOffset;

        switch ( buffer.BufferType )
        {
          case S3D_GeometryData.S3D_BufferType.Face:
            reader.Seek( startOffset + ( submesh.FaceOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.FaceCount; i++ )
              mesh.CreatePolygon( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() );
            break;
          case S3D_GeometryData.S3D_BufferType.Unk_Face:
            reader.Seek( startOffset + ( submesh.FaceOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.FaceCount; i++ )
              mesh.CreatePolygon( reader.ReadInt16(), reader.ReadInt16(), 0 );
            break;
          case S3D_GeometryData.S3D_BufferType.NormalVert:
            //reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            //for ( var i = 0; i < submesh.VertCount; i++ )
            //{
            //  mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() ) );
            //  reader.BaseStream.Position += buffer.ElementSize - 8;
            //}
            break;//Broken, probably a different structure
          case S3D_GeometryData.S3D_BufferType.StaticVert:
          case S3D_GeometryData.S3D_BufferType.SkinnedVert:
            reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.VertCount; i++ )
            {
              mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() ) );
              reader.BaseStream.Position += buffer.ElementSize - 8;
            }
            break;
          default:
            Console.WriteLine( "Unknown buffer: {0:X}", ( int ) buffer.BufferType );
            break;
        }
      }

    }
  }

}

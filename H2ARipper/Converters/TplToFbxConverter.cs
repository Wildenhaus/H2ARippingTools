using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using LibH2A.Common;
using LibH2A.Saber3D;
using LibH2A.Saber3D.Geometry;

namespace H2ARipper.Converters
{

  //public class TplToFbxConverter
  //{

  //  //todo: don't use aspose.3d for release. it's a trial version
  //  public static void Convert( string inFile, string outFile )
  //  {
  //    var reader = new EndianBinaryReader( File.OpenRead( inFile ) );
  //    var tpl = S3D_Template.Open( File.OpenRead( inFile ) );

  //    var scene = new Scene();

  //    var x = 0;
  //    foreach ( var submesh in tpl.Geometry.Data.SubMeshes )
  //      //if ( ++x == 104 )
  //      AddSubMesh( submesh, tpl, reader, scene );

  //    scene.Save( File.Create( outFile ), FileFormat.FBX7700Binary );
  //  }

  //  private static void AddSubMesh( S3D_SubMesh submesh, S3D_Template tpl, EndianBinaryReader reader, Scene scene )
  //  {
  //    var meshData = tpl.Geometry.Data.Meshes[ ( int ) submesh.MeshId ];

  //    var mesh = new Mesh();
  //    var meshNode = new Node( "", mesh );
  //    scene.RootNode.AddChildNode( meshNode );

  //    var uvs = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
  //    foreach ( var bufInfo in meshData.Buffers )
  //    {
  //      var buffer = tpl.Geometry.Data.Buffers[ ( int ) bufInfo.BufferId ];
  //      var bufferEnd = buffer.StartOffset + buffer.BufferLength;
  //      var startOffset = buffer.StartOffset + bufInfo.SubBufferOffset;

  //      switch ( buffer.BufferType )
  //      {
  //        case S3D_GeometryBufferType.Face:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.FaceOffset * buffer.ElementSize ), SeekOrigin.Begin );

  //          // Load faces
  //          var faces = new List<short[]>();
  //          for ( var i = 0; i < submesh.BufferInfo.FaceCount; i++ )
  //            faces.Add( new[] { reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() } );

  //          // Normalize the vertex indices
  //          var min = faces.Min( x => x.Min() );
  //          foreach ( var f in faces )
  //            for ( var i = 0; i < f.Length; i++ )
  //              f[ i ] -= min;

  //          foreach ( var f in faces )
  //            mesh.CreatePolygon( f[ 0 ], f[ 1 ], f[ 2 ] );

  //          break;
  //        case S3D_GeometryBufferType.Unk_Face:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.FaceOffset * buffer.ElementSize ), SeekOrigin.Begin );
  //          for ( var i = 0; i < submesh.BufferInfo.FaceCount; i++ )
  //            _ = reader.ReadInt32(); // Unk: MaterialID?
  //          break;
  //        case S3D_GeometryBufferType.StaticVert:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.VertexOffset * buffer.ElementSize ), SeekOrigin.Begin );
  //          for ( var i = 0; i < submesh.BufferInfo.VertexCount; i++ )
  //          {
  //            var x = SNorm( reader.ReadInt16() );
  //            var y = SNorm( reader.ReadInt16() );
  //            var z = SNorm( reader.ReadInt16() );
  //            var w = SNorm( reader.ReadInt16() );
  //            mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( x, y, z, w ) );
  //          }
  //          break;
  //        case S3D_GeometryBufferType.SkinnedVert:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.VertexOffset * buffer.ElementSize ), SeekOrigin.Begin );
  //          for ( var i = 0; i < submesh.BufferInfo.VertexCount; i++ )
  //          {
  //            var x = SNorm( reader.ReadInt16() );
  //            var y = SNorm( reader.ReadInt16() );
  //            var z = SNorm( reader.ReadInt16() );
  //            var w = SNorm( reader.ReadInt16() );

  //            var weight1 = reader.ReadByte();
  //            var weight2 = reader.ReadByte();
  //            var weight3 = reader.ReadByte();
  //            var weight4 = reader.ReadByte();

  //            var bone1 = reader.ReadByte();
  //            var bone2 = reader.ReadByte();
  //            var bone3 = reader.ReadByte();
  //            var bone4 = reader.ReadByte();

  //            mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( x, y, z, w ) );
  //          }
  //          break;
  //        case S3D_GeometryBufferType.UvAndUnk:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.VertexOffset * buffer.ElementSize ), SeekOrigin.Begin );
  //          for ( var i = 0; i < submesh.BufferInfo.VertexCount; i++ )
  //          {
  //            var unk = reader.ReadInt32(); // Blend Indices?

  //            var u = SNorm( reader.ReadInt16() );
  //            var v = 1 - SNorm( reader.ReadInt16() );
  //            uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u, v, 0, 0 ) );
  //          }
  //          break;
  //        case S3D_GeometryBufferType.Unk_Vert30:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.VertexOffset * buffer.ElementSize ), SeekOrigin.Begin );
  //          for ( var i = 0; i < submesh.BufferInfo.VertexCount; i++ )
  //          {
  //            _ = reader.ReadInt32(); // Unk: Normals?
  //            _ = reader.ReadInt32(); // Unk: Binormals?
  //            var u1 = SNorm( reader.ReadInt16() );
  //            var v1 = 1 - SNorm( reader.ReadInt16() );
  //            var u2 = SNorm( reader.ReadInt16() );
  //            var v2 = 1 - SNorm( reader.ReadInt16() );

  //            uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u1, v1, 0, 0 ) );
  //            uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u2, v2, 0, 0 ) );
  //          }
  //          break;
  //        case S3D_GeometryBufferType.Unk_Vert70:
  //          reader.Seek( startOffset + ( submesh.BufferInfo.VertexOffset * buffer.ElementSize ), SeekOrigin.Begin );
  //          for ( var i = 0; i < submesh.BufferInfo.VertexCount; i++ )
  //          {
  //            _ = reader.ReadInt32(); // Unk: Normals?
  //            _ = reader.ReadInt32(); // Unk: Binormals?
  //            _ = reader.ReadInt32(); // Unk: Tangents?
  //            var u1 = SNorm( reader.ReadInt16() );
  //            var v1 = 1 - SNorm( reader.ReadInt16() );
  //            var u2 = SNorm( reader.ReadInt16() );
  //            var v2 = 1 - SNorm( reader.ReadInt16() );
  //            var u3 = SNorm( reader.ReadInt16() );
  //            var v3 = 1 - SNorm( reader.ReadInt16() );

  //            uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u1, v1, 0, 0 ) );
  //            uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u2, v2, 0, 0 ) );
  //            uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u3, v3, 0, 0 ) );
  //          }
  //          break;
  //        default:
  //          Console.WriteLine( "Unknown buffer: {0:X}", ( int ) buffer.BufferType );
  //          break;
  //      }
  //    }

  //    // Transforms
  //    var pos = submesh.Position;
  //    var posX = SNorm( ( short ) pos.X );
  //    var posY = SNorm( ( short ) pos.Y );
  //    var posZ = SNorm( ( short ) pos.Z );
  //    meshNode.Transform.Translation = new Vector3( posX, posY, posZ );

  //    var scale = submesh.Scale;
  //    var scaleX = scale.X;
  //    var scaleY = scale.Y;
  //    var scaleZ = scale.Z;
  //    meshNode.Transform.Scale = new Vector3( scaleX, scaleY, scaleZ );

  //  }

  //  private static double SNorm( in short value )
  //    => ( double ) value / short.MaxValue;

  //  private static double UNorm( in ushort value )
  //    => ( double ) value / ushort.MaxValue;

  //}

  public class TplToFbxConverter
  {
    // TODO: don't use Aspose.3D for release.
    // It's a trial version and will fail after 50 models.

    public static void Convert( string inFile, string outFile )
      => Convert( File.OpenRead( inFile ), outFile );

    public static void Convert( Stream stream, string outFile )
    {
      var reader = new EndianBinaryReader( stream );
      var tplFile = S3D_Template.Open( stream );
      var geometry = tplFile.Geometry;

      var scene = new Scene();
      //foreach ( var mesh in geometry.Data.Meshes )
      //  AddMesh( scene, mesh, reader );

      foreach ( var submesh in geometry.Data.SubMeshes )
        AddSubMesh( scene.RootNode, submesh, reader );

      scene.Save( File.Create( outFile ), FileFormat.FBX7700Binary );
    }

    private static void AddMesh( Scene scene, S3D_Mesh mesh, EndianBinaryReader reader )
    {
      var meshNode = new Node();
      scene.RootNode.AddChildNode( meshNode );

      foreach ( var subMesh in mesh.SubMeshes )
        AddSubMesh( meshNode, subMesh, reader );
    }

    private static void AddSubMesh( Node meshNode, S3D_SubMesh submeshData, EndianBinaryReader reader )
    {
      var meshData = submeshData.ParentMesh;
      var bufferData = meshData.Parent.Buffers;

      var submeshEntity = new Mesh();
      var submeshNode = new Node( meshNode.Name, submeshEntity );
      meshNode.AddChildNode( submeshNode );

      var uvs = submeshEntity.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
      foreach ( var meshBuffer in meshData.Buffers )
      {
        var buffer = bufferData[ ( int ) meshBuffer.BufferId ];
        var startOffset = buffer.StartOffset + meshBuffer.SubBufferOffset;

        var submeshBufferInfo = submeshData.BufferInfo;
        switch ( buffer.BufferType )
        {
          case S3D_GeometryBufferType.Face:
          {
            var subBufferOffset = submeshBufferInfo.FaceOffset * buffer.ElementSize;
            reader.Seek( startOffset + subBufferOffset, SeekOrigin.Begin );

            // Load Faces
            var faces = new List<short[]>();
            for ( var i = 0; i < submeshBufferInfo.FaceCount; i++ )
              faces.Add( new[] { reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() } );

            // Normalize the indices
            var min = faces.Min( x => x.Min() );
            foreach ( var f in faces )
              for ( var i = 0; i < f.Length; i++ )
                f[ i ] -= min;

            foreach ( var f in faces )
              submeshEntity.CreatePolygon( f[ 0 ], f[ 1 ], f[ 2 ] );
          }
          break;
          case S3D_GeometryBufferType.StaticVert:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.Seek( startOffset + subBufferOffset, SeekOrigin.Begin );

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              var x = reader.ReadInt16().ToSNorm();
              var y = reader.ReadInt16().ToSNorm();
              var z = reader.ReadInt16().ToSNorm();
              var w = reader.ReadInt16().ToSNorm();

              submeshEntity.ControlPoints.Add( new Vector4( x, y, z, w ) );
            }
          }
          break;
          case S3D_GeometryBufferType.SkinnedVert:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.Seek( startOffset + subBufferOffset, SeekOrigin.Begin );

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              var x = reader.ReadInt16().ToSNorm();
              var y = reader.ReadInt16().ToSNorm();
              var z = reader.ReadInt16().ToSNorm();
              var w = reader.ReadInt16().ToSNorm();

              // TODO: Bones and Weights
              var weight1 = reader.ReadByte();
              var weight2 = reader.ReadByte();
              var weight3 = reader.ReadByte();
              var weight4 = reader.ReadByte();

              var bone1 = reader.ReadByte();
              var bone2 = reader.ReadByte();
              var bone3 = reader.ReadByte();
              var bone4 = reader.ReadByte();

              submeshEntity.ControlPoints.Add( new Vector4( x, y, z, w ) );
            }
          }
          break;
          case S3D_GeometryBufferType.UvAndUnk:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.Seek( startOffset + subBufferOffset, SeekOrigin.Begin );

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              var unk = reader.ReadInt32(); // Normals? Blend Indices?

              var u = reader.ReadInt16().ToSNorm();
              var v = 1 - reader.ReadInt16().ToSNorm();
              uvs.Data.Add( new Vector4( u, v, 0, 0 ) );
            }
          }
          break;
          case S3D_GeometryBufferType.Unk_Vert30:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.Seek( startOffset + subBufferOffset, SeekOrigin.Begin );

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              _ = reader.ReadInt32(); // Unk: Normal?
              _ = reader.ReadInt32(); // Unk: Binormal?

              var u1 = reader.ReadInt16().ToSNorm();
              var v1 = 1 - reader.ReadInt16().ToSNorm();
              var u2 = reader.ReadInt16().ToSNorm();
              var v2 = 1 - reader.ReadInt16().ToSNorm();

              uvs.Data.Add( new Vector4( u1, v1, 0, 0 ) );
              uvs.Data.Add( new Vector4( u2, v2, 0, 0 ) );
            }
          }
          break;
          case S3D_GeometryBufferType.Unk_Vert70:
          {
            var subBufferOffset = submeshBufferInfo.VertexOffset * buffer.ElementSize;
            reader.Seek( startOffset + subBufferOffset, SeekOrigin.Begin );

            for ( var i = 0; i < submeshBufferInfo.VertexCount; i++ )
            {
              _ = reader.ReadInt32(); // Unk: Normal?
              _ = reader.ReadInt32(); // Unk: Binormal?
              _ = reader.ReadInt32(); // Unk: Tangents?

              var u1 = reader.ReadInt16().ToSNorm();
              var v1 = 1 - reader.ReadInt16().ToSNorm();
              var u2 = reader.ReadInt16().ToSNorm();
              var v2 = 1 - reader.ReadInt16().ToSNorm();
              var u3 = reader.ReadInt16().ToSNorm();
              var v3 = 1 - reader.ReadInt16().ToSNorm();

              uvs.Data.Add( new Vector4( u1, v1, 0, 0 ) );
              uvs.Data.Add( new Vector4( u2, v2, 0, 0 ) );
              uvs.Data.Add( new Vector4( u3, v3, 0, 0 ) );
            }
          }
          break;
        }
      }

      // Transforms
      //var pos = submeshData.Position;
      //submeshNode.Transform.Translation = new Vector3( pos.X, pos.Y, pos.Z );

      //var scale = submeshData.Position;
      //submeshNode.Transform.Scale = new Vector3( scale.X, scale.Y, scale.Z );
    }

  }

}

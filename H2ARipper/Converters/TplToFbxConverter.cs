﻿using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using LibH2A.Common;
using LibH2A.Saber3D;
using LibH2A.Saber3D.Geometry;

namespace H2ARipper.Converters
{

  public class TplToFbxConverter
  {
    // TODO: don't use Aspose.3D for release.
    // It's a trial version and will fail after 50 models.

    public static void Convert( string inFile, string outFile )
      => Convert( File.OpenRead( inFile ), outFile );

    public static void Convert( Stream stream, string outFile )
    {
      var tplFile = S3D_Template.Open( stream );
      stream = tplFile.Stream;

      var reader = new EndianBinaryReader( stream );
      var geometry = tplFile.Geometry;

      var scene = new Scene();
      foreach ( var mesh in geometry.Data.Meshes )
        AddMesh( scene, mesh, reader );

      //foreach ( var submesh in geometry.Data.SubMeshes )
      //  AddSubMesh( scene.RootNode, submesh, reader );

      scene.Save( File.Create( outFile ), FileFormat.FBX7700Binary );
    }

    private static void AddMesh( Scene scene, S3D_Mesh mesh, EndianBinaryReader reader )
    {
      var nodes = mesh.Parent.Parent.Nodes;

      var meshNode = new Node();
      meshNode.Transform.Scale = new Vector3( 10, 10, 10 );
      scene.RootNode.AddChildNode( meshNode );

      foreach ( var subMesh in mesh.SubMeshes )
      {
        var submeshNode = AddSubMesh( meshNode, subMesh, reader );
        submeshNode.Name = nodes[ subMesh.NodeId ].Name;
      }
    }

    private static Node AddSubMesh( Node meshNode, S3D_SubMesh submeshData, EndianBinaryReader reader )
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
          case S3D_GeometryBufferType.Unk_Face:
          {

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
          default:
            break;
        }
      }

      // Transforms
      var pos = submeshData.Position;
      submeshNode.Transform.Translation = new Vector3( pos.X, pos.Y, pos.Z );

      var scale = submeshData.Scale;
      const float scaleFactor = 1f;
      submeshNode.Transform.Scale = new Vector3( scale.X * scaleFactor, scale.Y * scaleFactor, scale.Z * scaleFactor );

      return submeshNode;
    }

  }

}

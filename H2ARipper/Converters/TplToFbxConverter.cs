using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
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

      var x = 0;
      foreach ( var submesh in tpl.Geometry.Data.SubMeshes )
        //if ( ++x == 104 )
        AddSubMesh( submesh, tpl, reader, scene );

      scene.Save( File.Create( outFile ), FileFormat.FBX7700Binary );
    }

    private static void AddSubMesh( S3D_GeometryData.S3D_SubMesh submesh, S3D_Template tpl, EndianBinaryReader reader, Scene scene )
    {
      var meshData = tpl.Geometry.Data.MeshData[ submesh.Unk_MeshId ];

      var mesh = new Mesh();
      var meshNode = new Node( "", mesh );
      scene.RootNode.AddChildNode( meshNode );

      var uvs = mesh.CreateElementUV( TextureMapping.Diffuse, MappingMode.ControlPoint, ReferenceMode.Direct );
      foreach ( var bufInfo in meshData )
      {
        var buffer = tpl.Geometry.Data.Buffers[ ( int ) bufInfo.BufferId ];
        var bufferEnd = buffer.DataOffset + buffer.BufferLength;
        var startOffset = buffer.DataOffset + bufInfo.SubBufferOffset;

        switch ( buffer.BufferType )
        {
          case S3D_GeometryData.S3D_BufferType.Face:
            reader.Seek( startOffset + ( submesh.FaceOffset * buffer.ElementSize ), SeekOrigin.Begin );

            // Load faces
            var faces = new List<short[]>();
            for ( var i = 0; i < submesh.FaceCount; i++ )
              faces.Add( new[] { reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16() } );

            // Normalize the vertex indices
            var min = faces.Min( x => x.Min() );
            foreach ( var f in faces )
              for ( var i = 0; i < f.Length; i++ )
                f[ i ] -= min;

            foreach ( var f in faces )
              mesh.CreatePolygon( f[ 0 ], f[ 1 ], f[ 2 ] );

            break;
          case S3D_GeometryData.S3D_BufferType.Unk_Face:
            reader.Seek( startOffset + ( submesh.FaceOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.FaceCount; i++ )
              _ = reader.ReadInt32(); // Unk: MaterialID?
            break;
          case S3D_GeometryData.S3D_BufferType.StaticVert:
            reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.VertCount; i++ )
            {
              var x = SNorm( reader.ReadInt16() );
              var y = SNorm( reader.ReadInt16() );
              var z = SNorm( reader.ReadInt16() );
              var w = SNorm( reader.ReadInt16() );
              mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( x, y, z, w ) );
            }
            break;
          case S3D_GeometryData.S3D_BufferType.SkinnedVert:
            reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.VertCount; i++ )
            {
              var x = SNorm( reader.ReadInt16() );
              var y = SNorm( reader.ReadInt16() );
              var z = SNorm( reader.ReadInt16() );
              var w = SNorm( reader.ReadInt16() );

              var weight1 = reader.ReadByte();
              var weight2 = reader.ReadByte();
              var weight3 = reader.ReadByte();
              var weight4 = reader.ReadByte();

              var bone1 = reader.ReadByte();
              var bone2 = reader.ReadByte();
              var bone3 = reader.ReadByte();
              var bone4 = reader.ReadByte();

              mesh.ControlPoints.Add( new Aspose.ThreeD.Utilities.Vector4( x, y, z, w ) );
            }
            break;
          case S3D_GeometryData.S3D_BufferType.UvAndUnk:
            reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.VertCount; i++ )
            {
              var unk = reader.ReadInt32(); // Blend Indices?

              var u = SNorm( reader.ReadInt16() );
              var v = 1 - SNorm( reader.ReadInt16() );
              uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u, v, 0, 0 ) );
            }
            break;
          case S3D_GeometryData.S3D_BufferType.Unk_Vert30:
            reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.VertCount; i++ )
            {
              _ = reader.ReadInt32(); // Unk: Normals?
              _ = reader.ReadInt32(); // Unk: Binormals?
              var u1 = SNorm( reader.ReadInt16() );
              var v1 = 1 - SNorm( reader.ReadInt16() );
              var u2 = SNorm( reader.ReadInt16() );
              var v2 = 1 - SNorm( reader.ReadInt16() );

              uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u1, v1, 0, 0 ) );
              uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u2, v2, 0, 0 ) );
            }
            break;
          case S3D_GeometryData.S3D_BufferType.Unk_Vert70:
            reader.Seek( startOffset + ( submesh.VertOffset * buffer.ElementSize ), SeekOrigin.Begin );
            for ( var i = 0; i < submesh.VertCount; i++ )
            {
              _ = reader.ReadInt32(); // Unk: Normals?
              _ = reader.ReadInt32(); // Unk: Binormals?
              _ = reader.ReadInt32(); // Unk: Tangents?
              var u1 = SNorm( reader.ReadInt16() );
              var v1 = 1 - SNorm( reader.ReadInt16() );
              var u2 = SNorm( reader.ReadInt16() );
              var v2 = 1 - SNorm( reader.ReadInt16() );
              var u3 = SNorm( reader.ReadInt16() );
              var v3 = 1 - SNorm( reader.ReadInt16() );

              uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u1, v1, 0, 0 ) );
              uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u2, v2, 0, 0 ) );
              uvs.Data.Add( new Aspose.ThreeD.Utilities.Vector4( u3, v3, 0, 0 ) );
            }
            break;
          default:
            Console.WriteLine( "Unknown buffer: {0:X}", ( int ) buffer.BufferType );
            break;
        }
      }

      // Transforms
      var pos = submesh.TransformInfo.Position;
      var posX = SNorm( pos[ 0 ] );
      var posY = SNorm( pos[ 1 ] );
      var posZ = SNorm( pos[ 2 ] );
      meshNode.Transform.Translation = new Vector3( posX, posY, posZ );

      var scale = submesh.TransformInfo.Scale;
      var scaleX = scale[ 0 ];
      var scaleY = scale[ 1 ];
      var scaleZ = scale[ 2 ];
      meshNode.Transform.Scale = new Vector3( scaleX, scaleY, scaleZ );

    }

    private static double SNorm( in short value )
      => ( double ) value / short.MaxValue;

    private static double UNorm( in ushort value )
      => ( double ) value / ushort.MaxValue;

  }

}

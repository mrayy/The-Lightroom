using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator  {

	public static Mesh GeneratePlane(float length=1.0f,float width=1.0f,int resX=2,int resZ=2)
	{
		Mesh mesh = new Mesh();
		mesh.Clear();
		 
		 
		#region Vertices		
		Vector3[] vertices = new Vector3[ resX * resZ ];
		for(int z = 0; z < resZ; z++)
		{
			// [ -length / 2, length / 2 ]
			float zPos = ((float)z / (resZ - 1) - .5f) * length;
			for(int x = 0; x < resX; x++)
			{
				// [ -width / 2, width / 2 ]
				float xPos = ((float)x / (resX - 1) - .5f) * width;
				vertices[ x + z * resX ] = new Vector3( xPos, 0f, zPos );
			}
		}
		#endregion
		 
		#region Normales
		Vector3[] normales = new Vector3[ vertices.Length ];
		for( int n = 0; n < normales.Length; n++ )
			normales[n] = Vector3.up;
		#endregion
		 
		#region UVs		
		Vector2[] uvs = new Vector2[ vertices.Length ];
		for(int v = 0; v < resZ; v++)
		{
			for(int u = 0; u < resX; u++)
			{
				uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
			}
		}
		#endregion
		 
		#region Triangles
		int nbFaces = (resX - 1) * (resZ - 1);
		int[] triangles = new int[ nbFaces * 6 ];
		int t = 0;
		for(int face = 0; face < nbFaces; face++ )
		{
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);
		 
			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;
		 
			triangles[t++] = i + resX;	
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1; 
		}
		#endregion
		 
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		 
		mesh.RecalculateBounds();
		return mesh;
	}

	public static Mesh GenerateBox(Vector3 pivot,float width=1f,float length = 1f,float height=1.0f)
	{
		Mesh mesh = new Mesh ();
		mesh.Clear();



		#region Vertices
		Vector3 p0 = pivot+ new Vector3( -length * .5f,	-width * .5f, height * .5f );
		Vector3 p1 = pivot+new Vector3( length * .5f, 	-width * .5f, height * .5f );
		Vector3 p2 = pivot+new Vector3( length * .5f, 	-width * .5f, -height * .5f );
		Vector3 p3 = pivot+new Vector3( -length * .5f,	-width * .5f, -height * .5f );	

		Vector3 p4 = pivot+new Vector3( -length * .5f,	width * .5f,  height * .5f );
		Vector3 p5 = pivot+new Vector3( length * .5f, 	width * .5f,  height * .5f );
		Vector3 p6 = pivot+new Vector3( length * .5f, 	width * .5f,  -height * .5f );
		Vector3 p7 = pivot+new Vector3( -length * .5f,	width * .5f,  -height * .5f );

		Vector3[] vertices = new Vector3[]
		{
			// Bottom
			p0, p1, p2, p3,

			// Left
			p7, p4, p0, p3,

			// Front
			p4, p5, p1, p0,

			// Back
			p6, p7, p3, p2,

			// Right
			p5, p6, p2, p1,

			// Top
			p7, p6, p5, p4
		};
		#endregion

		#region Normales
		Vector3 up 	= Vector3.up;
		Vector3 down 	= Vector3.down;
		Vector3 front 	= Vector3.forward;
		Vector3 back 	= Vector3.back;
		Vector3 left 	= Vector3.left;
		Vector3 right 	= Vector3.right;

		Vector3[] normales = new Vector3[]
		{
			// Bottom
			down, down, down, down,

			// Left
			left, left, left, left,

			// Front
			front, front, front, front,

			// Back
			back, back, back, back,

			// Right
			right, right, right, right,

			// Top
			up, up, up, up
		};
		#endregion	

		#region UVs
		Vector2 _00 = new Vector2( 0f, 0f );
		Vector2 _10 = new Vector2( 1f, 0f );
		Vector2 _01 = new Vector2( 0f, 1f );
		Vector2 _11 = new Vector2( 1f, 1f );

		Vector2[] uvs = new Vector2[]
		{
			// Bottom
			_11, _01, _00, _10,

			// Left
			_11, _01, _00, _10,

			// Front
			_11, _01, _00, _10,

			// Back
			_11, _01, _00, _10,

			// Right
			_11, _01, _00, _10,

			// Top
			_11, _01, _00, _10,
		};
		#endregion

		#region Triangles
		int[] triangles = new int[]
		{
			// Bottom
			3, 1, 0,
			3, 2, 1,			

			// Left
			3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
			3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

			// Front
			3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
			3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

			// Back
			3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
			3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

			// Right
			3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
			3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

			// Top
			3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
			3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

		};
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();

		return mesh;

	}
}

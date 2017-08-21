using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancing  {

	public Mesh srcMesh;

	public int InstancesCount;

	public Matrix4x4 TransformationMatrix=Matrix4x4.identity;

	public delegate void OnMeshInstancedDeleg (int index);
	public OnMeshInstancedDeleg OnMeshInstanced;

	public Vector3[] InstancesPosition;


	public Mesh CreateInstanceMesh()
	{
		int nv = srcMesh.vertices.Length;

		int count = InstancesCount;

		var Verts = new Vector3[count*nv];
		var Normals = new Vector3[count*nv];
		var texCoords=new Vector2[count*nv];
		var srcVerts = srcMesh.vertices;
		var srcNormals = srcMesh.normals;
		var srcIndices = srcMesh.GetIndices (0);

		var Ai = 0;

		Vector3 pos = Vector3.zero;

		for(int c=0;c<InstancesCount;++c)
		{
			Vector2 uv=new Vector2((float)c/InstancesCount,0);
			if (InstancesPosition != null)
				pos = InstancesPosition [c];
			for (int i = 0; i < srcVerts.Length; ++i) {
				Verts [Ai + i] = TransformationMatrix.MultiplyPoint(srcVerts[i]+pos);
				Normals [Ai + i] = TransformationMatrix.MultiplyVector( srcNormals [i]);
				texCoords [Ai + i] = uv;
			}
			if(OnMeshInstanced!=null)
				OnMeshInstanced (c);
			Ai += srcVerts.Length;
		}
		var Indicies = new int[count*srcIndices.Length];
		Ai = 0;
		int Av = 0;
		for (int o = 0; o < count; ++o) {

			for (int i = 0; i < srcIndices.Length; ++i) {
				Indicies [Ai + i] = srcIndices [i] + Av;
			}
			Ai += srcIndices.Length;
			Av += srcVerts.Length;
		}

		var mesh = new Mesh ();
		mesh.vertices = Verts;
		mesh.normals = Normals;
		mesh.uv = texCoords;
		mesh.SetIndices (Indicies,srcMesh.GetTopology(0),0);
		mesh.RecalculateBounds ();
		mesh.hideFlags = HideFlags.DontSave;

		return mesh;
	}


	public Texture CreateDataTexture(bool gpu)
	{
		Texture tex;
		if (gpu)
			tex = new RenderTexture (InstancesCount, 1, 1, RenderTextureFormat.ARGBFloat);
		else
			tex = new Texture2D (InstancesCount, 1, TextureFormat.RGBAFloat, false);
		tex.hideFlags = HideFlags.DontSave;
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Repeat;
		return tex;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallGenerator : MonoBehaviour {


	public int TilesCount
	{
		get{
			return 1+(int)(1.0f / TileSize);
		}
	}
	public float TileSize
	{
		get{
			return Prefab.transform.localScale.x;
		}
	}

	public Transform Prefab;

	public Color BaseColor;

	public WallCubeBehaviour.WallConfigurations Config=new WallCubeBehaviour.WallConfigurations();


	List<WallCubeBehaviour> _cubes=new List<WallCubeBehaviour>();


	//shaders
	[SerializeField] Shader _renderShader;

	Material _renderMaterial;

	Texture2D _dataBuffer1;
	Color[] _dataMatrix1;


	Mesh _srcMesh;
	Mesh _mesh;
	bool _dirty=true;

	GameObject CreateInstance(Vector3 pos,Quaternion rot)
	{
		var g = Instantiate(Prefab.gameObject,Vector3.zero,Quaternion.identity,transform) as GameObject;
		g.transform.localPosition = pos;
		g.transform.localRotation = rot;
		g.SetActive (true);

		return g;
	}

	// Use this for initialization
	void Start () {

		//generate a cube mesh
		{
			_srcMesh=MeshGenerator.GenerateBox (new Vector3 (0, 0, -0.5f));
			Prefab.GetComponent<MeshFilter> ().mesh = _srcMesh;
		}


		if (true)
			return;
		Vector3 pos=new Vector3(0,0,0);
		pos.x = -TileSize * TilesCount / 2.0f;

		float maxDistance = TilesCount * TileSize / 2.0f;

		int N = TilesCount;
		for(int i=-N/2;i<=N/2;++i)
		{
			pos.y = TileSize*i;
			for(int j=-N/2;j<=N/2;++j)
			{
				pos.x = TileSize*j;
				var g=CreateInstance (pos, Quaternion.identity);
				float dist = pos.magnitude / maxDistance;
				g.GetComponent<WallCubeBehaviour> ().Config = Config;
				g.GetComponent<WallCubeBehaviour> ().Saturation = dist*0.5f+0.4f+UnityEngine.Random.Range(0,20)/200.0f;
				_cubes.Add (g.GetComponent<WallCubeBehaviour> ());

			}
		}

	}

	void SpawnCubes()
	{
		float maxDistance = TilesCount*TileSize / 2.0f;
		foreach (var c in _cubes) {

			float dist = (c.Position-transform.position) .magnitude / maxDistance;
			c.OnSpawn (-dist);
		}

	}


	Material _createMaterial(Shader s)
	{
		Material m = new Material (s);
		m.hideFlags = HideFlags.DontSave;
		return m;
	}

	Texture2D _createBuffer(int width,int height)
	{
		Texture2D t = new Texture2D (width, height, TextureFormat.RGBAFloat,false);
		t.hideFlags = HideFlags.DontSave;
		t.filterMode = FilterMode.Point;
		t.wrapMode = TextureWrapMode.Repeat;
		return t;
	}

	Mesh _createInstancedMesh(Mesh src)
	{
		int nv = src.vertices.Length;

		int count = (TilesCount+1) * (TilesCount+1);

		var Verts = new Vector3[count*nv];
		var Normals = new Vector3[count*nv];
		var texCoords=new Vector2[count*nv];
		var srcVerts = src.vertices;
		var srcNormals = src.normals;
		var srcIndices = src.GetIndices (0);

		var Ai = 0;


		Vector3 pos=new Vector3(0,0,0);
		pos.x = -TileSize * TilesCount / 2.0f;

		float maxDistance = TilesCount * TileSize / 2.0f;

		Matrix4x4 trans = Matrix4x4.Scale(Prefab.transform.localScale) ;
		Matrix4x4 trans2 = transform.localToWorldMatrix*trans;

		int N = TilesCount;
		int cc = 0;
		Vector2 index = Vector2.zero;
		for(int y=-N/2;y<=N/2;++y)
		{
			index.x = 0;
			pos.y = y;
			for(int x=-N/2;x<=N/2;++x)
			{
				pos.x = x;
				var c = new WallCubeBehaviour ();
				c.Position = trans2.MultiplyPoint(pos);
				c.BasePosition = pos;
				c.Config = Config;
				float dist = pos.magnitude / maxDistance;
				c.Saturation = dist*0.5f+0.4f+UnityEngine.Random.Range(0,20)/200.0f;
				c.Index=(int)(index.y*(TilesCount+1)+index.x);
				_cubes.Add (c);
				Vector2 uv=new Vector2((float)y/(float)(TilesCount+1),(float)x/(float)(TilesCount+1));
				for (int i = 0; i < srcVerts.Length; ++i) {
					Verts [Ai + i] = trans.MultiplyPoint(srcVerts[i]+pos);
					Normals [Ai + i] = trans.MultiplyVector( srcNormals [i]);
					texCoords [Ai + i] = uv;
				}
				cc++;
				Ai += srcVerts.Length;
				index.x ++;
			}
			index.y++;
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
		mesh.SetIndices (Indicies,src.GetTopology(0),0);
		mesh.RecalculateBounds ();
		mesh.hideFlags = HideFlags.DontSave;

		return mesh;
	}

	void _applyKernelParams()
	{
	}

	void _InitMesh()
	{
		if (_mesh == null) {
			_mesh = _createInstancedMesh (_srcMesh);
			GetComponent<MeshFilter> ().mesh = _mesh;
		}

		if (_dataBuffer1)
			DestroyImmediate (_dataBuffer1);

		_dataBuffer1 = _createBuffer (TilesCount+1, TilesCount+1);

		if (!_renderShader)
			_renderShader = Shader.Find ("Hidden/WallRenderShader");
		if (!_renderMaterial)
			_renderMaterial = _createMaterial (_renderShader);
		if (_dataMatrix1==null) {
			_dataMatrix1 = new Color[(TilesCount + 1) * (TilesCount + 1)];
			for (int i = 0; i < _dataMatrix1.Length; ++i) {
				_dataMatrix1 [i] = Color.black;
			}
		}

		_renderMaterial.SetTexture("_DataTex",_dataBuffer1);
		GetComponent<MeshRenderer> ().material = _renderMaterial;

		_applyKernelParams ();


		_dirty = false;
	}

	void _updateDataMatrix()
	{
		foreach (var c in _cubes) {
			_dataMatrix1 [c.Index].r=c.Scale;
			_dataMatrix1 [c.Index].g=Config.H;
			_dataMatrix1 [c.Index].b=c.Saturation;
			_dataMatrix1 [c.Index].a=c.Lighting;
		}
		_dataBuffer1.SetPixels(_dataMatrix1);
		_dataBuffer1.Apply ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_dirty)
			_InitMesh ();

		Color.RGBToHSV (BaseColor, out Config.H, out Config.S,out  Config.V);
		foreach (var c in _cubes)
			c.Update ();

		_updateDataMatrix ();

		if (Input.GetKey (KeyCode.Space))
			SpawnCubes ();
	}
}

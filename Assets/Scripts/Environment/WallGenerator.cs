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
			return TileScale.x;
		}
	}

	public Vector3 TileScale=new Vector3(1,1,1);

	public Color BaseColor;

	public WallCubeBehaviour.WallConfigurations Config=new WallCubeBehaviour.WallConfigurations();

	List<WallCubeBehaviour> _cubes=new List<WallCubeBehaviour>();

	GPUInstancing _instanceMaker=new GPUInstancing();

	//shaders
	[SerializeField] Shader _renderShader;

	Material _renderMaterial;

	Texture2D _dataBuffer1;
	Color[] _dataMatrix1;
	Texture2D _dataBuffer2;
	Color[] _dataMatrix2;

	float _noise;
	float _noiseSpeed;

	Mesh _srcMesh;
	Mesh _mesh;
	bool _dirty=true;

	[SerializeField]bool _paused=true;
	public bool Paused
	{
		set{
			_paused = value;

			if (_paused)
				_onPaused ();
		}

		get{
			return _paused;
		}
	}

	// Use this for initialization
	void Start () {
		_noise = UnityEngine.Random.value;
		_noiseSpeed= UnityEngine.Random.value*0.1f+0.05f;
	}

	public void SpawnCubes()
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

	void _OnMeshInstanced(int i)
	{
	}

	Mesh _createInstancedMesh(Mesh src)
	{
		_instanceMaker.srcMesh = src;
		_instanceMaker.InstancesCount = (TilesCount + 1) * (TilesCount + 1);

		_instanceMaker.InstancesPosition = new Vector3[_instanceMaker.InstancesCount];
		_instanceMaker.OnMeshInstanced = _OnMeshInstanced;

		Vector3 pos=new Vector3(0,0,0);
		pos.x = -TileSize * TilesCount / 2.0f;


		Matrix4x4 trans = Matrix4x4.Scale(TileScale) ;
		Matrix4x4 trans2 = transform.localToWorldMatrix*trans;

		_instanceMaker.TransformationMatrix = trans;

		float maxDistance = TilesCount  / 2.0f;

		int N = TilesCount;
		for(int y=-N/2;y<=N/2;++y)
		{
			pos.y = y;
			for(int x=-N/2;x<=N/2;++x)
			{
				pos.x = x;
				var c = new WallCubeBehaviour ();
				c.Position = trans2.MultiplyPoint(pos);
				c.BasePosition = pos;
				c.Config = Config;
				float dist = (pos).magnitude / maxDistance;
				c.Saturation = dist*0.5f+0.4f+UnityEngine.Random.Range(0,20)/200.0f;
				c.Index=_cubes.Count;
				_instanceMaker.InstancesPosition [c.Index] = pos;
				_cubes.Add (c);
			}
		}
		return _instanceMaker.CreateInstanceMesh();
	}


	void _InitMesh()
	{
		if (_srcMesh == null) {
			_srcMesh = MeshGenerator.GenerateBox (new Vector3 (0, 0, -0.5f));
		}

		if (_mesh == null) {
			_mesh = _createInstancedMesh (_srcMesh);
			GetComponent<MeshFilter> ().mesh = _mesh;
		}

		if (!_renderShader)
			_renderShader = Shader.Find ("Hidden/WallRenderShader");
		if (!_renderMaterial)
			_renderMaterial = _createMaterial (_renderShader);
		if (!_dataBuffer1)
		{
			_dataBuffer1 = _instanceMaker.CreateDataTexture (false) as Texture2D;
			//_dataBuffer2 = _instanceMaker.CreateDataTexture (false) as Texture2D;
		
			_dataMatrix1 = new Color[_instanceMaker.InstancesCount];
			//_dataMatrix2 = new Color[_instanceMaker.InstancesCount];
			for (int i = 0; i < _dataMatrix1.Length; ++i) {
				_dataMatrix1 [i] = Color.black;
			//	_dataMatrix1 [i] = Color.black;
			}
		}

		_renderMaterial.SetTexture("_DataTex1",_dataBuffer1);
		//_renderMaterial.SetTexture("_DataTex2",_dataBuffer2);
		GetComponent<MeshRenderer> ().material = _renderMaterial;


		_dirty = false;
	}

	void _updateDataMatrix()
	{
		foreach (var c in _cubes) {
			_dataMatrix1 [c.Index].r=c.Scale;
			_dataMatrix1 [c.Index].g=(Config.H+ c.affectorHue)/2;
			_dataMatrix1 [c.Index].b=c.Saturation;
			_dataMatrix1 [c.Index].a=c.Lighting;
		}
		_dataBuffer1.SetPixels(_dataMatrix1);
		_dataBuffer1.Apply ();
	}

	void _onPaused()
	{
		foreach (var c in _cubes) {
			_dataMatrix1 [c.Index].r=0;
			_dataMatrix1 [c.Index].g=0;
			_dataMatrix1 [c.Index].b=0;
			_dataMatrix1 [c.Index].a=0;
		}
		_dataBuffer1.SetPixels(_dataMatrix1);
		_dataBuffer1.Apply ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_dirty)
			_InitMesh ();

		if (Paused)
			return;

		//Color.RGBToHSV (BaseColor, out Config.H, out Config.S,out  Config.V);
		Config.H=Reaktion.Perlin.Noise(_noise);
		_noise += Time.deltaTime*_noiseSpeed;
		BaseColor= Color.HSVToRGB(Config.H,Config.S,Config.V);
		foreach (var c in _cubes)
			c.Update ();

		_updateDataMatrix ();


	}
}

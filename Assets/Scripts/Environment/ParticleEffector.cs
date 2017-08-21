using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffector : MonoBehaviour {


	public Mesh ParticleMesh;

	GPUInstancing _instanceMaker=new GPUInstancing();

	//shaders
	public Shader RenderShader;

	Material _renderMaterial;

	public Texture2D _dataBuffer1;
	Color[] _dataMatrix1;

	public Texture2D _dataBuffer2;
	Color[] _dataMatrix2;

	public Mesh _mesh;
	bool _dirty=true;

	public float GenerateSpeed;
	float _gentime;

	public int MaxCount;

	public float MinLifespan;
	public float MaxLifespan;

	public Vector3 acceleration;

	public Vector3 MinVolume;
	public Vector3 MaxVolume;

	int _activeParticlesCount;

	public Color BaseColor;

	class Particle
	{
		public Vector3 pos;
		public Vector3 vel;
		public Vector3 accel;
		public float scale;
		public float lifespan;
		public Quaternion rot;
		public Quaternion randRot;

		public bool Update()
		{
			if (lifespan < 0)
				return false;
			pos += vel * Time.deltaTime;
			vel += accel * Time.deltaTime;
			rot =  randRot * rot;
			lifespan -= Time.deltaTime;

			var attractors=AttractorManager.Instance.CalculateAttractionVectors (pos);

			foreach (var a in attractors) {
				float len = a.sqrMagnitude;
				if (len < 1) {
					len = Mathf.Sqrt (len);

					pos -= 0.5f * a * Time.deltaTime/len;
				}
			}

			return true;
		}
	}

	Particle[] _particles;

	// Use this for initialization
	void Start () {
		
	}

	Material _createMaterial(Shader s)
	{
		Material m = new Material (s);
		m.hideFlags = HideFlags.DontSave;
		return m;
	}
	Mesh _createInstancedMesh()
	{
		_instanceMaker.srcMesh = ParticleMesh;
		_instanceMaker.InstancesCount = MaxCount;

		_particles = new Particle[MaxCount];
		for (int i = 0; i < MaxCount; ++i) {
			_particles [i] = new Particle ();
		}

		return _instanceMaker.CreateInstanceMesh();
	}


	void _InitMesh()
	{

		if (_mesh == null) {
			_mesh = _createInstancedMesh ();
		//	GetComponent<MeshFilter> ().mesh = _mesh;
		}


		if (!_renderMaterial)
			_renderMaterial = _createMaterial (RenderShader);
		if (!_dataBuffer1){
			_dataBuffer1 = _instanceMaker.CreateDataTexture (false) as Texture2D;
			_dataBuffer2 = _instanceMaker.CreateDataTexture (false) as Texture2D;
			_dataMatrix1 = new Color[_instanceMaker.InstancesCount];
			_dataMatrix2 = new Color[_instanceMaker.InstancesCount];

			_activeParticlesCount = 0;
			for (int i = 0; i < _dataMatrix1.Length; ++i) {
				_generateParticle ();
				_dataMatrix1 [i] = Color.black;
				_dataMatrix2 [i] = Color.black;
			}
		}

		_renderMaterial.SetTexture("_DataTex1",_dataBuffer1);
		_renderMaterial.SetTexture("_DataTex2",_dataBuffer2);
		//GetComponent<MeshRenderer> ().material = _renderMaterial;


		_dirty = false;
	}


	void _setupParticle(Particle p)
	{
		p.lifespan = Random.Range (MinLifespan, MaxLifespan);
		p.pos =  MinVolume;
		p.pos.x+=Random.value*(MaxVolume.x-MinVolume.x);
		p.pos.y+=Random.value*(MaxVolume.y-MinVolume.y);
		p.pos.z+=Random.value*(MaxVolume.z-MinVolume.z);

		p.vel = Vector3.zero;
		p.accel = (new Vector3(acceleration.x*Random.value,acceleration.y*Random.value, acceleration.z*Random.value));
		p.rot = Random.rotation;
		p.randRot = Quaternion.AngleAxis(Random.value*10,Random.onUnitSphere);

		p.scale = Random.value*0.5f+0.5f;

		p.pos = transform.localToWorldMatrix.MultiplyPoint (p.pos);
	}
	bool _generateParticle()
	{
		if (_activeParticlesCount == _particles.Length)
			return false;
		_setupParticle (_particles [_activeParticlesCount]);
		_activeParticlesCount++;
		return true;
	
	}
	void _updateDataMatrix()
	{
		for (int i = 0; i < _activeParticlesCount; ++i) {
			_dataMatrix1 [i].r = _particles [i].pos.x;
			_dataMatrix1 [i].g = _particles [i].pos.y;
			_dataMatrix1 [i].b = _particles [i].pos.z;
			_dataMatrix1 [i].a = _particles [i].scale;

			_dataMatrix2 [i].r = _particles [i].rot.x;
			_dataMatrix2 [i].g = _particles [i].rot.y;
			_dataMatrix2 [i].b = _particles [i].rot.z;
			_dataMatrix2 [i].a = _particles [i].lifespan/MaxLifespan;
		}
		_dataBuffer1.SetPixels(_dataMatrix1);
		_dataBuffer2.SetPixels(_dataMatrix2);
		_dataBuffer1.Apply ();
		_dataBuffer2.Apply ();
	}

	void _updateParticles()
	{

		_gentime -= Time.deltaTime;
		if (_gentime < 0) {
			//	_generateParticle ();
			_gentime = GenerateSpeed;
		}

		for (int i = 0; i < _activeParticlesCount; ++i) {
			if (!_particles [i].Update ()) {
				_setupParticle (_particles [i]);
			} 
		}

		float h, s, v;

		var g=GetComponent<WallGenerator> ();
		if (g)
			BaseColor = g.BaseColor;
		Color.RGBToHSV (BaseColor, out h, out s, out v);
		_renderMaterial.SetFloat ("_Hue", h);
		_renderMaterial.SetFloat ("_Saturation", s);
	}
	// Update is called once per frame
	void Update () {
		if (_dirty)
			_InitMesh ();
		
		_updateParticles ();
		_updateDataMatrix ();

		Graphics.DrawMesh (_mesh, Matrix4x4.identity, _renderMaterial,0);

	}


	void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube ((MaxVolume + MinVolume)/2, (MaxVolume - MinVolume));
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Flask;


[RequireComponent(typeof(Renderer))]
public class WallCubeBehaviour :IBehaviour {


	[Serializable]
	public class WallConfigurations
	{
		[HideInInspector]
		public float H,S,V;

		public float spawnSpeed = 1.0f;

		public AnimationCurve SpawnAnimation=AnimationCurve.Linear(0,0,1,1);
		public AnimationCurve ClickAnimation=AnimationCurve.Linear(0,0,1,1);
	}

	WallConfigurations _config;
	public WallConfigurations Config
	{
		set{_config=value;}
		get{return _config;}
	}


	public float Saturation=1.0f;
	float Lighting=0.0f;

	float _spawnTime=0;

	public float Attraction;
	DTween _spring=new DTween(0,15f);

	Vector3 _originalScale;

	enum EState
	{
		Idle,
		Spawning,
		Active
	}
	EState _state=EState.Idle;

	Renderer _renderer;
	// Use this for initialization
	void Start () {

		_originalScale = transform.localScale;

		_renderer = GetComponent<Renderer> ();

		_spring.omega = 2f + 20*Saturation;

	}


	public void OnSpawn(float spawnTime)
	{
		_state = EState.Spawning;
		_spawnTime = spawnTime;
	}

	void _process()
	{
		switch (_state) {
		case EState.Idle:
			break;
		case EState.Spawning:
			{
				if (_spawnTime >= 0) {
					float spawn = Config.SpawnAnimation.Evaluate (_spawnTime);
					transform.localScale = Vector3.Scale (_originalScale, new Vector3 (1, 1, spawn));
					Lighting = spawn;
				}
				_spawnTime += Time.deltaTime*Config.spawnSpeed;
				if (_spawnTime > 1)
					_state = EState.Active;

			}
			break;
		case EState.Active:
			{
				float a=AttractorManager.Instance.CalculateAttraction (transform.position);
				Attraction=_spring.Step (a);


				float strength = Config.ClickAnimation.Evaluate (Attraction);
				transform.localScale = Vector3.Scale (_originalScale, new Vector3 (1, 1, strength));
				Lighting = strength;

			}
			break;
		}
		_renderer.material.color = Color.HSVToRGB (Config.H, Saturation, Lighting);
	}

	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();

		_process ();
	}
}

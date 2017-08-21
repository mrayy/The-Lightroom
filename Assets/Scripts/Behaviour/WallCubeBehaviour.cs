using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Flask;


public class WallCubeBehaviour {


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
	public float Lighting=0.0f;

	float _spawnTime=0;

	public float Attraction;
	DTween _spring=new DTween(0,15f);


	public int Index=0;
	public Vector3 Position;
	public Vector3 BasePosition;
	public float Scale=0;

	enum EState
	{
		Idle,
		Spawning,
		Active
	}
	EState _state=EState.Idle;

	public WallCubeBehaviour () {
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
					Scale=spawn;
					Lighting = spawn;
				}
				_spawnTime += Time.deltaTime*Config.spawnSpeed;
				if (_spawnTime > 1)
					_state = EState.Active;

			}
			break;
		case EState.Active:
			{
				float a=AttractorManager.Instance.CalculateAttraction (Position);
				Attraction=_spring.Step (a);


				float strength = Config.ClickAnimation.Evaluate (Attraction);
				Scale = strength;
				Lighting = strength;

			}
			break;
		}
		//_renderer.material.color = Color.HSVToRGB (Config.H, Saturation, Lighting);
	}

	// Update is called once per frame
	public void Update () {
		_process ();
	}
}

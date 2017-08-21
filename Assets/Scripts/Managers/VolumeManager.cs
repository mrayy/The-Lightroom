using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager: MonoBehaviour {

	static VolumeManager _instance;

	public static VolumeManager Instance
	{
		get{
			return _instance;
		}
	}

	VolumeManager()
	{
		_instance = this;
	}

	public float AudioOmega=60.0f;
	public float TargetVolume=0.5f;
	public float Volume;
	public float Speed=4.0f;
	public float Weight=5.0f;

	float amplifier=1.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		float target = Mathf.Clamp01 (Weight / (3 + Weight));
		amplifier = Mathf.Exp (-Speed * Time.deltaTime) * (amplifier - target) + target;

		Volume = TargetVolume * amplifier;
	}
}

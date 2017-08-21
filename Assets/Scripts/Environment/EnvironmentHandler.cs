using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentHandler : MonoBehaviour {


	WallGenerator[] _walls;
	ParticleEffector[] _particles;

	// Use this for initialization
	void Start () {
		_walls = GameObject.FindObjectsOfType<WallGenerator> ();
		_particles = GameObject.FindObjectsOfType<ParticleEffector> ();
	}

	public void Reset()
	{
		foreach (var w in _walls) {
			w.Paused = true;
		}
		foreach (var e in _particles) {
			e.Paused = true;
		}
	}

	public void StartEnvironment()
	{
		foreach (var w in _walls) {
			w.Paused = false;
			w.SpawnCubes ();
		}
		foreach (var e in _particles) {
			e.Paused = false;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}

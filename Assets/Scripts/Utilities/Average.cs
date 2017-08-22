using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageF {
	
	float[] _samples;
	int _current;
	float _val = 0;

	public float Value {
		get{ return _val; }
	}


	// Use this for initialization
	public AverageF (int count) {
		_samples = new float[count];
	}
	
	public float AddSample(float v){
		_val = 0;
		_samples [_current] = v;
		_current = (_current + 1) % _samples.Length;
		for (int i = 0; i < _samples.Length; ++i)
			_val += _samples [_current];

		_val=_val/ (float)_samples.Length;
		return _val;
	}
}

public class Average2 {

	Vector2[] _samples;
	int _current;
	Vector2 _val = Vector2.zero;

	public Vector2 Value {
		get{ return _val; }
	}


	// Use this for initialization
	public Average2 (int count) {
		_samples = new Vector2[count];
	}

	public Vector2 AddSample(Vector2 v){
		_val = Vector2.zero;
		_samples [_current] = v;
		_current = (_current + 1) % _samples.Length;
		for (int i = 0; i < _samples.Length; ++i)
			_val += _samples [_current];

		_val=_val/ (float)_samples.Length;
		return _val;
	}
}
public class Average3 {

	Vector3[] _samples;
	int _current;
	Vector3 _val = Vector3.zero;

	public Vector3 Value {
		get{ return _val; }
	}

	// Use this for initialization
	public Average3 (int count) {
		_samples = new Vector3[count];
	}

	public Vector3 AddSample(Vector3 v){
		_val = Vector3.zero;
		_samples [_current] = v;
		_current = (_current + 1) % _samples.Length;
		for (int i = 0; i < _samples.Length; ++i)
			_val += _samples [_current];

		_val=_val/ (float)_samples.Length;
		return _val;
	}
}

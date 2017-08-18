using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : IAudioGenerator {

	float Multiplier=16.0f;
	float Modulation=0.005f;

	float _mx=0;
	float _cx=0;
	float _step=0;


	public float Note;
	public int _samplesCount;
	/*
	public float CurrentSample;

	// Use this for initialization
	void Start () {
	}*/

	public int SetNote(float note)
	{
		Note = note;
		float f = 440.0f * Mathf.Pow (2.0f, (note - 69) / 12.0f);
		_step = f/AudioSettings.outputSampleRate;
		_samplesCount= (int)(1.0f / _step);

		return _samplesCount;
	}

	public int SamplesCount()
	{
		return _samplesCount;
	}

	public float Sample()
	{
		_mx += _step * Multiplier;
		_cx += _step;

		_mx -= Mathf.Floor (_mx);
		_cx -= Mathf.Floor (_cx);

		float x = _cx + Modulation * Mathf.Sin (_mx * Mathf.PI * 2.0f);
		x -= Mathf.Floor (x);

		return Mathf.Sin(x* Mathf.PI * 2.0f);
	}

}

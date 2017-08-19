using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource),typeof(ElementBehaviour))]
public class AudioBehaviour : IBehaviour {

	static float[][] _Scales = new float[][]
	{
		new float[]{0,4,5,7,11},
		new float[]{0,2,3,4,7,9},
		new float[]{0,2,4,7,9,11},
	};
	public int ScaleIndex=1;
	public int BaseNote=69;

	IAudioGenerator _generator;
	public int Index;
	public float Note;
	float[] _samples;

	float _decay=1.0f;

	ElementBehaviour _baseBehaviour;

	AudioSource _source;

	// Use this for initialization
	void Start () {

		var gen = new Oscillator ();

		float[] intervals = _Scales [ScaleIndex];
		float interval = intervals [Index % intervals.Length];
		float octave = (float)Index / (float)intervals.Length;
		Note=BaseNote+octave*12+interval;

		gen.SetNote (Note);

		_generator = gen;

		_samples = new float[_generator.SamplesCount ()];
		for (int i = 0; i < _samples.Length; ++i)
			_samples [i] = _generator.Sample ();

		_source=GetComponent<AudioSource> ();
		_source.loop = true;
		_source.clip = AudioClip.Create ("", _samples .Length, 1, AudioSettings.outputSampleRate, false);
		_source.clip.SetData (_samples, 0);
		_source.Play ();

		_baseBehaviour = GetComponent<ElementBehaviour> ();

	}
	protected override void StopBehaviour () {
		base.StopBehaviour ();
		_source.Stop ();
	}
	protected override void StartBehaviour () {
		base.StartBehaviour ();
		_source.Play ();
	}

	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();


		//update audio volume
		_source.volume = VolumeManager.Instance.Volume * 0.5f*_decay*(1.0f+Mathf.Cos(_baseBehaviour.Theta));
	}
}
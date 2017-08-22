using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSpectrum))]
public class AudioListenerBehaviour :IBehaviour {

	AudioSpectrum _audioSpec;
	Vector3 _baseScale;

	Material _mtrl;

	public int BaseBand=0;
	public float peak;

	// Use this for initialization
	void Start () {
		_audioSpec = GetComponent<AudioSpectrum> ();
		_baseScale = transform.localScale;

		_mtrl = GetComponent<Renderer> ().material;
	}
	public override void StopBehaviour(){
		base.StopBehaviour ();
	}
	public override void StartBehaviour(){
		base.StartBehaviour ();
	}


	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();
		peak = _audioSpec.PeakLevels [BaseBand]*10;
		_mtrl.SetFloat ("_Exp",peak);
		this.transform.localScale = _baseScale+ new Vector3 (_audioSpec.MeanLevels[BaseBand],_audioSpec.MeanLevels[BaseBand],_audioSpec.MeanLevels[BaseBand]);
	}
}

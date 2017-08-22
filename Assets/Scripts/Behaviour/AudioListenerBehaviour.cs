using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSpectrum))]
public class AudioListenerBehaviour :IBehaviour {

	AudioSpectrum _audioSpec;
	Vector3 _baseScale;

	public int BaseBand=0;

	// Use this for initialization
	void Start () {
		_audioSpec = GetComponent<AudioSpectrum> ();
		_baseScale = transform.localScale;
	}


	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();


		this.transform.localScale = _baseScale+ new Vector3 (_audioSpec.MeanLevels[BaseBand],_audioSpec.MeanLevels[BaseBand],_audioSpec.MeanLevels[BaseBand]);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flask;

public class AttractorBehaviour : IBehaviour {

	public Color BaseColor;

	DTween _colorTween=new DTween(0,0.5f);
	DTween _scaleTween=new DTween(0,1f);

	AudioBehaviour _audio;
	Renderer _renderer;

	public bool selected=false;

	float V=0.7f;
	bool _destroyed=false;

	DTween _decayTween=new DTween(1,1);
	// Use this for initialization
	void Start () {
		_renderer=GetComponent<Renderer> ();
		_audio=GetComponent<AudioBehaviour> ();
	}

	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();
		_colorTween.Step ();
		if (Mathf.Abs (_colorTween.position - _colorTween.target) < 0.01f) {
			_colorTween.target = Random.value;
		}
		_decayTween.Step ();
		_audio._decay = _decayTween.position;
		if (!_destroyed) {
			if (selected) {
				_scaleTween.target = 1.3f;
				_colorTween.omega = 5;
			} else {
				_scaleTween.target = 1f;
				_colorTween.omega = 0.5f;
			}
		}
		_renderer.material.color = BaseColor= Color.HSVToRGB (_colorTween.position, 1,V*_decayTween.position);
		_scaleTween.Step ();
		transform.localScale = new Vector3 (_scaleTween.position,_scaleTween.position,_scaleTween.position);

		if (_destroyed && _scaleTween.position < 0.05f && _decayTween.position<0.05f) {
			GameObject.Destroy (gameObject);
		}
	}

	public void Destroy()
	{
		_destroyed = true;
		_decayTween.target = 0;
		_scaleTween.target = 0f;
		_colorTween.omega = 2f;
	}

}

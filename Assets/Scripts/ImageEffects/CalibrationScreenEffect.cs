using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CalibrationScreenEffect : MonoBehaviour {
	Flask.DTween _flashTween=new Flask.DTween(0,10);
	Flask.DTween _sizeTween=new Flask.DTween(0,5f);
	Flask.DTweenVector2 _positionTween=new Flask.DTweenVector2(new Vector2(0,0),10);
	Flask.DTween _fadeTween=new Flask.DTween(0,10f);

	Vector2 _targetPosition;

	public float TargetRepeat=10;
	public float Repeat=10;
	public float MaxGazeSize=0.2f;
	public Vector4 pos;

	public RawImage target;
	public Texture BGTexture;

	OffscreenProcessor _renderer;

	enum EStatus
	{
		Idle,
		FadeIn,
		Calibrate,
		FadeOut,
	}

	EStatus _status=EStatus.Idle;

	float _beatingSpeed=1.5f;
	Coroutine _hearBeatCor;
	// Use this for initialization
	void Awake () {
	}

	void Start()
	{
		_flashTween.target = 0;
		_sizeTween.target = 1;
		_renderer = new OffscreenProcessor ();
		_renderer.ShaderName = "Hidden/CalibrationScreenShader";

		_hearBeatCor=StartCoroutine (HeartBeat (_sizeTween));

		//if(_hearBeatCor!=null)
		//	StopCoroutine (_hearBeatCor);
	}

	// Update is called once per frame
	void Update () {
		_flashTween.Step ();
		_sizeTween.Step ();
		_positionTween.Step ();
		_fadeTween.Step ();
		Repeat = TargetRepeat*_fadeTween.position;
		RenderImage ();
	}

	IEnumerator HeartBeat(Flask.DTween t)
	{
		while(true) {
			t.position = 1.5f;
			t.target = 1;
			yield return new WaitForSeconds (_beatingSpeed);
		}

	}

	public void SetPosition(Vector2 pos,bool trigger)
	{
		_targetPosition = pos;
		_positionTween.target = _targetPosition;
		if (trigger) {
			_beatingSpeed = 1;
		} else {
			_beatingSpeed = 2;
		}
		this.enabled = true;
	}
	public void SetTriggered()
	{
		_flashTween.position = 1;
		this.enabled = true;
	}
	// Postprocess the image
	void RenderImage ()
	{

		pos = new Vector4 (_positionTween.position.x, _positionTween.position.y, _flashTween.position, _sizeTween.position*MaxGazeSize);
		if (_renderer.ProcessingMaterial != null) {
			_renderer.ProcessingMaterial.SetFloat ("_Repeat", Repeat);
			_renderer.ProcessingMaterial.SetVector ("_Position", pos);
			_renderer.ProcessingMaterial.SetTexture ("_BGTex", BGTexture);
		}
		target.texture= _renderer.ProcessTexture (new Vector2(Screen.width,Screen.height));

		//if (_flashTween.position == 0)
		//	this.enabled = false;
	}

	public void Restart()
	{
		_fadeTween.position = 0;
		_fadeTween.target = 1;
	}
	public void OnDone()
	{
		_fadeTween.position = 1;
		_fadeTween.target = 2;
	}
}
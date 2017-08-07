using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CalibrationScreenEffect : MonoBehaviour {
	private Material material;

	Flask.DTween _flashTween=new Flask.DTween(0,10);
	Flask.DTween _sizeTween=new Flask.DTween(0,5f);
	Flask.DTweenVector2 _positionTween=new Flask.DTweenVector2(new Vector2(0,0),10);

	Vector2 _targetPosition;

	public float Repeat=10;
	public float MaxGazeSize=0.2f;
	public Vector4 pos;

	public RawImage target;

	OffscreenProcessor _renderer;
	// Use this for initialization
	void Awake () {
	}

	void Start()
	{
		_renderer = new OffscreenProcessor ();
		_renderer.ShaderName = "Hidden/CalibrationScreenShader";
	}

	// Update is called once per frame
	void Update () {
		_flashTween.Step (0);
		_sizeTween.Step (1);
		_positionTween.Step (_targetPosition);
		RenderImage ();
	}

	public void SetPosition(Vector2 pos,bool trigger)
	{
		_targetPosition = pos;
		if(trigger)
			_sizeTween.position = 2;
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
		if (material == null)
		{
			material = new Material(Shader.Find("Hidden/CalibrationScreenShader") );
			material.hideFlags = HideFlags.DontSave;
		}

		pos = new Vector4 (_positionTween.position.x, _positionTween.position.y, _flashTween.position, _sizeTween.position*MaxGazeSize);
		if (_renderer.ProcessingMaterial != null) {
			_renderer.ProcessingMaterial.SetFloat ("_Repeat", Repeat);
			_renderer.ProcessingMaterial.SetVector ("_Position", pos);
		}
		target.texture= _renderer.ProcessTexture (new Vector2(Screen.width,Screen.height));

		//if (_flashTween.position == 0)
		//	this.enabled = false;
	}
}
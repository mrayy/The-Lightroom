using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CalibrationScreenEffect : MonoBehaviour {
	private Material material;

	Flask.DTween _flashTween=new Flask.DTween(0,10);
	Flask.DTween _sizeTween=new Flask.DTween(0,5f);
	Flask.DTweenVector2 _positionTween=new Flask.DTweenVector2(new Vector2(0,0),10);

	public RenderTexture screen;

	Vector2 _targetPosition;

	public float Repeat=10;
	public float MaxGazeSize=0.2f;
	public Vector4 pos;
	// Use this for initialization
	void Awake () {
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update () {
		_flashTween.Step (0);
		_sizeTween.Step (1);
		_positionTween.Step (_targetPosition);

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
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (material == null)
		{
			material = new Material(Shader.Find("Hidden/CalibrationScreenShader") );
			material.hideFlags = HideFlags.DontSave;
		}

		pos = new Vector4 (_positionTween.position.x, _positionTween.position.y, _flashTween.position, _sizeTween.position*MaxGazeSize);

		material.SetFloat ("_Repeat", Repeat);
		material.SetVector ("_Position", pos);

		screen = source;
		Graphics.Blit (source, destination, material);

		//if (_flashTween.position == 0)
		//	this.enabled = false;
	}
}
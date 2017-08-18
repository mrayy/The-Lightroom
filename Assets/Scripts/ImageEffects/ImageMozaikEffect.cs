using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageMozaikEffect : MonoBehaviour {

	public VideoCapture BGTexture;

	public MeshFilter target;
	public MeshRenderer targetMtrl;

	public int Repeat=40;

	void Start()
	{

		target.mesh = MeshGenerator.GeneratePlane (1, 1, Repeat, Repeat);
		targetMtrl.material = new Material (Shader.Find ("Image/ImageMozaik"));
	}

	// Update is called once per frame
	void Update () {
		RenderImage ();
	}

	// Postprocess the image
	void RenderImage ()
	{
		if (targetMtrl.material != null) {
			targetMtrl.material.SetFloat ("_Repeat", Repeat);
			targetMtrl.material.SetTexture ("MainTex", BGTexture.BlitImage);
			targetMtrl.material.mainTexture = BGTexture.BlitImage;
		}

		//if (_flashTween.position == 0)
		//	this.enabled = false;
	}

}
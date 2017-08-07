using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlurImageBypass : MonoBehaviour {

	public float intensity;
	public int Iterations=2;
	public int Downscale=1;
	BlurImageGenerator _blurGenerator;

	public Texture BluredTexture;

	Texture[] _blurTex;
	// Use this for initialization
	void Awake () {

	}

	void Start()
	{
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (_blurGenerator == null) {
			_blurGenerator = new BlurImageGenerator ();
		}
		_blurGenerator.Iterations = Iterations;
		_blurGenerator.DownScaler = Downscale;
		BluredTexture=_blurGenerator.GenerateBlur (source,ref _blurTex);

		Graphics.Blit (BluredTexture, destination);

	}
}

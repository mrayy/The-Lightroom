using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BlurImageBypass))]
public class GazeShader : MonoBehaviour {

	public float intensity;
	public float dimAmount;

	struct GazePoint
	{
		public Vector2 Position;
		public Vector2 Radius;
	}
	List<GazePoint> _points=new List<GazePoint>();


	private Material material;
	private Material materialMask;

	RenderTexture _maskTexture;
	BlurImageBypass _blurImage;
	// Use this for initialization
	void Awake () {
		_blurImage = GetComponent<BlurImageBypass> ();
	}


	public void AddGazePoint(Vector2 p,Vector2 r)
	{
		GazePoint g = new GazePoint ();
		g.Position = p;
		g.Radius = r;
		_points.Add (g);
	}
	public void Clear()
	{
		_points.Clear ();
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
		if (material == null)
		{
			material = new Material(Shader.Find("Hidden/GazeShader") );
			material.hideFlags = HideFlags.DontSave;

			materialMask=new Material(Shader.Find("Hidden/GazeMaskShader") );
			materialMask.hideFlags = HideFlags.DontSave;

		}
		if(_maskTexture==null || source.width!=_maskTexture.width || source.height!=_maskTexture.height)
			_maskTexture = new RenderTexture (source.width, source.height, 16);

		//Create Mask for all gaze points
		RenderTexture.active = _maskTexture;
		GL.Clear (true,true,Color.white);
		materialMask.SetVector ("_ScreenSize", SettingsHolder.Instance.GetComponent<Camera>().pixelRect.size);
		foreach (var g in _points) {
			materialMask.SetVector ("_Parameters", new Vector4 (g.Position.x,g.Position.y, g.Radius.x,g.Radius.y));

			Graphics.Blit ( _maskTexture, materialMask);
		}
		RenderTexture.active = null;

		//blit the gaze frame
		Texture tex=_blurImage.BluredTexture;
		material.SetTexture ("_BlurTex", tex);
		material.SetFloat ("_Intensity", intensity);
		material.SetFloat ("_DimAmount", dimAmount);

		material.SetTexture ("_MaskTex", _maskTexture);

		Graphics.Blit (source, destination, material);
	}
}

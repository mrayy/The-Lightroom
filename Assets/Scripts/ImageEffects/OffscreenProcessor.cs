using UnityEngine;
using System.Collections;

public class OffscreenProcessor  {

	Material _ProcessingMaterial;
	public RenderTexture ResultTexture{
		get{
			return _RenderTexture;
		}
	}
	public Material ProcessingMaterial {
		get{ return _ProcessingMaterial; }
	}
	RenderTexture _RenderTexture;

	public RenderTextureFormat TargetFormat=RenderTextureFormat.ARGB32;

	public string ShaderName
	{
		set{
			ProcessingShader=Shader.Find(value);
		}
	}
	public Shader ProcessingShader
	{
		set{
			_ProcessingMaterial=new Material(value);
		}
		get{
			return _ProcessingMaterial.shader;
		}
	}

	public OffscreenProcessor()
	{
		_ProcessingMaterial = null;//new Material("");
		TargetFormat = RenderTextureFormat.Default;
	}
	void _Setup(Vector2 size,int downSample)
	{
		int width = (int)(size.x/(downSample+1));
			int height = (int)(size.y/(downSample+1));
		if (_RenderTexture == null) {
			_RenderTexture = new RenderTexture (width, height,16, TargetFormat);
		} else if (	_RenderTexture.width != width || 
			_RenderTexture.height != height) 
		{
			_RenderTexture = new RenderTexture (width, height,16, TargetFormat);
		}
		_RenderTexture.wrapMode = TextureWrapMode.Clamp;
	}
	public Texture ProcessTexture(Texture InputTexture,int pass=0,int downSample=0)
	{
		if (InputTexture==null || InputTexture.width == 0 || InputTexture.height == 0)
			return InputTexture;
		_Setup (InputTexture.texelSize,downSample);
		ProcessingMaterial.mainTexture = InputTexture;
		RenderTexture old = RenderTexture.active;
		RenderTexture.active = _RenderTexture;
		GL.Clear (true,true,Color.black);
		Graphics.Blit (InputTexture,_RenderTexture, ProcessingMaterial,pass);
		RenderTexture.active = old;
		return _RenderTexture;

	}

	public Texture ProcessTexture(Vector2 targetSize,int pass=0,int downSample=0)
	{
		if (targetSize.x == 0 || targetSize.y == 0)
			return null;
		_Setup (targetSize,downSample);
		ProcessingMaterial.mainTexture = null;
		RenderTexture old = RenderTexture.active;
		RenderTexture.active = _RenderTexture;
		GL.Clear (true,true,Color.black);
		Graphics.Blit (_RenderTexture,_RenderTexture, ProcessingMaterial,pass);
		RenderTexture.active = old;
		return _RenderTexture;

	}
}

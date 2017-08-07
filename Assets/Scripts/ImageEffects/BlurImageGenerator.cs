using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurImageGenerator {


	public int Iterations=2;
	public int DownScaler=1;

	OffscreenProcessor[] _Blur;

	Texture _resultTex;

	void _Recreate()
	{
		if (_Blur!=null && _Blur.Length == Iterations)
			return;
		_Blur=new OffscreenProcessor[Iterations];
		for (int i = 0; i < _Blur.Length; ++i) {
			_Blur [i] = new OffscreenProcessor ();
			_Blur [i].ShaderName = "Image/ImageBlur";
		}
	}

	public BlurImageGenerator()
	{
		_Recreate();
	}

	public Texture GenerateBlur(Texture tex,ref Texture[] result)
	{

		_Recreate ();
		if (result == null || result.Length != _Blur.Length)
			result = new Texture[_Blur.Length];
		for (int i = 0; i < _Blur.Length; ++i) {
			tex = _Blur [i].ProcessTexture (tex, -1, DownScaler);
			result [i] = tex;
		}
		_resultTex = tex;
		return _resultTex;
	}
}

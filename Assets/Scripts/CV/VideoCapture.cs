using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;

public class VideoCapture : MonoBehaviour {

	public int index;

	UnityOpenCVVideoCaptureAPI _capDev;
	UnityOpenCVFaceDetectorAPI _faceDetector;
	GstImageInfo _image;
	public Texture2D BlitImage;

	public RawImage target;

	Thread _imageGrabber;
	bool _isDone=false;
	bool _captured=false;
	public int Margin=50;

	// Use this for initialization
	void Start () {
		_capDev = new UnityOpenCVVideoCaptureAPI ();
		_faceDetector = new UnityOpenCVFaceDetectorAPI (Application.dataPath+"/Data/openCV/cascades/haarcascade_frontalface_default.xml",0.5f,1.01f,8,0.3f,0.6f);
		_faceDetector.BindCamera (_capDev);
		 _image=new GstImageInfo();

		_capDev.Open (index);

		BlitImage = new Texture2D (1, 1);
		_image.Create (1, 1, GstImageInfo.EPixelFormat.EPixel_R8G8B8);


		_imageGrabber = new Thread (new ThreadStart (ImageGrabberThread));
		_imageGrabber.Start ();
	}

	Rect _face;
	void ImageGrabberThread()
	{
		Vector2 sz;
		int c;
		while (!_isDone) {
			if (!_captured) {
				//_capDev.Capture ();
				List<Rect> faces= _faceDetector .DetectFaces();
				foreach (var f in faces) {
					Debug.Log ("Found face: " + f.position.ToString ());
				}
				if (faces.Count > 0) {
					_face = faces [0];

					if (_face != null) {

						int x=(int)(Mathf.Max(0, _face.x * 2-Margin));
						int y=(int)(Mathf.Max(0, _face.y * 2-Margin));
						int w = (int)(Mathf.Min(640-x,_face.width * 2 + Margin * 2));
						int h = (int)(Mathf.Min(480-y,_face.height * 2 + Margin * 2));
						_capDev.ToImage (_image,x, y, w,h);
						_captured = true;
					}
				}
			}
		}
	}


	void OnDestroy()
	{
		_isDone = true;
		_imageGrabber.Join ();
		_capDev.Destroy ();
		_image.Destroy ();
		_faceDetector.Destroy ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_captured) {
			_image.BlitToTexture (BlitImage);
			target.texture = BlitImage;
			_captured = false;
		}
	}
}

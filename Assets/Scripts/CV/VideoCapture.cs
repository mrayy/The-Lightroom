using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System;

public class VideoCapture : MonoBehaviour,DebugInterface.IDebugElement {

	public int index;
	public int Width=640,Height=480;

	[Serializable]
	public class FaceDetectorParameters
	{
		public string cascadesPath="/Data/openCV/cascades/haarcascade_frontalface_default.xml";
		public float resizeFactor = 0.5f;
		public float scaler = 1.01f;
		public int minNeighbors = 8;
		public float minSize = 1.0f / 5.0f;
		public float maxSize=2.0f/3.0f;
	}

	public FaceDetectorParameters DetectorParameters;
	public DebugInterface Debugger;

	UnityOpenCVVideoCaptureAPI _capDev;
	UnityOpenCVFaceDetectorAPI _faceDetector;
	GstImageInfo _image;
	GstImageInfo _faceImage;
	public Texture2D BlitImage;
	public Texture2D FaceImage;

	public RawImage target;

	public Rect FaceRect;

	FrameCounterHelper _captureFPS=new FrameCounterHelper();

	Average3 _facePos = new Average3 (4);
	public bool _faceDetected=false;

	public bool FaceDetected {
		get {
			return _faceDetected;
		}
	}

	public Vector3 FacePos {
		get{ return _facePos.Value; }
	}

	Thread _imageGrabber;
	bool _isDone=false;
	bool _captured=false;
	bool _faceCaptured=false;
	public int Margin=50;


	// Use this for initialization
	void Start () {
		_capDev = new UnityOpenCVVideoCaptureAPI ();
		_faceDetector = new UnityOpenCVFaceDetectorAPI (Application.dataPath+DetectorParameters.cascadesPath,DetectorParameters.resizeFactor,DetectorParameters.scaler,DetectorParameters.minNeighbors,
			DetectorParameters.minSize,DetectorParameters.maxSize);
		_faceDetector.BindCamera (_capDev);
		_image=new GstImageInfo();
		_faceImage=new GstImageInfo();

		_capDev.Open (index);
		_capDev.SetSize (Width, Height);

		BlitImage = new Texture2D (1, 1);
		FaceImage = new Texture2D (1, 1);
		_image.Create (1, 1, GstImageInfo.EPixelFormat.EPixel_R8G8B8);
		_faceImage.Create (1, 1, GstImageInfo.EPixelFormat.EPixel_R8G8B8);


		_imageGrabber = new Thread (new ThreadStart (ImageGrabberThread));
		_imageGrabber.Start ();

		if(Debugger!=null)
			Debugger.AddDebugElement (this);
	}

	Rect _face;
	void ImageGrabberThread()
	{
		Vector2 sz;
		int c;
		while (!_isDone) {
			if (!_captured) {
				_capDev.ToImage (_image,0,0,Width,Height);
				_captured = true;
				_captureFPS.AddFrame ();
				//_capDev.Capture ();
				List<Rect> faces= _faceDetector .DetectFaces();
				//foreach (var f in faces) {
				//	Debug.Log ("Found face: " + f.position.ToString ());
				//}
				if (faces.Count > 0) {
					_face = faces [0];

					if (_face != null) {

						int x = (int)(Mathf.Max (0, _face.x  - Margin));
						int y = (int)(Mathf.Max (0, _face.y - Margin));
						int w = (int)(Mathf.Min (Width - x, _face.width  + Margin * 2));
						int h = (int)(Mathf.Min (Height - y, _face.height  + Margin * 2));
						FaceRect.Set (x, y, w, h);
						_facePos.AddSample (new Vector3 (x, y, w));
						_capDev.ToImage (_faceImage, x, y, w, h);
						_faceCaptured = true;
						_faceDetected = true;
					} else {
						_faceDetected = false;
					}
				} else {
					_faceDetected = false;
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
		_faceImage.Destroy ();
		_faceDetector.Destroy ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_faceCaptured) {
			_faceImage.BlitToTexture (FaceImage);
			_faceCaptured = false;
		}
		if (_captured) {
			_image.BlitToTexture (BlitImage);
			target.texture = BlitImage;
			_captured = false;
		}
	}
	public string GetDebugString()
	{
			return "Camera Capture FPS:"+((int)_captureFPS.FPS).ToString();
	}
}

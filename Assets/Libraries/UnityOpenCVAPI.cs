using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Threading;

public class UnityOpenCVVideoCaptureAPI
{
	public const string DllName = "UnityOpenCV";

	[DllImport(DllName)]
	private static extern IntPtr CVVideoCap_Create();
	[DllImport(DllName)]
	private static extern void CVVideoCap_Close(IntPtr Instance);
	[DllImport(DllName)]
	private static extern void CVVideoCap_Destroy(IntPtr Instance);

	[DllImport(DllName)]
	private static extern void CVVideoCap_SetSize(IntPtr Instance,int w,int h);
	[DllImport(DllName)]
	private static extern bool CVVideoCap_Open(IntPtr Instance,int index);
	[DllImport(DllName)]
	private static extern bool CVVideoCap_IsOpen(IntPtr Instance);
	[DllImport(DllName)]
	private static extern void CVVideoCap_Capture(IntPtr Instance);
	[DllImport(DllName)]
	private static extern bool CVVideoCap_ToImage(IntPtr Instance,IntPtr image, int x, int y, int w, int h);

	IntPtr _instance;

	public System.IntPtr GetInstance(){
		return _instance;
	}
	public bool IsOpen {
		get {
			return CVVideoCap_IsOpen (_instance);
		}
	}

	public UnityOpenCVVideoCaptureAPI()
	{
		_instance = CVVideoCap_Create ();
	}
	public bool Open(int idx){
		return CVVideoCap_Open(_instance,idx);
	}
	public void Close()
	{
		CVVideoCap_Close (_instance);
	}
	public void SetSize(int w,int h)
	{
		CVVideoCap_SetSize (_instance, w, h);
	}
	public void Capture()
	{
		CVVideoCap_Capture(_instance);
	}
	public void Destroy()
	{
		CVVideoCap_Destroy(_instance);
	}
	public bool ToImage(GstImageInfo image, int x, int y, int w, int h)
	{
		if (CVVideoCap_ToImage (_instance, image.GetInstance (), x, y, w, h)) {
			image.UpdateInfo ();
			return true;
		}
		return false;
	}
}

public class UnityOpenCVFaceDetectorAPI {

	public const string DllName = "UnityOpenCV";

	[DllImport(DllName)]
	private static extern IntPtr FaceDetector_Create([MarshalAs(UnmanagedType.LPStr)]string cascadesPath,float resizeFactor,float scaler,int minNeighbors,float minSize,float maxSize);

	[DllImport(DllName)]
	private static extern void FaceDetector_Destroy(IntPtr Instance);

	[DllImport(DllName)]
	private static extern void FaceDetector_BindImage(IntPtr Instance,IntPtr ifo);
	[DllImport(DllName)]
	private static extern void FaceDetector_BindCamera(IntPtr Instance,IntPtr camera);
	[DllImport(DllName)]
	private static extern int FaceDetector_DetectFaces(IntPtr Instance,ref IntPtr ptrResultVerts);

	IntPtr _instance;


	IntPtr _ptrResultPoints = IntPtr.Zero;
	int _resultFacesLength = 0;
	float[] _resultFaces = null;
	List<Rect> _resultFacesRects = new List<Rect> ();
	object _dataLock=new object();

	public List<Rect> DetectedFaces
	{
		get{
			return _resultFacesRects;
		}
	}

	public UnityOpenCVFaceDetectorAPI(string cascadesPath,float resizeFactor=1.0f,float scaler=1.01f,int minNeighbors=8,float minSize=1.0f/5.0f,float maxSize=2.0f/3.0f)
	{
		_instance = FaceDetector_Create (cascadesPath,resizeFactor,scaler,minNeighbors,minSize,maxSize);
	}

	public void Destroy()
	{
		FaceDetector_Destroy (_instance);
		_instance = IntPtr.Zero;
	}

	public void BindCamera(UnityOpenCVVideoCaptureAPI cam)
	{
		FaceDetector_BindCamera(_instance,cam.GetInstance ());

	}

	public void BindImage(GstImageInfo img)
	{
		FaceDetector_BindImage (_instance,img.GetInstance ());
	}

	public List<Rect> DetectFaces()
	{
		_resultFacesLength= FaceDetector_DetectFaces (_instance, ref _ptrResultPoints);

		_resultFacesRects.Clear ();
		lock (_dataLock) {
			if (_resultFaces == null || _resultFacesLength != _resultFaces.Length * 4) {
				_resultFaces = new float[_resultFacesLength*4 ];
			}

			if (_resultFacesLength > 0) {
				Marshal.Copy (_ptrResultPoints, _resultFaces, 0, _resultFacesLength* 4);
			}
		}

		if (_resultFacesLength > 0) {
			for (int i = 0; i < _resultFaces.Length / 4; ++i) {
				Rect r=new Rect(_resultFaces[i*4+0],_resultFaces[i*4+1],_resultFaces[i*4+2],_resultFaces[i*4+3]);
				_resultFacesRects.Add (r);
				//Debug.Log ("["+i.ToString()+"]-"+r.ToString());
			}

		}

		return _resultFacesRects;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class HandPosCalibrator : MonoBehaviour {

	class CalibrationPlane
	{
		Vector3 X, Y;
		bool _isCalibrated=false;
		public bool IsCalibrated {
			get {
				return _isCalibrated;
			}
		}
		Vector3 normal;
		Vector3 position;

		public bool FitPoints(Vector3[] Poses)
		{
			_isCalibrated = false;
			//http://www.ilikebigbits.com/blog/2015/3/2/plane-from-points
			Vector3 sum = Vector3.zero;
			foreach (var p in Poses) {
				sum += p;
			}
			Vector3 mean = sum / (float)Poses.Length;
			float xx = 0.0f; float xy = 0.0f; float xz = 0.0f;
			float yy = 0.0f; float yz = 0.0f; float zz = 0.0f;

			foreach (var p in Poses) {
				var d = p - mean;
				xx += d.x * d.x;
				xy += d.x * d.y;
				xz += d.x * d.z;
				yy += d.y * d.y;
				yz += d.y * d.z;
				zz += d.z * d.z;
			}
			float det_x = yy*zz - yz*yz;
			float det_y = xx*zz - xz*xz;
			float det_z = xx*yy - xy*xy;

			float maxDet = Mathf.Max (Mathf.Max (det_x, det_y), det_z);
			if (maxDet <= 0) {
				Debug.Log ("Failed to fit points to plane!");
				return false;
			}
			Vector3 dir;

			if(maxDet==det_x) {
				float a = (xz*yz - xy*zz) / det_x;
				float b = (xy*yz - xz*yy) / det_x;
				dir = new Vector3 (1.0f, a, b);
			}
			if(maxDet==det_y) {
				float a = (yz*xz - xy*zz) / det_y;
				float b = (xy*xz - yz*xx) / det_y;
				dir = new Vector3 (a,1.0f, b);
			} else {
				float a = (yz*xy - xz*yy) / det_z;
				float b = (xz*xy - yz*xx) / det_z;
				dir = new Vector3 (a, b,1.0f);
			}

			normal = dir.normalized;
			position=mean;

//			Vector3 pos=normal*-FittingPlane.distance;
			if (normal != Vector3.forward)
				X = Vector3.Cross (normal, Vector3.forward);
			else X= Vector3.Cross (normal, Vector3.up);
			Y = Quaternion.AngleAxis (90, normal) * X;


			Debug.Log (normal.ToString ()+":"+mean.ToString());
			_isCalibrated = true;
			return true;

		}

		public Vector2 ProjectPoint2D(Vector3 p)
		{
			if (!IsCalibrated)
				return Vector2.zero;
			p = p - position;
			Vector2 ret = new Vector2 ();
			ret.x = Vector3.Dot (p, X);
			ret.y = Vector3.Dot (p, Y);


			return ret;
		}
		public Vector3 ProjectPoint3D(Vector3 p)
		{
			if (!IsCalibrated)
				return Vector2.zero;

			Vector3 v = p - position;
			float dist = Vector3.Dot (v, normal);

			Vector3 ret = p - dist * normal;

			return ret;
		}

		public void DebugDraw()
		{
			Utilities.DrawPlane (position, normal);
		}
	}

	class RegressionData
	{
		public Vector2 B0;
		public Vector2 B1;

		bool _isCalibrated=false;
		public bool IsCalibrated {
			get {
				return _isCalibrated;
			}
		}

		public void Reset()
		{
			_isCalibrated=false;
		}

		public Vector2 GetPoint(Vector2 p)
		{
			return B0 + new Vector2 (p.x * B1.x, p.y * B1.y);
		}


		public bool Calibrate(Vector2[] x,Vector2[] y)
		{
			_isCalibrated = false;

			if (x.Length != y.Length)
				return false;
			int len = x.Length;

			Vector2 xMean = new Vector2 ();
			Vector2 yMean = new Vector2 ();

			Vector2[] errX = new Vector2[len];
			Vector2[] errY = new Vector2[len];
			Vector2[] SquaredX = new Vector2[len];
			Vector2 Sum = Vector2.zero;

			for (int i = 0; i < len; ++i) {
				errX[i]=x [i];
				xMean += errX [i];

				errY[i]=y [i];
				yMean += errY[i];
			}
			xMean /= (float)len;
			yMean /= (float)len;
			for (int i = 0; i < len; ++i) {

				errX [i] = errX [i] - xMean;
				errY [i] = errY [i] - yMean;
				SquaredX[i]=new Vector2(errX [i].x*errX[i].x,errX [i].y*errX [i].y);
				Sum += SquaredX [i];
			}

			B1 = Vector2.zero;
			for (int i = 0; i < len; ++i) {
				B1.x += errX [i].x * errY [i].x;
				B1.y += errX [i].y * errY [i].y;
			}
			B1.x/= Sum.x;
			B1.y/= Sum.y;

			B0=yMean-new Vector2(B1.x*xMean.x,B1.y*xMean.y);
			_isCalibrated = true;
			return true;
		}
	}

	public Vector2[] CalibrationPoints;
	public Vector3[] Poses;

	int _CurrCalibPoint=0;
	bool _isPinched=false;

	CalibrationPlane _calibPlane = new CalibrationPlane ();
	RegressionData _regData=new RegressionData();

	public CalibrationPoint CalibPoint;

	public PinchDetector Hand;


	float _lastTime;

	public bool IsCalibrated {
		get {
			return _regData.IsCalibrated;
		}
	}

	// Use this for initialization
	void Start () {
		Poses = new Vector3[CalibrationPoints.Length];
	}

	public Vector2 point;
	
	// Update is called once per frame
	void Update () {

		if (Hand.IsPinching) {
			if (!_isPinched) {
				Debug.Log ("Pinched");
				NextPoint (Hand.Position);
				if (_CurrCalibPoint == CalibrationPoints.Length) {
					if (!Calibrate ()) {
						Debug.Log ("Failed to calibrate, restarting");
						Reset ();
					}
				}
				_lastTime = Time.time;
				_isPinched = true;
			} else if (Time.time - _lastTime > 3 && _CurrCalibPoint!=0) {
				Debug.Log ("Resetting");
				Reset ();
			}
		} else {
			_isPinched = false;
		}

		if (IsCalibrated) {
			point = _regData.GetPoint (_calibPlane.ProjectPoint2D (Hand.Position));
		}
	}

	bool Calibrate()
	{
		_regData.Reset ();
		if (!_calibPlane.FitPoints (Poses))
			return false;

		Vector2[] a = new Vector2[CalibrationPoints.Length];

		for (int i = 0; i < CalibrationPoints.Length; ++i) {
			a[i]=_calibPlane.ProjectPoint2D(Poses [i]);
		}

		return _regData.Calibrate (a, CalibrationPoints);
	}

	bool NextPoint(Vector3 pos)
	{
		if (_CurrCalibPoint >= Poses.Length)
			return false;
		Poses [_CurrCalibPoint] = pos;
		_CurrCalibPoint++;
		return true;
	}


	public void Reset()
	{
		_regData.Reset ();
		_CurrCalibPoint = 0;
		for (int i = 0; i < CalibrationPoints.Length; ++i) {
			Poses [i]=Vector3.zero;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		for (int i = 0; i < _CurrCalibPoint; ++i) {
			Gizmos.DrawSphere (Poses[i], 0.01f);
		}

		if (IsCalibrated) {
			Gizmos.DrawSphere (_calibPlane.ProjectPoint3D (Hand.Position),0.01f);
		}
		if (IsCalibrated)
			_calibPlane.DebugDraw ();
	}


}

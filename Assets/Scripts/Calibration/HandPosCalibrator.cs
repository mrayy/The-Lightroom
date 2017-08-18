using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class HandPosCalibrator : MonoBehaviour {
	

	public Vector2[] CalibrationPoints;
	public Vector3[] Poses;

	int _CurrCalibPoint=0;
	bool _isPinched=false;

	CalibrationPlane _calibPlane = new CalibrationPlane ();
	Regression2D _regData=new Regression2D();

	public CalibrationPoint CalibPoint;

	public bool UseMouse=false;
	public AbstractHoldDetector Hand;
	public Vector2 point;//For debugging
	public Vector3 Pos;//For debugging

	float _lastTime;

	public bool IsPinched {
		get {
			if (UseMouse) {
				return Input.GetMouseButtonDown (0);
			} else
				return Hand.IsHolding;
		}
	}
	public Vector3 Position {
		get {
			if (UseMouse) {
				return new Vector3 (Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height, 0);
			} else
				return Hand.Position;
		}
	}



	public bool IsCalibrated {
		get {
			return _regData.IsCalibrated;
		}
	}

	// Use this for initialization
	void Start () {
		Poses = new Vector3[CalibrationPoints.Length];
		Reset ();
	}

	
	// Update is called once per frame
	void Update () {
		Pos = Position;

		if (IsPinched) {
			if (!_isPinched) {
				Debug.Log ("Pinched");
				NextPoint ( Position);
				if (_CurrCalibPoint == CalibrationPoints.Length) {
					if (!Calibrate ()) {
						Debug.Log ("Failed to calibrate, restarting");
						Reset ();
					} else {
						CalibPoint.SetCalibrated (true);
					}
				}
				_lastTime = Time.time;
				_isPinched = true;
			} else if (Time.time - _lastTime > 3 && _CurrCalibPoint!=0) {
				Reset ();
			}
		} else {
			_isPinched = false;
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Reset ();
		}

		if (IsCalibrated) {
			point = _regData.GetPoint (_calibPlane.ProjectPoint2D (Position));

			CalibPoint.SetPosition (point);
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

		CalibPoint.Pinched ();
		Poses [_CurrCalibPoint] = pos;
		_CurrCalibPoint++;

		if (_CurrCalibPoint < CalibrationPoints.Length)
			CalibPoint.SetPosition (CalibrationPoints [_CurrCalibPoint]);
		else
			OnDone ();


		return true;
	}

	void OnDone()
	{
		CalibPoint.OnDone ();
	}


	public void Reset()
	{
		Debug.Log ("Resetting");
		_regData.Reset ();
		_CurrCalibPoint = 0;
		CalibPoint.Reset ();
		CalibPoint.SetPosition (CalibrationPoints [_CurrCalibPoint]);
		CalibPoint.Pinched ();

		for (int i = 0; i < CalibrationPoints.Length; ++i) {
			Poses [i]=Vector3.zero;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		for (int i = 0; i < _CurrCalibPoint; ++i) {
			Gizmos.DrawSphere (Poses[i], 0.01f);
		}

		Gizmos.DrawSphere ( Pos,0.01f);

		if (IsCalibrated) {
			Gizmos.DrawSphere (_calibPlane.ProjectPoint3D (Position),0.01f);
		}
		if (IsCalibrated)
			_calibPlane.DebugDraw ();
	}


}

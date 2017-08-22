using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class HandPosCalibrator : MonoBehaviour,IDependencyNode {
	

	public Vector2[] CalibrationPoints;
	public Vector3[] Poses;

	int _CurrCalibPoint=-1;
	bool _isPinched=false;

	CalibrationPlane _calibPlane = new CalibrationPlane ();
	Regression2D _regData=new Regression2D();

	public delegate void OnCalibrationDone_Deleg();
	public delegate void OnCalibrationReset_Deleg(bool failure);
	public delegate void OnCalibrationPoint_Deleg(Vector3 pos);
	public OnCalibrationDone_Deleg OnCalibrationDone;
	public OnCalibrationPoint_Deleg OnCalibrationPoint;
	public OnCalibrationReset_Deleg OnCalibrationReset;

	public CalibrationPoint CalibPoint;

	public bool UseMouse=false;
	public HandDetector Hand;
	public Vector2 point;//For debugging
	public Vector3 Pos;//For debugging

	public UnityEngine.UI.Image HandDetectedImage;

	float _lastTime;

	bool _isPinchUp=false;

	public bool IsPinchedUp {
		get {
			return _isPinchUp;
		}
	}
	public bool IsPinched {
		get {
			if (UseMouse || Hand==null) {
				return Input.GetMouseButtonDown (0);
			} else
				return Hand.IsPinched;
		}
	}
	public Vector3 Position {
		get {
			if (UseMouse || Hand==null) {
				return new Vector3 (Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height, 0);
			} else
				return Hand.Position;
		}
	}

	public bool BothTriggered
	{
		get{
			if (UseMouse || Hand==null) {
				return Input.GetMouseButtonDown (1);
			} else
				return Hand.BothTriggered;
		}
	}

	public bool IsDetected
	{
		get{

			if (UseMouse || Hand == null) {
				return true;
			} else {
				return Hand.IsDetected;
			}
		}
	}


	public bool IsCalibrated {
		get {
			return _regData.IsCalibrated;
		}
	}

	public bool IsCalibrating {
		get {
			return _CurrCalibPoint>=0 && _CurrCalibPoint < CalibrationPoints.Length;
		}
	}

	public float PinchTime {
		get{ return Time.time - _lastTime; }
	}

	public int CalibrationPointNumber {
		get{ return _CurrCalibPoint; }
	}

	public void OnDependencyStart(DependencyRoot root)
	{
		bool.TryParse(SettingsHolder.Instance.GetValue("Simulation","NoLeapmotion","false"),out UseMouse);
	}
	// Use this for initialization
	void Start () {
		Poses = new Vector3[CalibrationPoints.Length];
		Reset ();
		SettingsHolder.Instance.AddDependencyNode (this);
	}

	
	// Update is called once per frame
	void Update () {
		Pos = Position;

		if (IsPinched) {
			if (!_isPinched) {
				NextPoint ( Position);
				if (_CurrCalibPoint == CalibrationPoints.Length) {
					if (!Calibrate ()) {
						Debug.Log ("Failed to calibrate, restarting");
						Reset (true);
					} else {
						CalibPoint.SetCalibrated (true);
					}
				}
				_lastTime = Time.time;
				_isPinched = true;
			}
			/*else if (Time.time - _lastTime > 3 && _CurrCalibPoint!=0) {
				Reset ();
			}*/
		} else {
			if (_isPinched)
				_isPinchUp = true;
			else
				_isPinchUp = false;
			_isPinched = false;
			_lastTime = Time.time;
		}

		//if (Input.GetKeyDown (KeyCode.R)) {
		//	Reset ();
		//}

		if (HandDetectedImage != null) {
			HandDetectedImage.enabled=IsDetected;
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

		if (OnCalibrationPoint != null)
			OnCalibrationPoint (pos);

		if (_CurrCalibPoint < CalibrationPoints.Length)
			CalibPoint.SetPosition (CalibrationPoints [_CurrCalibPoint]);
		else
			OnDone ();


		return true;
	}

	void OnDone()
	{
		CalibPoint.OnDone ();
		if (OnCalibrationDone!=null)
			OnCalibrationDone ();
	}


	public void Reset(bool fromFailure=false)
	{
		//Debug.Log ("Resetting");
		_regData.Reset ();
		_CurrCalibPoint = 0;
		CalibPoint.Reset ();
		CalibPoint.SetPosition (CalibrationPoints [_CurrCalibPoint]);
		CalibPoint.Pinched ();

		if (OnCalibrationReset!=null)
			OnCalibrationReset (fromFailure);

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

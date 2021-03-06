﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationPoint : MonoBehaviour {

	enum EState
	{
		Idle,
		Showing,
		Show,
		Pinching,
		Pinched
	}

	EState state=EState.Idle;
	EState targetState=EState.Idle;

	public CalibrationScreenEffect Effect;

	bool _calibrated=false;

	// Use this for initialization
	void Start () {
		
	}

	public void SetCalibrated(bool calibrated)
	{
		_calibrated = calibrated;
		Effect.MaxGazeSize = _calibrated ? 0.05f : 0.09f;
	}

	public void SetPosition(Vector2 pos)
	{
		Effect.SetPosition (pos*0.5f+new Vector2(0.5f,0.5f),!_calibrated);

		transform.localPosition = new Vector3 (pos.x*Screen.width/2, pos.y*Screen.height/2, 0);
	}

	public void Pinched()
	{
		targetState = EState.Pinched;
		Effect.SetTriggered ();
	}

	public void Reset()
	{
		Effect.Restart ();
		SetCalibrated (false);
	}

	public void OnDone()
	{
		Effect.OnDone ();
	}

}

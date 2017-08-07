using System.Collections;
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
		Effect.MaxGazeSize = _calibrated ? 0.1f : 0.2f;
	}

	public void SetPosition(Vector2 pos)
	{
		Effect.SetPosition (pos*0.5f+new Vector2(0.5f,0.5f),!_calibrated);
	}

	public void Pinched()
	{
		targetState = EState.Pinched;
		Effect.SetTriggered ();
	}

}

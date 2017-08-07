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

	// Use this for initialization
	void Start () {
		
	}



	public void SetPosition(Vector2 pos)
	{
		targetState = EState.Show;
		Effect.SetPosition (pos*0.5f+new Vector2(0.5f,0.5f));
	}

	public void Pinched()
	{
		targetState = EState.Pinched;
		Effect.SetTriggered ();
	}

}

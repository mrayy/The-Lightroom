using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBehaviour : MonoBehaviour {


	public bool _behaviourPaused=false;

	public virtual void StopBehaviour(){
		_behaviourPaused = true;
	}
	public virtual void StartBehaviour(){
		_behaviourPaused = false;
	}
	protected virtual void UpdateBehaviour()
	{
	}

	void Update()
	{
		if (!_behaviourPaused)
			UpdateBehaviour ();
	}
}

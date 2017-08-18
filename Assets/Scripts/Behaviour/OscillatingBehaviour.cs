using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingBehaviour : IBehaviour {

	public float Speed = 1;
	public float Offset=0;
	public float Range=0;
	public Vector3 InitialOffset;

	float _time=0;

	// Use this for initialization
	void Start () {

		Offset = (float)Random.Range (-100, 100) / 200.0f;
		Speed = (float)Random.Range (-100, 100) / 200.0f;
		Range = (float)Random.Range (-100, 100) / 20.0f;

		InitialOffset = transform.localPosition;
		
	}
	
	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();

		this.transform.localPosition = new Vector3 (InitialOffset.x,InitialOffset.y,Mathf.Sin(_time*Speed+Offset)*Range+InitialOffset.z);

		_time += Time.deltaTime;
	}
}

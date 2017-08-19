using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElementBehaviour))]
public class OscillatingBehaviour : IBehaviour {

	public Vector3 InitialOffset;

	Quaternion _axis=Quaternion.identity;
	ElementBehaviour _baseBehaviour;

	float _time=0;

	// Use this for initialization
	void Start () {

		InitialOffset = transform.localPosition;

		_axis = Quaternion.Euler (Random.Range (0, 180), Random.Range (0, 180), 0);

		_baseBehaviour = GetComponent<ElementBehaviour> ();
	}
	
	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();

		Vector3 pos=new Vector3(_baseBehaviour.Radius*Mathf.Cos(_baseBehaviour.Theta),0,_baseBehaviour.Radius*Mathf.Sin(_baseBehaviour.Theta));

		this.transform.localPosition = InitialOffset+_axis*pos;

		_time += Time.deltaTime;
	}
}

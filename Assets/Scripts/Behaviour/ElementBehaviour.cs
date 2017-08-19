using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBehaviour : IBehaviour {

	public float BaseRadius=1.5f;
	public float Radius=1.5f;
	public float Speed=0.5f;

	public float Theta;

	// Use this for initialization
	void Start () {
		BaseRadius += Random.Range (0, 400) / 100.0f;
	}
	
	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();

		Radius = BaseRadius / (1.0f + Mathf.Cos (Theta)*0.5f);
		Theta += Speed*Time.deltaTime*VolumeManager.Instance.AudioOmega/(Radius*Radius);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorBehaviour : IBehaviour {

	public Color BaseColor;
	Renderer _renderer;
	// Use this for initialization
	void Start () {


		_renderer=GetComponent<Renderer> ();
	}

	// Update is called once per frame
	protected override void UpdateBehaviour () {
		base.UpdateBehaviour ();
		_renderer.material.color = BaseColor;
	}
}

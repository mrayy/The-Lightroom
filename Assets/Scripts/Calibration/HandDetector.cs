using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class HandDetector : MonoBehaviour {

	public AbstractHoldDetector[] Detectors;


	public bool BothTriggered
	{
		get{ 
			bool t = true;
			foreach (var d in Detectors) {
				if (!d.HandModel.IsTracked || !d.IsHolding) {
					t = false;
					break;
				}
			}
			return t;
		}
	}
	public bool IsDetected
	{
		get {
			foreach (var d in Detectors) {
				if (d.HandModel.IsTracked) {
					return true;
				}
			}
			return false;
		}
	}

	public bool IsPinched {
		get {
			foreach (var d in Detectors) {
				if (d.HandModel.IsTracked && d.IsHolding) {
					return true;
				}
			}
			return false;
		}
	}
	public Vector3 Position {
		get{
			foreach (var d in Detectors) {
				if (d.HandModel.IsTracked) {
					return d.Position;
				}
			}
			return Vector3.zero;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

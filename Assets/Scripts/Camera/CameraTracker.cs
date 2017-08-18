using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour {

	public Vector3 Gain=new Vector3(1,1,1);
	public Vector3 Initial;
	public Vector3 Offset;

	public bool UseMouse=false;
	public VideoCapture CaptureCam;

	// Use this for initialization
	void Start () {
		Offset = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			Initial = GetEyePos ();
		}
		Vector3 pos = GetEyePos () - Initial;
		transform.position = new Vector3(pos.x*Gain.x,pos.y*Gain.y,pos.z*Gain.z)+Offset;
	}

	Vector3 GetEyePos()
	{
		if (UseMouse)
			return Input.mousePosition;
		else {
			var pos = CaptureCam.FacePos;
			float scale = CaptureCam.FaceRect.width;
			var ret= new Vector3 (-pos.x, -pos.y, scale);
			ret.z = Mathf.Min (ret.z, -0.1f);
			return ret;
				
		}
	}
}

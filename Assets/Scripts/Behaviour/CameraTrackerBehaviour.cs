using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flask;

public class CameraTrackerBehaviour : IBehaviour {

	public Vector3 Gain=new Vector3(1,1,1);
	public Vector3 Initial;
	public Vector3 Offset;

	public Vector3 MinLimits = new Vector3 (-1, -1, -1);
	public Vector3 MaxLimits = new Vector3 (1, 1, 1);

	public bool UseMouse=false;
	public VideoCapture CaptureCam;


	//for initial animation purpose
	public float MinZ;
	public float MaxZ;
	public float AnimSpeed=1;
	bool _animationDone=false;

	DTween _animTween = new DTween (0, 2);


	// Use this for initialization
	void Start () {
		Offset = transform.localPosition;
		Offset.z = MinZ;
	}

	bool _updateAnimation()
	{
		if (_animationDone == true)
			return false;

		_animTween.Step (MaxZ);
		Offset.z = _animTween.position;
		transform.localPosition = Offset;
		if (Mathf.Abs (Offset.z - MaxZ) < 0.01f) {
			Initial = GetEyePos ();
			_animationDone = true;
		}
		return true;
	}

	public void Reset()
	{
		_animationDone = false;
		Offset.z = MinZ;
		_animTween.position = MinZ;
		transform.localPosition = Offset;
	}
	public override void StartBehaviour(){
		Reset ();
		base.StartBehaviour ();
	}

	// Update is called once per frame
	protected override void UpdateBehaviour()
	{
		base.UpdateBehaviour ();

		if (_updateAnimation ())
			return;
		Vector3 pos = GetEyePos () - Initial;
		pos = Vector3.Scale (pos, Gain);
		pos.x = Mathf.Clamp (pos.x, MinLimits.x, MaxLimits.x);
		pos.y = Mathf.Clamp (pos.y, MinLimits.y, MaxLimits.y);
		pos.z = Mathf.Clamp (pos.z, MinLimits.z, MaxLimits.z);
		transform.position = pos+Offset;
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

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube (Offset+(MinLimits + MaxLimits) / 2, (MaxLimits - MinLimits));
	}
}

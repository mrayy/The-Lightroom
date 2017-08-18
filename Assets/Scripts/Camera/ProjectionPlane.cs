using UnityEngine;
using System.Collections;

public class ProjectionPlane : MonoBehaviour {

	public float ScreenWidth;
	public float ScreenHeight;
	public float OffsetX,OffsetY;
	public float ScreenDistance;
	public Transform head;
	public float aspect;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		aspect = (float)Screen.width/(float)Screen.height;



		//this.transform.localPosition = new Vector3 (OffsetX, OffsetY, head.localPosition.z + ScreenDistance);
		this.transform.localScale = new Vector3 (aspect, 1,1);
	}
}

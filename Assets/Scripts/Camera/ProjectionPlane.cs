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
		this.transform.localScale = GetScale();
	}

	Vector3 GetScale()
	{
		aspect = (float)Screen.width/(float)Screen.height;
		//this.transform.localPosition = new Vector3 (OffsetX, OffsetY, head.localPosition.z + ScreenDistance);
		return new Vector3 (ScreenWidth, ScreenWidth/aspect,1);
	}

	void OnDrawGizmos()
	{
		Matrix4x4 m = Matrix4x4.TRS (transform.position, transform.rotation, GetScale());
		Vector3 pa=m.MultiplyPoint(new Vector3(-0.5f,-0.5f,0));
		Vector3 pb=m.MultiplyPoint(new Vector3(-0.5f, 0.5f,0));
		Vector3 pc=m.MultiplyPoint(new Vector3( 0.5f, 0.5f,0));
		Vector3 pd=m.MultiplyPoint(new Vector3( 0.5f,-0.5f,0));

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (pa,pb);
		Gizmos.DrawLine (pb,pc);
		Gizmos.DrawLine (pc,pd);
		Gizmos.DrawLine (pd,pa);
	}
}

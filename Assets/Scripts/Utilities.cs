using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

	public static void  DrawPlane(Vector3 pos,Vector3 normal)
	{
		Vector3 v3;
		if (normal != Vector3.forward)
			v3 = Vector3.Cross (normal, Vector3.forward);
		else v3= Vector3.Cross (normal, Vector3.up);

		Vector3 p0=pos+v3;
		Vector3 p1=pos-v3;
		v3 = Quaternion.AngleAxis (90, normal) * v3;

		Vector3 p2=pos+v3;
		Vector3 p3=pos-v3;

		Gizmos.color = Color.green;
		Gizmos.DrawLine (p0, p2);
		Gizmos.DrawLine (p1, p3);
		Gizmos.DrawLine (p0, p1);
		Gizmos.DrawLine (p1, p2);
		Gizmos.DrawLine (p2, p3);
		Gizmos.DrawLine (p3, p0);
		Gizmos.color = Color.red;
		Gizmos.DrawRay (pos, normal);
	}

}

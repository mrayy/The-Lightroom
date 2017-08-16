using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPerspective : MonoBehaviour {
	
	bool m_dirty;

	public Vector3 m_pos;
	public Vector3 m_pa,m_pb,m_pc;

	public float m_zfar, m_znear;

	public float m_fov;

	Matrix4x4 m_projection;
	Matrix4x4 m_view;

	Quaternion m_rotation;
	void Start()
	{
		m_dirty = true;
		m_znear = 0.1f;
		m_zfar = 100;
		SetScreenCorners(new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1));
	}

	 void LateUpdate()
	{
		_UpdateMatrix ();
		Camera.main.projectionMatrix = m_projection;
		Camera.main.worldToCameraMatrix = m_view;
	}

	void _UpdateMatrix()
	{
	//	if (!m_dirty)
	//		return;
	//	m_dirty = false;

		Vector3 va, vb, vc;
		Vector3 vr, vu, vn;
		float l, r, b, t, d;

		// Compute an orthonormal basis for the screen

		vr = m_pb - m_pa;
		vu = m_pc - m_pa;

		vr.Normalize();
		vu.Normalize();

		vn = -Vector3.Cross(vr,vu);

		// Compute Screen corner vectors
		va = m_pa - m_pos;
		vb = m_pb - m_pos;
		vc = m_pc - m_pos;

		//Find the distance from the eye to screen plane
		d = - Vector3.Dot( va,vn);

		//Find the extent of the perpendicular projection
		l = Vector3.Dot(vr,va)*m_znear / d;
		r = Vector3.Dot(vr,vb)*m_znear / d;
		b = Vector3.Dot(vu,va)*m_znear / d;
		t = Vector3.Dot(vu,vc)*m_znear / d;

		//Set Projection Matrix

		m_projection.m00 = 2.0f*m_znear / (r - l);
		m_projection.m01 = 0.0f;
		m_projection.m02 = (r + l) / (r - l);
		m_projection.m03 = 0;

		m_projection.m10 = 0;
		m_projection.m11 = 2.0f*m_znear / (t - b);
		m_projection.m12 = (t + b) / (t - b);
		m_projection.m13 = 0;

		m_projection.m20 = 0;
		m_projection.m21 = 0;
		m_projection.m22 = (m_znear + m_zfar) / (m_znear - m_zfar);
		m_projection.m23 = 2.0f*m_znear*m_zfar / (m_znear - m_zfar);

		m_projection.m30 = 0;
		m_projection.m31 = 0;
		m_projection.m32 = -1.0f;
		m_projection.m33 = 0;


		Matrix4x4 rot,tran;
		rot = tran = Matrix4x4.identity;

		rot.m00 = vr.x;
		rot.m01 = vr.y;
		rot.m02 = vr.z;
		rot.m03 = 0;

		rot.m10 = vu.x;
		rot.m11 = vu.y;
		rot.m12 = vu.z;
		rot.m13 = 0;

		rot.m20 = vn.x;
		rot.m21 = vn.y;
		rot.m22 = vn.z;
		rot.m23 = 0;

		rot.m30 = 0;
		rot.m31 = 0;
		rot.m32 = 0;
		rot.m33 = 1;

		//rot = rot.getTransposed();
		tran.m03 = -m_pos.x;
		tran.m13 = -m_pos.y;
		tran.m23 = -m_pos.z;

		m_view = rot*tran;

		//m_rotation.fromMatrix(math::MathUtil::CreateLookAtMatrix(m_pos,(m_pb + m_pc)*0.5f, vu));
		//m_rotation;

		float ba = (m_pb - m_pa).magnitude;
		float ca = (m_pc - m_pa).magnitude;
		float vlen = va.magnitude;
		m_fov = Mathf.Rad2Deg*Mathf.Atan2(ba+ca, vlen);
	}
	public void SetEyePosition(Vector3  pos)
	{
		m_pos = pos;
		m_dirty = true;
	}
	public void SetZNearFar(float near, float far)
	{
		m_znear = near;
		m_zfar = far;
		m_dirty = true;
	}

	//pa: Lower Left Corner of the screen 
	//pb: Top Left Corner of the screen 
	//pc: Lower Right Corner of the screen 
	// pc	----------------
	// |                |
	// |                |
	// |                |
	// pb---------------pa
	public void SetScreenCorners(Vector3  bottomLeft, Vector3  topLeft, Vector3  bottomRight)
	{
		m_pa = bottomLeft;
		m_pb = bottomRight;
		m_pc = topLeft;
		m_dirty = true;
	}

	public void SetScreenParams(Vector3  center, Vector2 hsize, Quaternion  ori)
	{
		Vector3 pa, pb, pc;

		pa = center + ori*new Vector3(-hsize.x, -hsize.y, 0);
		pb = center + ori*new Vector3(-hsize.x, hsize.y, 0);
		pc = center + ori*new Vector3(hsize.x, -hsize.y, 0);

		SetScreenCorners(pa, pb, pc);
	}

	public  Matrix4x4 GetProjectionMatrix()
	{
		_UpdateMatrix();
		return m_projection;
	}
	public Matrix4x4 GetViewMatrix()
	{
		_UpdateMatrix();
		return m_view;
	}
	public Quaternion GetRotation()
	{
		_UpdateMatrix();
		return m_rotation;
	}
	public float GetFoV()
	{
		_UpdateMatrix();
		return m_fov;
	}
}

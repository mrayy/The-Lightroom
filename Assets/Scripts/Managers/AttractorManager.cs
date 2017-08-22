using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorManager : MonoBehaviour {

	static AttractorManager _instance;
	public static AttractorManager Instance
	{
		get{return _instance;}
	}

	List<AttractorBehaviour> _attractors=new List<AttractorBehaviour>();

	public HandPosCalibrator Hand;
	public EnvironmentHandler Env;

	public AttractorBehaviour Prefab;

	public Transform root;

	public bool IsActive=true;

	bool _isPinched=false;
	bool _isRemoved=false;

	float _time=0;

	AttractorManager()
	{
		_instance = this;
	}
	// Use this for initialization
	void Start () {

		var attractors=GameObject.FindObjectsOfType<AttractorBehaviour> ();
		foreach (var a in attractors)
			_attractors.Add (a);
	}

	public void AddAttractor(AttractorBehaviour a)
	{
		_attractors.Add (a);
	}

	public void RemoveAttractor(AttractorBehaviour a)
	{
		_attractors.Remove (a);
	}

	public float CalculateAttraction(Vector3 pos)
	{
		if (_attractors.Count == 0)
			return 0;
		float sum = 0;
		foreach (var a in _attractors) {
			float dist = (pos - a.transform.position).magnitude;

			sum += 1.0f/dist;
		}
		return (float)sum/(float)_attractors.Count;
	}
	public Vector3[] CalculateAttractionVectors(Vector3 pos)
	{
		if (_attractors.Count == 0)
			return null;
		Vector3[] res = new Vector3[_attractors.Count];
		for(int i=0;i< _attractors.Count;++i) {
			res[i] = (pos - _attractors[i].transform.position);
		}
		return res;
	}


	public Color CalculateAttractionColor(Vector3 pos)
	{
		Color res = Color.black;
		if (_attractors.Count == 0)
			return res;
		foreach (var a in _attractors) {
			float dist = (pos - a.transform.position).magnitude;
			res = res + a.BaseColor/ dist;
		}
		return res/(float)_attractors.Count;
	}
	public AttractorBehaviour GetClosest(Ray r,out float dist)
	{
		dist=99999;
		AttractorBehaviour bestAttr = null;
		foreach (var a in _attractors) {
			float d = Vector3.Cross (r.direction, a.transform.position - r.origin).magnitude;
			if (d < dist) {
				dist = d;
				bestAttr = a;
			}
		}

		return bestAttr;
	}
	public Ray hpos;
	public AttractorBehaviour Closest;
	// Update is called once per frame
	void Update () {

		if (!IsActive)
			return;

		hpos=Camera.main.ScreenPointToRay (new Vector3 ((Hand.point.x*0.5f+0.5f)*Screen.width,(Hand.point.y*0.5f+0.5f)*Screen.height,4));
		float dist = 0;
		var c = GetClosest (hpos, out dist);
		if (c != Closest && Closest != null) {
			Closest.selected = false;
		}
		Closest = c;
		if(Closest!=null)
			Closest.selected = true;

		if (Hand.IsPinched && Hand.PinchTime>2) {
			if (!_isRemoved && (Time.time-_time)>2.0f) {
				_time = Time.time;
				_isRemoved = true;


				if (Closest!=null) {
					Env.TriggerSpace (0.2f);
					Closest.Destroy ();
					_attractors.Remove (Closest);
				}
			}
		} else {
			_isRemoved = false;
		}

		if (Hand.IsPinchedUp) {
			if (!_isPinched&& (Time.time-_time)>2.0f) {
				_time = Time.time;
				_isPinched = true;

				Env.TriggerSpace (0.1f);

				var obj=Instantiate (Prefab.gameObject, Vector3.zero, Quaternion.identity, root) as GameObject;
				obj.transform.position =hpos.origin+ hpos.direction*3;
				AddAttractor (obj.GetComponent<AttractorBehaviour> ());
			}
		} else {
			_isPinched = false;
		}
	}

	void OnDrawGizmos()
	{
		var r=Camera.main.ScreenPointToRay (new Vector3 ((Hand.point.x*0.5f+0.5f)*Screen.width,(Hand.point.y*0.5f+0.5f)*Screen.height,4));

		Gizmos.DrawRay(r);
	}
}

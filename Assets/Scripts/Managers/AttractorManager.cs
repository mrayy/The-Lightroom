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
		float sum = 0;
		foreach (var a in _attractors) {
			float dist = (pos - a.transform.position).magnitude;

			sum += 1.0f/dist;
		}
		return (float)sum/(float)_attractors.Count;
	}
	public Vector3[] CalculateAttractionVectors(Vector3 pos)
	{
		Vector3[] res = new Vector3[_attractors.Count];
		for(int i=0;i< _attractors.Count;++i) {
			res[i] = (pos - _attractors[i].transform.position);
		}
		return res;
	}


	public Color CalculateAttractionColor(Vector3 pos)
	{
		Color res = Color.black;
		foreach (var a in _attractors) {
			float dist = (pos - a.transform.position).magnitude;
			res = res + a.BaseColor/ dist;
		}
		return res/(float)_attractors.Count;
	}
	// Update is called once per frame
	void Update () {
		
	}
}

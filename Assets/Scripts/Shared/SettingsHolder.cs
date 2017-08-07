using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHolder : MonoBehaviour 
{
	static SettingsHolder _Instance;

	public bool Debug=false;


	public Camera activeCamera;


	public static SettingsHolder Instance
	{
		get{
			if (_Instance == null)
				_Instance = GameObject.FindObjectOfType<SettingsHolder> ();
			return _Instance;
		}
	}
	// Use this for initialization
	void Start () {
		_Instance = this;


	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F9)) {
			Debug = !Debug;

		}
	}


	public Vector3 GetProjectedCanvasPosition(Vector3 pos)
	{
		Vector3 p=GetComponent<Camera>().WorldToScreenPoint (pos);
		p.x -= GetComponent<Camera>().pixelWidth / 2;
		p.y -= GetComponent<Camera>().pixelHeight / 2;
		return p;
	}
}

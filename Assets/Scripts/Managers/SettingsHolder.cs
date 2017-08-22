using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsHolder : DependencyRoot 
{
	static SettingsHolder _Instance;

	public bool Debug=false;

	public INIParser _settings;

	public Camera activeCamera;

	public INIParser Settings
	{
		get{
			return _settings;
		}
	}

	public static SettingsHolder Instance
	{
		get{
			if (_Instance == null)
				_Instance = GameObject.FindObjectOfType<SettingsHolder> ();
			return _Instance;
		}
	}
	// Use this for initialization
	protected override void Start () {
		_Instance = this;
		_settings = new INIParser ();
		_settings.Open (Application.dataPath+"\\Data\\Settings.ini");

		base.Start ();
	}

	public string GetValue(string cat,string name,string def)
	{
		return _settings.ReadValue (cat, name,def);
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

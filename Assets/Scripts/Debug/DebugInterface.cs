using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugInterface : MonoBehaviour {

	public interface IDebugElement
	{
		string GetDebugString();
	}

	public class DebugFPSElement:DebugInterface.IDebugElement
	{
		FrameCounterHelper m_fpsHelper=new FrameCounterHelper();

		public DebugFPSElement ()
		{
		}

		public string GetDebugString()
		{
			m_fpsHelper.AddFrame ();
			return "FPS= " + m_fpsHelper.FPS.ToString();
		}
	}

	public Text DebugText;
	public bool Visible=false;

	List<IDebugElement> _debugElements=new List<IDebugElement>();

	public void AddDebugElement(IDebugElement e)
	{
		_debugElements.Add (e);
	}
	public void RemoveDebugElement(IDebugElement e)
	{
		_debugElements.Remove (e);
	}

	// Use this for initialization
	void Start () {
		_debugElements.Add (new DebugFPSElement ());
		DebugText.enabled=Visible;
	}

	// Update is called once per frame
	void Update () {

		if (DebugText != null) {
			DebugText.text=GenerateDebugString();
		}

		DebugText.enabled=SettingsHolder.Instance.Debug;

	}

	string GenerateDebugString()
	{
		string str = "";
		foreach (var e in _debugElements) {
			str+=e.GetDebugString()+"\n";
		}
		return str;
	}
	void OnGUI()
	{
	}
}

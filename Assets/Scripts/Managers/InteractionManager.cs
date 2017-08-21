using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flask;

public class InteractionManager : MonoBehaviour {

	public HandPosCalibrator HandCalibrator;
	public VolumeManager Volume;
	public EnvironmentHandler Environment;

	public CameraTrackerBehaviour targetCamera;

	public Text InstructionText;
	public Text DetailsText;

	public Color Color1;
	public Color Color2;

	public float _currAlpha;

	DTween _colorTween=new DTween(0,1);
	public float _lastTween=1;


	Coroutine _currentTextCor;

	enum State
	{
		Calibration,
		Idle
	}


	bool _firstCalibration=true;
	State _currentState;

	// Use this for initialization
	void Start () {
		HandCalibrator.OnCalibrationDone += OnCalibrationDone;
		HandCalibrator.OnCalibrationReset += OnCalibrationReset;
		HandCalibrator.OnCalibrationPoint += OnCalibrationPoint;

		if (HandCalibrator.IsCalibrating) {
			OnCalibrationReset(false);
		}

		_lastTween = _colorTween.target=1;

	}

	float PlayText(string[] text,float[] timeout,float speed=1,bool isCentered=false)
	{
		if(_currentTextCor!=null)
			StopCoroutine (_currentTextCor);

		float time = 0;
		for(int i=0;i<text.Length;++i)
		{
			time+=timeout[i];
			if (text [i] == "")
				time += 2.0f / speed;
		}
		_currentTextCor=StartCoroutine (PlayTextCor (text,timeout,speed,isCentered));
		return time;
	}

	IEnumerator PlayTextCor(string[] text,float[] timeout,float speed=1,bool isCentered=false)
	{
		if (!isCentered) {
			if (Random.value > 0.5f) {
				InstructionText.rectTransform.localPosition = new Vector3 (0, -190, 0);
			} else {
				InstructionText.rectTransform.localPosition = new Vector3 (0, 190, 0);
			}
			InstructionText.fontSize = 42;
		} else {
			InstructionText.rectTransform.localPosition = Vector3.zero;
			InstructionText.fontSize = 80;
		}
		for (int i = 0; i < text.Length; ++i) {
			_currAlpha = 0;
			InstructionText.text = text [i];
			if (text [i] != "") {
				do {
					_currAlpha = Mathf.Clamp01(_currAlpha+Time.deltaTime*speed);
					yield return new WaitForEndOfFrame ();
				} while (_currAlpha < 1);
			}
			_currAlpha = 1;
			yield return new WaitForSeconds (timeout [i]);
			if (text [i] != "") {
				do{
					_currAlpha = Mathf.Clamp01(_currAlpha-Time.deltaTime*speed);
					yield return new WaitForEndOfFrame ();
				} while (_currAlpha > 0);
			}
			_currAlpha = 0;
		}
	}

	void PlayDetails(string[] text,float[] timeout)
	{
		if(_currentTextCor!=null)
			StopCoroutine (_currentTextCor);
		_currentTextCor=StartCoroutine (PlayDetailsCor (text,timeout));
	}

	IEnumerator PlayDetailsCor(string[] text,float[] timeout)
	{
		float time = 0;
		Color clr = DetailsText.color;
		for (int i = 0; i < text.Length; ++i) {

			if (Random.value > 0.5f) {
				DetailsText.rectTransform.localPosition = new Vector3 (-450,0, 0);
			} else {
				DetailsText.rectTransform.localPosition = new Vector3 (450, 190, 0);
			}
			time = 0;
			clr.a = 0;
			DetailsText.text = text [i];
			if (text [i] != "") {
				do {
					clr.a = Mathf.Clamp01(_currAlpha+Time.deltaTime);
					DetailsText.color=clr;
					DetailsText.lineSpacing=Mathf.Lerp(4,1,clr.a);
					yield return new WaitForEndOfFrame ();
				} while (clr.a < 1);
			}
			clr.a = 1;
			yield return new WaitForSeconds (timeout [i]);
			if (text [i] != "") {
				do{
					clr.a = Mathf.Clamp01(_currAlpha-Time.deltaTime);
					DetailsText.color=clr;
					DetailsText.lineSpacing=Mathf.Lerp(4,1,clr.a);
					yield return new WaitForEndOfFrame ();
				} while (clr.a > 0);
			}

		}
	}

	void OnStartCalibration()
	{
		string[] text = new string[]{ "Hello","", "Lets Calibrate your Space","", "Move your hand to the point and pinch" };
		float[] timeout = new float[]{ 2,0.2f, 5,0.2f, 5 };
		PlayText (text,timeout);
		_firstCalibration = false;

	}

	Coroutine cooldownCoroutine;
	IEnumerator StartSceneCooldown(float t)
	{
		yield return new WaitForSeconds (t);
		for (int i = 3; i >0; i--) {
			PlayText (new string[]{i.ToString()},new float[]{1f},4,true);
			yield return new WaitForSeconds (1.5f);
		}
		StartScene ();
	}

	void OnCalibrationDone()
	{
		string[] text = new string[]{ "Okay,","", "All cool?","", "If not, press R or pinch for two seconds to recalibrate" };
		float[] timeout = new float[]{ 2,0.2f, 2,0.2f, 5 };
		float t=PlayText (text,timeout);

		cooldownCoroutine=StartCoroutine(StartSceneCooldown(t));
	}

	void OnCalibrationReset(bool failure)
	{
		if (!_firstCalibration) {
			string[] text;
			float[] timeout;
			if (!failure) {
				text = new string[]{ "Alright, lets do this again", "", "Move your hand to the point and pinch" };
				timeout = new float[]{ 2, 0.2f, 10 };
			}
			else {
				text = new string[]{ "Something went wrong, lets do this again" };
				timeout = new float[]{ 5 };
			}
			PlayText (text, timeout);
		} else
			OnStartCalibration ();
	}

	void OnCalibrationPoint(Vector3 p)
	{
		string[] text = new string[]{ "Thats it" };
		float[] timeout = new float[]{ 1 };
		PlayText (text,timeout);
	}

	void Reset()
	{
		HandCalibrator.Reset ();
		targetCamera.Reset ();
		targetCamera.StopBehaviour ();
		Environment.Reset ();
		_currentState = State.Calibration;
		if (cooldownCoroutine != null) {
			StopCoroutine (cooldownCoroutine);
			cooldownCoroutine = null;
		}
	}

	void StartScene()
	{
		targetCamera.StartBehaviour ();
		Environment.StartEnvironment ();
		_currentState = State.Idle;


		string[] text = new string[]{ 
			"","This is LightRoom (not Adobe)",
			"", "Your face is tracked using WebCam for OffAxis rendering",
			"","You can add attractors by pinching",
			"","You can reset any time using R button",
			"", "Enjoy.." };
		float[] timeout = new float[]{ 
			2,5,
			0.2f,7,
			0.2f,5,
			0.2f,5,
			0.2f,4
		};
		float t=PlayText (text,timeout);

		PlayDetails (new string[]{ "", "Particles are GPU Instanced, and processed in the shader " },
			new float[]{ t, 10 });

	}


	bool CheckReset()
	{
		return Input.GetKeyDown (KeyCode.R) || HandCalibrator.IsPinched && HandCalibrator.PinchTime>3 && HandCalibrator.CalibrationPointNumber>0;
	}

	// Update is called once per frame
	void Update () {
		switch (_currentState) {
		case State.Calibration:
			if (CheckReset()) {
				Reset ();
			}
			break;
		case State.Idle:
			{
				if (Input.GetKeyDown (KeyCode.R)) {
					Reset ();
				}
				break;
			}
		}


		_colorTween.Step ();
		if (Mathf.Abs (_colorTween.position - _colorTween.target) < 0.05f) {
		//	if (_lastTween == _colorTween.target) {
				_colorTween.position=_colorTween.target;
				_colorTween.target = 1 - _colorTween.target;
				_lastTween = _colorTween.target;
		//	}
		}

		Color1.a = Color2.a=_currAlpha;
		InstructionText.color = Color.Lerp (Color1, Color2, _colorTween.position);
	}
}

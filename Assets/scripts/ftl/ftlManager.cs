using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;

public class ftlManager : MonoBehaviour {

	public static Dictionary <int, List<ftlTouch>> ftlTouches = new Dictionary<int, List<ftlTouch>>();
	public static Dictionary <int, AnalysisTouch> analysisTouches = new Dictionary<int, AnalysisTouch>();
	public static Dictionary <string, DirectoryHolder> directoryHolders = new Dictionary <string, DirectoryHolder>();
	public static Dictionary <int, ftlTouch> graphPoints = new Dictionary <int, ftlTouch>();
	public static GameObject FolderButton;
	public static GameObject vector;
	public static GameObject readoutBox;
	public static GameObject WINDOW;
	public static string SelectedDirectory = "";
	public static Color Invisible = new Color (0f, 0f, 0f, 0f);
	public static Color ButtonNormal = new Color (1f, 1f, 1f, 1f);
	public static Color ButtonGrey = new Color (1f, 1f, 1f, 0.5f);
	public static Color ButtonPressed = new Color (0.3f, 1.0f, 0.3f, 1f);
	public static GUIStyle buttonText;
	public static GUIStyle messageText;
	public static GUIStyle warningText;
	public static bool RealTime = true;
	public static bool Looping = false;
	public static float GRAPH_TIME_ELAPSED = 0;
	public static float START_TIME = 0;
	public static float CURRENT_TIME = 0;
	public static float MIN_TIME = 0;
	public static float MAX_TIME = 0;
	public static float MIN_TIME_HARD = 0;
	public static float MAX_TIME_HARD = 0;
	public static string USER_MESSAGE = "Experiment Mode";
	public static string USER_WARNING = "";
	public static Texture2D FilesBG;
	public static float panGraph = 0;
	public static bool savingSubset = false;
	public static Color[] colors = new Color[10];
	public static bool savedDataAvailableToLoad = false;
	public static Font GameFont;
	//public static float yScale = 20f;

	// ADD COLOR LOOKUP TABLE WITH 10 COLORS!!!

	void Awake()
	{
		buttonText = new GUIStyle();
		buttonText.alignment = TextAnchor.MiddleCenter;
		buttonText.fontSize = 16;

		messageText = new GUIStyle();
		messageText.alignment = TextAnchor.MiddleCenter;
		messageText.normal.textColor = new Color ( 0.35f, 0.239f, 0.024f, 1f);
		messageText.fontSize = 16;

		warningText = new GUIStyle();
		warningText.alignment = TextAnchor.MiddleCenter;
		warningText.normal.textColor = new Color ( 0.35f, 0.239f, 0.024f, 1f);
		warningText.fontSize = 12;
//		var left = Camera.main.ScreenToWorldPoint (new Vector3 (0f, 0f, 0f));
//		var extentX = GameObject.Find ("LeftControls").GetComponent<Collider> ().bounds.extents.x * 1.2f;
//		left.x += extentX;
//		left.y = -4.5f;
//		left.z = 0f;
//		GameObject.Find ("LeftControls").transform.position = left;
//
//		var right = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, 0f, 0f));
//		right.x -= extentX;
//		right.y = -4.5f;
//		right.z = 0f;
//		GameObject.Find ("Loop").transform.position = right;
	}

	public bool IsRealTime ()
	{
		return RealTime;
	}
	public Vector3 transformToObject (Vector3 _vertex, GameObject _object)
	{
		var ratio = new Vector2 (_object.GetComponent<Collider>().bounds.size.x / Screen.width , _object.GetComponent<Collider>().bounds.size.y / Screen.height);
		_vertex.x = _object.GetComponent<Collider>().bounds.center.x + (ratio.x * _vertex.x) - _object.GetComponent<Collider>().bounds.size.x / 2;
		_vertex.y = -_object.GetComponent<Collider>().bounds.center.y + (ratio.y * _vertex.y) - _object.GetComponent<Collider>().bounds.size.y / 2;
		_vertex = Camera.main.WorldToScreenPoint (_vertex);		
		return _vertex;
	}

	public Vector3 transformToRendererObject (Vector3 _vertex, GameObject _object)
	{
		var ratio = new Vector2 (_object.GetComponent<SpriteRenderer>().bounds.size.x / Screen.width , _object.GetComponent<SpriteRenderer>().bounds.size.y / Screen.height);
		_vertex.x = _object.GetComponent<SpriteRenderer>().bounds.center.x + (ratio.x * _vertex.x) - _object.GetComponent<SpriteRenderer>().bounds.size.x / 2;
		_vertex.y = -_object.GetComponent<SpriteRenderer>().bounds.center.y + (ratio.y * _vertex.y) - _object.GetComponent<SpriteRenderer>().bounds.size.y / 2;
		_vertex = Camera.main.WorldToScreenPoint (_vertex);		
		return _vertex;
	}

	public Vector3 transformToWindow (Vector3 _vertex, GameObject _object)
	{
		var ratio = new Vector2 (_object.GetComponent<SpriteRenderer>().bounds.size.x / Screen.width , _object.GetComponent<SpriteRenderer>().bounds.size.y / Screen.height);
		_vertex.x = _object.GetComponent<SpriteRenderer>().bounds.center.x + (ratio.x * _vertex.x) - _object.GetComponent<SpriteRenderer>().bounds.size.x / 2;
		_vertex.y = _object.GetComponent<SpriteRenderer>().bounds.center.y + (ratio.y * _vertex.y) - _object.GetComponent<SpriteRenderer>().bounds.size.y / 2;
		//_vertex = Camera.main.WorldToScreenPoint (_vertex);		
		return _vertex;
	}

	public GameObject GetWindow ()
	{	
		return WINDOW;
	}

	public Color GetColor (int _Id , float _brightness , float _alpha)
	{
//		var rFactor = 3f;
//		var gFactor = 1f;
//		var bFactor = 1f;
//		var _color = new Color ((Mathf.Sin ((_Id * 71f) + 17)), (Mathf.Sin ((_Id * 19f) - 25)), (Mathf.Sin ((_Id + 5) * 23f)), 1f);
//		_color = new Color (rFactor * _brightness * _color.r , gFactor * _brightness * _color.g , bFactor * _brightness * _color.b , _alpha);
		var i = _Id % colors.Length;
		var _color = colors [i];
		_color.r *= _brightness;
		_color.g *= _brightness;
		_color.b *= _brightness;
		_color.a = _alpha;
		return _color;
	}

	public string GetNewDirName ()
	{
		var newDirManager = new DirectoryManager();
		return newDirManager.GetNewDirectoryName ();
	}

	public Dictionary<string, DirectoryHolder> GetAllFolders ()
	{
		foreach (var folder in directoryHolders)
		{
			Destroy(folder.Value.Button);
		}
		directoryHolders.Clear ();
		var newDirManager = new DirectoryManager();
		newDirManager.SetDictionary = directoryHolders;
		return newDirManager.GetAllDirectories (FolderButton);
	}

	public void Clear()
	{
		ftlTouches.Clear ();
		graphPoints.Clear ();
		foreach (var line in GameObject.FindGameObjectsWithTag("Line"))
			Destroy (line);
		foreach (var _readout in GameObject.FindGameObjectsWithTag("ReadoutButton"))
		{
			_readout.GetComponent<ReadoutButton>().DestroyReadout();
			Destroy (_readout);
		}
		START_TIME = Time.realtimeSinceStartup;
		foreach (var sgObject in GameObject.FindGameObjectsWithTag("Graph"))
		{
			var sg = sgObject.GetComponent<ftlSpeedGraph>();
			sg.yScale = 20f;
		}
	}

	public void HideSliders ()
	{
		foreach (var glidePart in GameObject.FindGameObjectsWithTag ("Glides"))
			glidePart.GetComponent<ButtonSliders> ().SlideOut ();
	}

	public void ShowSliders ()
	{
		var maxPos = GameObject.FindGameObjectWithTag ("SliderMax").transform.position;
		maxPos.x = 5.87f;
		GameObject.FindGameObjectWithTag ("SliderMax").transform.position = maxPos;
		GameObject.FindGameObjectWithTag ("SliderMax").GetComponent<SpriteRenderer> ().color = ButtonNormal;
		GameObject.FindGameObjectWithTag ("SliderMax").GetComponent<Collider>().enabled = true;

		var minPos = GameObject.FindGameObjectWithTag ("SliderMax").transform.position;
		minPos.x = -4.121f;
		GameObject.FindGameObjectWithTag ("SliderMin").transform.position = minPos;
		GameObject.FindGameObjectWithTag ("SliderMin").GetComponent<SpriteRenderer> ().color = ButtonNormal;
		GameObject.FindGameObjectWithTag ("SliderMin").GetComponent<Collider>().enabled = true;
		foreach (var glidePart in GameObject.FindGameObjectsWithTag ("Glides"))
			glidePart.GetComponent<ButtonSliders> ().SlideIn ();
	}

	public void SetupInitialRealTime()
	{
		if (GameObject.Find ("Analysis"))
			GameObject.Find ("Analysis").GetComponent<SpriteRenderer> ().color = ButtonGrey;
		if (GameObject.Find ("Save"))
			GameObject.Find ("Save").GetComponent<ButtonSliders> ().Hide ();
		if (GameObject.Find ("Loop"))
			GameObject.Find ("Loop").GetComponent<ButtonSliders> ().Hide ();
		if (GameObject.Find ("Clear"))
			GameObject.Find ("Clear").GetComponent<ButtonSliders> ().Hide ();
		if (GameObject.Find ("Select"))
			GameObject.Find ("Select").GetComponent<ButtonSliders> ().Hide ();
		if (GameObject.Find ("Load"))
		{
			if (savedDataAvailableToLoad == true)
			{
				GameObject.Find ("Load").GetComponent<ButtonSliders>().Hide ();
				System.Threading.Thread.Sleep (100);
				GameObject.Find ("Load").GetComponent<SpriteRenderer> ().color = ButtonNormal;
				GameObject.Find ("Load").GetComponent<ButtonSliders>().SlideIn();
			}
			else
				GameObject.Find ("Load").GetComponent<ButtonSliders> ().Hide ();
		}
		
		foreach (var glidePart in GameObject.FindGameObjectsWithTag ("Glides"))
			glidePart.GetComponent<ButtonSliders> ().Hide ();
		Looping = false;
		savingSubset = false;
	}

	public void SetupRealTime()
	{
		if (GameObject.Find ("Analysis"))
			GameObject.Find ("Analysis").GetComponent<SpriteRenderer> ().color = ButtonGrey;
		if (GameObject.Find ("Save"))
			GameObject.Find ("Save").GetComponent<ButtonSliders> ().Hide ();
		if (GameObject.Find ("Clear"))
			GameObject.Find ("Clear").GetComponent<ButtonSliders> ().Hide ();
		if (GameObject.Find ("Select"))
			GameObject.Find ("Select").GetComponent<ButtonSliders> ().SlideOut ();
		if (GameObject.Find ("Loop"))
			GameObject.Find ("Loop").GetComponent<ButtonSliders> ().SlideOut ();

		HideSliders ();
		Looping = false;
		savingSubset = false;
	}

	public void SetupAnalysis()
	{
		if (GameObject.Find ("Experiment"))
			GameObject.Find ("Experiment").GetComponent<SpriteRenderer> ().color = ButtonNormal;
		Looping = false;
		savingSubset = true;
	}

	public void ChangeGraphTimeScale(bool _default)
	{
		var duration = (Screen.width) / (MAX_TIME - MIN_TIME);
		if (_default)
			duration = 200f;
		foreach (var graph in GameObject.FindGameObjectsWithTag("Graph"))
		{
			graph.GetComponent<ftlSpeedGraph>().TimeScaleModifier = duration;
		}
	}
}

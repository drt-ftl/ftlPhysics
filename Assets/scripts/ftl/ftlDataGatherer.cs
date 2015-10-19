
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using TouchScript.Hit;


public class ftlDataGatherer : ftlManager 
{
	public GameObject analysisTouchObject;
	public GameObject folderButton;
	public GameObject displayVector;
	public GameObject readout;
	public GameObject windowBox;
	private bool hotKeyPressed = false;
	public GameObject messageContainer;
	public GameObject warningContainer;
	public Texture2D filesBG;
	public bool BezelTV;
	public bool separateMouse;
	public Color[] touchColors = new Color[10];
	public Font gameFont;
	public AudioSource firstTouchSound;
	public AudioSource SliderSound;
	public float sliderSoundVolume;
    public Sprite tvBg;
    public Sprite panelBg;
//	private Dictionary <int, GameObject> scribblers = new Dictionary<int, GameObject>();


	void Awake () 
	{
        var bg = GameObject.Find("BG");
        if (BezelTV)
            bg.GetComponent<SpriteRenderer>().sprite = tvBg;
        else
            bg.GetComponent<SpriteRenderer>().sprite = panelBg;
        GameFont = gameFont;
		ftlTouches.Clear ();
		graphPoints.Clear ();
		FolderButton = folderButton;
		vector = displayVector;
		readoutBox = readout;
		WINDOW = windowBox;
		FilesBG = filesBG;
		var c = WINDOW.GetComponent<SpriteRenderer>().color;
//		if (!BezelTV)
//			c.a = 1.0f;
//		else
//			c.a = 0f;
//		WINDOW.GetComponent<SpriteRenderer>().color = c;
		//WINDOW.GetComponent<Collider>().enabled = !BezelTV;
		colors = touchColors;
		buttonText.font = GameFont;
		messageText.font = GameFont;
		warningText.font = GameFont;

		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan += OnTouchesBegan;
			TouchManager.Instance.TouchesEnded += OnTouchesEnded;
			TouchManager.Instance.TouchesMoved += OnTouchesMoved;
			TouchManager.Instance.TouchesCancelled += OnTouchesCancelled;
		}

		var dirInfo = new System.IO.DirectoryInfo (Application.dataPath).GetDirectories();
		var dirExists = false;
		foreach (var dir in dirInfo)
		{
			if (dir.Name == "logs" && dir.GetDirectories().Length >= 1)
				dirExists = true;
		}
		if (dirExists)
		{
			savedDataAvailableToLoad = true;
			GameObject.Find ("Load").GetComponent<SpriteRenderer> ().color = ButtonNormal;
		}
		else
		{
			savedDataAvailableToLoad = false;
			//GameObject.Find ("Load").GetComponent<SpriteRenderer> ().color = ButtonGrey;
		}
	}

	void OnEnable()
	{
        SetupInitialRealTime ();
	}

	private void OnGUI()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
		messageText.font = GameFont;
		var centroid = transformToObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , messageContainer);
		var GuiRect =  ( new Rect (centroid.x - 100, centroid.y - 50, 200 , 100));
		GUI.color = new Color (0f, 0f, 0f, 0.8f);
		GUI.Label (GuiRect, USER_MESSAGE, messageText);

		centroid = transformToObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , warningContainer);
		GuiRect =  ( new Rect (centroid.x - 100, centroid.y - 50, 200 , 100));
		GUI.color = new Color (0.4f, 0.4f, 0.4f, 0.8f);
		warningText.font = GameFont;
		GUI.Label (GuiRect, USER_WARNING, warningText);

		if (RealTime)
			MAX_TIME = Time.realtimeSinceStartup - START_TIME + 0.1f;
		if (Looping && Time.realtimeSinceStartup - START_TIME >= MAX_TIME)
		{
			START_TIME = Time.realtimeSinceStartup - MIN_TIME;
		}
		if (Time.realtimeSinceStartup - START_TIME <= MAX_TIME)
		{
			CURRENT_TIME = Time.realtimeSinceStartup - START_TIME;
		}
	}

	void Update()
	{
        #region Hotkeys
        if (RealTime && GameObject.Find("Clear").transform.position.x >= Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x)
            GameObject.Find("Clear").GetComponent<ButtonSliders>().SlideIn();

		if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
		{
			if (Input.GetKeyDown (KeyCode.C)) 
				OnHotkey("Clear");
			if (Input.GetKeyDown (KeyCode.S)) 
				OnHotkey("Save");
			if (Input.GetKeyDown (KeyCode.A)) 
				OnHotkey("Analysis");
			if (Input.GetKeyDown (KeyCode.E)) 
				OnHotkey("Experiment");
			if (Input.GetKeyDown (KeyCode.R)) 
				OnHotkey("Loop");
			if (Input.GetKeyDown (KeyCode.L)) 
				OnHotkey("Load");
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) 
			OnHotkey("Back");
		if (Input.GetKeyDown (KeyCode.Return)) 
			OnHotkey("Select");
		#endregion
	}

	private void OnTouchesBegan(object sender, TouchEventArgs e)
	{
		if (!enabled) return;
		foreach (var touch in e.Touches)
		{
			if (!RealTime && 
			    (touch.Hit == null || (touch.Hit != null && touch.Hit.Transform.tag == "Line")))// && (!separateMouse) || (separateMouse && touch.Tags.HasTag("Mouse")))
			{
				var newAnalysisTouch = new AnalysisTouch();
				newAnalysisTouch.iTouch = touch;
				newAnalysisTouch.SetDictionary  = analysisTouches;
				var position = Camera.main.ScreenToWorldPoint (touch.Position);
				position.z = 0;
				newAnalysisTouch.TouchObject = Instantiate (analysisTouchObject, position, Quaternion.identity) as GameObject;
				newAnalysisTouch.TouchObject.name = "TouchObject";
				newAnalysisTouch.Add ();
			}
			else if (touch.Hit != null)// && (!separateMouse || (separateMouse && touch.Tags.HasTag("Mouse"))))
			{
				if (touch.Hit.Transform.tag == "SliderMin")
				{
					if (GameObject.Find ("Save"))
					{
						GameObject.Find ("Save").GetComponent<ButtonSliders>().SlideIn();
					}
					touch.Hit.Transform.name = "Min_" + touch.Id.ToString();
					var touchPosition = Camera.main.ScreenToWorldPoint (touch.Position);
					var sliderPosition = touch.Hit.Transform.transform.position;
					sliderPosition.x = touchPosition.x;
					if (touchPosition.x >= -4.121f) // Should be -4.3187 ??
					{
						var timeSpan = MAX_TIME_HARD - MIN_TIME_HARD;
						var fullSpaceBetween = 9.991f;
						var minSliderNormalized = (touchPosition.x + 4.121f) / fullSpaceBetween;
						MIN_TIME = MIN_TIME_HARD + minSliderNormalized * timeSpan;
						touch.Hit.Transform.transform.position = sliderPosition;
					}
				}
				else if (touch.Hit.Transform.tag == "SliderMax")
				{
					if (GameObject.Find ("Save"))
						GameObject.Find ("Save").GetComponent<ButtonSliders>().SlideIn();
					touch.Hit.Transform.name = "Max_" + touch.Id.ToString();
					var touchPosition = Camera.main.ScreenToWorldPoint (touch.Position);
					var sliderPosition = touch.Hit.Transform.transform.position;
					sliderPosition.x = touchPosition.x;
					if (touchPosition.x <= 4.121f)
					{
						var timeSpan = MAX_TIME_HARD - MIN_TIME_HARD;
						var fullSpaceBetween = 9.991f;
						var maxSliderNormalized = (touchPosition.x + 4.121f) / fullSpaceBetween;
						MAX_TIME = MIN_TIME_HARD + maxSliderNormalized * timeSpan;
						touch.Hit.Transform.transform.position = sliderPosition;
					}
				}
				else if (touch.Hit.Transform.tag == "ReadoutButton")
				{
					touch.Hit.Transform.GetComponent<ReadoutButton>().DestroyReadout();
					var gpId = touch.Hit.Transform.GetComponent<ReadoutButton>().SetDataDisplay.GraphPointId;
					graphPoints.Remove(gpId);
					Destroy(touch.Hit.Transform.gameObject);
				}
			}
			else if (RealTime)
			{
				var clip = firstTouchSound.clip;
				firstTouchSound.PlayOneShot(clip);
				USER_MESSAGE = "Experiment Mode";
				USER_WARNING = "";
				if (ftlTouches.Count == 0)
				{
					START_TIME = Time.realtimeSinceStartup;
					CURRENT_TIME = 0;
					MIN_TIME_HARD = CURRENT_TIME;
					MIN_TIME = MIN_TIME_HARD;
				}
				if (!separateMouse || (separateMouse && touch.Tags.HasTag("Touch")))
				{
					if (ftlTouches.Count == 0)
					{
						GameObject.Find ("Save").GetComponent<ButtonSliders>().SlideIn();
						GameObject.Find ("Clear").GetComponent<ButtonSliders>().SlideIn();
						GameObject.Find ("Analysis").GetComponent<SpriteRenderer> ().color = ButtonNormal;
					}
					var newFtlTouch = new ftlTouch();
					newFtlTouch.EventTime = CURRENT_TIME;
					newFtlTouch.iTouch = touch;
					newFtlTouch.SetDictionary = ftlTouches;
					if (ftlTouches.ContainsKey (touch.Id - 1))
					{
						var lastFtlTouch = ftlTouches[touch.Id - 1][ftlTouches[touch.Id - 1].Count - 1];
						if (lastFtlTouch.iTouch.Position.x == touch.Position.x)
						{
							if (lastFtlTouch.iTouch.Position.y < touch.Position.y && lastFtlTouch.iTouch.Position.y < 50f)
								lastFtlTouch.Discard = true;
							else if (touch.Position.y < lastFtlTouch.iTouch.Position.y && newFtlTouch.iTouch.Position.y < 50f)
								newFtlTouch.Discard = true;
						}
					}
					newFtlTouch.CamObject = gameObject;
					newFtlTouch.Add (BezelTV);
				}
			}
		}
	}

	private void OnTouchesMoved(object sender, TouchEventArgs e)
	{
		if (!enabled) return;
		foreach (var touch in e.Touches)
		{
			if (!RealTime)
			{
				if (touch.Hit != null && touch.Hit.Transform != null && touch.Hit.Transform.GetComponent<ftlSpeedGraph>() != null)
						panGraph -= (touch.Position.x - touch.PreviousPosition.x);
				if (analysisTouches.ContainsKey(touch.Id))
				{
					var position = Camera.main.ScreenToWorldPoint (touch.Position);
					position.z = 0;
					analysisTouches[touch.Id].TouchObject.transform.position = position;
				}

				if (GameObject.Find ("Min_" + touch.Id.ToString()))
				{
					if (!SliderSound.isPlaying)
						SliderSound.Play ();
					SliderSound.volume = sliderSoundVolume;
					var touchPosition = Camera.main.ScreenToWorldPoint (touch.Position);
					var min = GameObject.Find ("Min_" + touch.Id.ToString());
					var sliderPosition = min.transform.position;
					sliderPosition.x = touchPosition.x;
					var maxPos = GameObject.FindGameObjectWithTag("SliderMax");
					if (touchPosition.x >= -4.121f && touchPosition.x < (maxPos.transform.position.x - 0.2f))
					{
						var timeSpan = MAX_TIME_HARD - MIN_TIME_HARD;
						var fullSpaceBetween = 9.991f;
						var minSliderNormalized = (touchPosition.x + 4.121f) / fullSpaceBetween;
						MIN_TIME = MIN_TIME_HARD + minSliderNormalized * timeSpan;
						min.transform.position = sliderPosition;
					}
				}
				if (GameObject.Find ("Max_" + touch.Id.ToString()))
				{
					if (!SliderSound.isPlaying)
						SliderSound.Play ();
					SliderSound.volume = sliderSoundVolume;
					var touchPosition = Camera.main.ScreenToWorldPoint (touch.Position);
					var max = GameObject.Find ("Max_" + touch.Id.ToString());
					var sliderPosition = max.transform.position;
					sliderPosition.x = touchPosition.x;
					var minPos = GameObject.FindGameObjectWithTag("SliderMin");
					if (touchPosition.x <= 5.87f && touchPosition.x > (minPos.transform.position.x + 0.2f))
					{
						var timeSpan = MAX_TIME_HARD - MIN_TIME_HARD;
						var fullSpaceBetween = 9.991f;
						var maxSliderNormalized = (touchPosition.x + 4.121f) / fullSpaceBetween;
						MAX_TIME = MIN_TIME_HARD + maxSliderNormalized * timeSpan;
						max.transform.position = sliderPosition;
					}
				}
			}
			else if (RealTime)
			{
				if (!separateMouse || (separateMouse && touch.Tags.HasTag("Touch")))
				{
					var newFtlTouch = new ftlTouch();
					newFtlTouch.EventTime = Time.realtimeSinceStartup -START_TIME;
					newFtlTouch.iTouch = touch;
					newFtlTouch.SetDictionary = ftlTouches;
					if (ftlTouches.ContainsKey (touch.Id - 1))
					{
						var lastFtlTouch = ftlTouches[touch.Id - 1][ftlTouches[touch.Id - 1].Count - 1];
						if (lastFtlTouch.iTouch.Position.x == touch.Position.x)
						{
							if (lastFtlTouch.iTouch.Position.y < touch.Position.y && lastFtlTouch.iTouch.Position.y < 50f)
								lastFtlTouch.Discard = true;
							else if (touch.Position.y < lastFtlTouch.iTouch.Position.y && newFtlTouch.iTouch.Position.y < 50f)
								newFtlTouch.Discard = true;
						}
					}
					newFtlTouch.CamObject = gameObject;
					newFtlTouch.Add (BezelTV);
				}
			}
		}
	}

	private void OnTouchesEnded(object sender, TouchEventArgs e)
	{
		if (!enabled) return;
		foreach (var touch in e.Touches)
		{
			if (analysisTouches.ContainsKey(touch.Id))
			{
				Destroy (analysisTouches[touch.Id].TouchObject);
				analysisTouches.Remove(touch.Id);
			}
			if (touch.Hit != null)
			{
				if (touch.Hit.Transform.tag.Contains("Slider"))
				{
					SliderSound.volume = 0;
					SliderSound.Stop ();
				}
			}
			if (ftlTouches.ContainsKey(touch.Id) && RealTime)
			{
				var newFtlTouch = new ftlTouch();
				newFtlTouch.EventTime = CURRENT_TIME;
				newFtlTouch.iTouch = touch;
				newFtlTouch.SetDictionary = ftlTouches;
				newFtlTouch.CamObject = gameObject;
				newFtlTouch.Add (BezelTV);
			}
		}
	}

	private void OnTouchesCancelled(object sender, TouchEventArgs e)
	{
		OnTouchesEnded(sender, e);
	}

	private void OnTouchedObject (GameObject hit)
	{
		var newCheckButtons = new CheckButtons ();
		newCheckButtons.PressButton (hit);
	}

	private void OnReleasedObject (GameObject hit)
	{
		var newCheckButtons = new CheckButtons ();
		newCheckButtons.ReleaseButton (hit);
	}

	private void OnHotkey (string hit)
	{
		var newCheckHotkeys = new CheckHotkeys ();
		newCheckHotkeys.PressButton (hit);
	}

	public void Pause (bool showSliders)
	{
		if (!showSliders) // Switching to Analysis Mode
			HideSliders ();
		else
			ShowSliders ();
		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan -= OnTouchesBegan;
			TouchManager.Instance.TouchesEnded -= OnTouchesEnded;
			TouchManager.Instance.TouchesMoved -= OnTouchesMoved;
			TouchManager.Instance.TouchesCancelled -= OnTouchesCancelled;
		}
		System.Threading.Thread.Sleep (10);

		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan += OnTouchesBegan;
			TouchManager.Instance.TouchesEnded += OnTouchesEnded;
			TouchManager.Instance.TouchesMoved += OnTouchesMoved;
			TouchManager.Instance.TouchesCancelled += OnTouchesCancelled;
		}
	}
}

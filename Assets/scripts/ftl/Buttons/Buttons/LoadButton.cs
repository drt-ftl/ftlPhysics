using UnityEngine;
using System.Collections;
using System.IO;

public class LoadButton : ftlManager 
{
	public void press ()
	{
		Camera.main.GetComponent<ftlDataGatherer> ().Pause (false);
		RealTime = false;
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		SelectedDirectory = Application.dataPath + "/logs";
		GetComponent<GeneralButton> ().release();
		if (Directory.Exists(Application.dataPath + "/logs"))
		{
			GetAllFolders ();
		}
		else
		{
			if (GameObject.Find ("Analysis"))
				GameObject.Find ("Analysis").GetComponent<SpriteRenderer> ().color = ButtonGrey;
			USER_WARNING = "No Saved Files To Load";
			Camera.main.GetComponent<ftlDataGatherer> ().Pause (false);
			RealTime = true;
		}
		SetupAnalysis ();
		USER_MESSAGE = "Browsing";
		USER_WARNING = "";
		if (GameObject.Find ("Clear"))
			GameObject.Find ("Clear").GetComponent<ButtonSliders> ().SlideOut ();
		if (GameObject.Find ("Save"))
			GameObject.Find ("Save").GetComponent<ButtonSliders> ().SlideOut ();
		if (GameObject.Find ("Loop"))
			GameObject.Find ("Loop").GetComponent<ButtonSliders> ().SlideOut ();
		Clear ();
	}
}

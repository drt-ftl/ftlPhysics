using UnityEngine;
using System.Collections;

public class RealTimeButton : ftlManager 
{

	public void press ()
	{
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		if (RealTime)
			return;
		Camera.main.GetComponent<ftlDataGatherer> ().Pause (false);
		SetupRealTime ();
		RealTime = true;
		GetComponent<GeneralButton> ().release();
		HideSliders ();
		foreach (var folder in GameObject.FindGameObjectsWithTag("FolderButton"))
			Destroy (folder);
		directoryHolders.Clear ();
		USER_MESSAGE = "Experiment Mode";
		USER_WARNING = "";
		MIN_TIME = 0;
		Clear ();
		if (!GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().isPlaying)
			GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().PlayOneShot (GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().clip);
		ChangeGraphTimeScale (true);
	}
}

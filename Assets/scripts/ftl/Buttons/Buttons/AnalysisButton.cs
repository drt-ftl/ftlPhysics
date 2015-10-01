using UnityEngine;
using System.Collections;

public class AnalysisButton : ftlManager 
{

	public void press ()
	{
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		if (!RealTime)
			return;
		if (GameObject.Find ("Clear"))
			GameObject.Find ("Clear").GetComponent<ButtonSliders> ().SlideOut ();
//		if (GameObject.Find ("Save"))
//			GameObject.Find ("Save").GetComponent<ButtonSliders> ().SlideOut ();
		if (GameObject.Find ("Loop")) 
		{
			GameObject.Find ("Loop").GetComponent<ButtonSliders> ().SlideIn ();
			GameObject.Find ("Loop").GetComponent<SpriteRenderer>().color = ButtonNormal;
		}
		Looping = false;
		Camera.main.GetComponent<ftlDataGatherer> ().Pause (false);
		RealTime = false;
		GetComponent<GeneralButton> ().release();
		MAX_TIME_HARD = CURRENT_TIME;
		MAX_TIME = MAX_TIME_HARD;
		Camera.main.GetComponent<ftlDataGatherer> ().Pause(true);
		SetupAnalysis ();
		RealTime = false;
		//gameObject.name = "Experiment";
		USER_MESSAGE = "Analysis Mode";
		if (!GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().isPlaying)
			GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().PlayOneShot (GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().clip);
		ChangeGraphTimeScale (false);
	}
}

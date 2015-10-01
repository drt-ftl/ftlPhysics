using UnityEngine;
using System.Collections;
using System.IO;

public class ClearButton : ftlManager 
{	

	public void press ()
	{
		Camera.main.GetComponent<ftlDataGatherer> ().Pause (false);
		RealTime = false;
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		GetComponent<GeneralButton> ().release();
		Clear ();
		Camera.main.GetComponent<ftlDataGatherer> ().Pause (false);
		RealTime = true;
		if (GameObject.Find ("Save"))
			GameObject.Find ("Save").GetComponent<ButtonSliders>().SlideOut();
		GetComponent<ButtonSliders>().SlideOut();
	}
}
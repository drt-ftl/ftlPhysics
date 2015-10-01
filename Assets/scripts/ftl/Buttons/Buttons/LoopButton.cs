using UnityEngine;
using System.Collections;

public class LoopButton : ftlManager 
{	
	void OnEnable()
	{
		GetComponent<ButtonSliders> ().Hide ();
	}

	public void press ()
	{
		if (!Looping)
		{
			GetComponent<GeneralButton> ().press ();
			Looping = true;
			ChangeGraphTimeScale(false);
		}
		else
		{
			GetComponent<GeneralButton> ().release ();
			if (!Camera.main.GetComponent<AudioSource> ().isPlaying)
				Camera.main.GetComponent<AudioSource> ().PlayOneShot (Camera.main.GetComponent<AudioSource> ().clip);
			Looping = false;
		}
	}
	
	public void release ()
	{
	}
}
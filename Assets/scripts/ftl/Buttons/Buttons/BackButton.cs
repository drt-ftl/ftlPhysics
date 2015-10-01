using UnityEngine;
using System.Collections;

public class BackButton : ftlManager 
{
	public void press ()
	{
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		HideSliders ();
		GetComponent<GeneralButton> ().release();
		GameObject.Find ("Select").GetComponent<ButtonSliders> ().SlideOut ();
		if (!directoryHolders.ContainsKey(SelectedDirectory))
			return;
		var nextDirUp = directoryHolders [SelectedDirectory].DirParent;
		if (nextDirUp != Application.dataPath)
		{
			foreach (var folder in directoryHolders)
			{
				if (folder.Value.DirParent == nextDirUp)
				{
					folder.Value.Button.GetComponent<SpriteRenderer>().color = ButtonNormal;
				}
				else
				{
					folder.Value.Button.GetComponent<SpriteRenderer>().color = Invisible;
				}
			}
			SelectedDirectory = nextDirUp;
		}
		if (SelectedDirectory == Application.dataPath + "/logs")
		 GetComponent<SpriteRenderer> ().color = ButtonGrey;
		GameObject.Find ("Loop").GetComponent<ButtonSliders> ().SlideOut ();
		GameObject.Find ("Save").GetComponent<ButtonSliders> ().SlideOut ();
		//GameObject.Find ("Select").GetComponent<ButtonSliders> ().SlideIn ();
		USER_MESSAGE = "";
		USER_WARNING = "";
		MIN_TIME = 0;
		Clear ();
	}

}

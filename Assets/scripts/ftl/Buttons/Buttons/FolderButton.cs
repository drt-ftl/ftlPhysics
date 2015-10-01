using UnityEngine;
using System.Collections;
using System.IO;

public class FolderButton : ftlManager 
{
	private DirectoryInfo dirInfo;
	public static bool shouldShow = false;
	void OnGUI()
	{
		if (SelectedDirectory == name)
		{
			var files = directoryHolders[name].ListOfFiles;
			var centroid = transformToRendererObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , GameObject.Find ("FileSlot"));
			var GuiRect =  ( new Rect (centroid.x - 100, centroid.y - 12.5f, 200 , 25));
			var startRect = GuiRect;
			GUI.color = new Color (1f, 1f, 1f, 1f);
			if (shouldShow)
			{
				foreach (var file in files)
				{
					var newText = new GUIStyle();
					newText.alignment = TextAnchor.MiddleCenter;
					newText.fontSize = 16;
					newText.normal.background = FilesBG;
					GUI.Label (GuiRect, file.Name, newText);
					if (GuiRect.y < 1000)
					{
						GuiRect.y += 20f;
					}
					else
					{
						GuiRect.y = startRect.y;
						GuiRect.x += 100f;
					}
				}
			}
		}
	}

	public void ShouldNotShow ()
	{
		shouldShow = false;
	}

	public void press ()
	{
		GetComponent<GeneralButton> ().press ();
		shouldShow = true;
	}
	
	public void release ()
	{
		GetComponent<GeneralButton> ().release();
		GameObject.Find ("Back").GetComponent<SpriteRenderer> ().color = ButtonNormal;
		//GameObject.Find ("Load").GetComponent<SpriteRenderer> ().color = ButtonGrey;
		SelectedDirectory = name;
		foreach (var button in directoryHolders)
		{
			if (button.Value.DirParent == name)
			{
				button.Value.Button.GetComponent<SpriteRenderer>().color = ButtonNormal;
			}
			else
			{
			button.Value.Button.GetComponent<SpriteRenderer>().color = Invisible;
			}
		}
		var newDirInfo = new DirectoryInfo (name);
		if (newDirInfo.GetDirectories().Length < 1)
		{
			GameObject.Find ("Select").GetComponent<SpriteRenderer>().color = ButtonNormal;
			GameObject.Find ("Select").GetComponent<ButtonSliders>().SlideIn();
		}
		else
		{
			GameObject.Find ("Select").GetComponent<SpriteRenderer>().color = ButtonGrey;
			GameObject.Find ("Select").GetComponent<ButtonSliders>().SlideOut();
		}
	}

}

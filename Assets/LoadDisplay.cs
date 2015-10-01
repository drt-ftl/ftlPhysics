using UnityEngine;
using System.Collections;
using System.IO;

public class LoadDisplay : ftlManager
{
	Rect GuiRect;
	GUIStyle style;
	void OnEnable()
	{
		style = buttonText;
	}
	void OnGUI()
	{
		style.normal.textColor = new Color ( 0f, 0f, 0f, 0.5f);
		var centroid = transformToObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , gameObject);
		GuiRect =  new Rect (centroid.x - 100, centroid.y - 50, 200 , 100);
		var text = "";
		if (Directory.Exists (SelectedDirectory)) {
			var info = new DirectoryInfo (SelectedDirectory);
			text = info.Name;
		}
		style.font = GameFont;
		GUI.Label (GuiRect, text, style);

		if (RealTime) 
		{
		}
		else
		{
		}
	}
}

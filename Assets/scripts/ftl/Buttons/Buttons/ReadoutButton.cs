using UnityEngine;
using System.Collections;

public class ReadoutButton : ftlManager
{
	private GUIStyle readoutText;
	private bool activated = false;
	private DataDisplay thisDataDisplay;

	void OnGUI()
	{
		if (activated)
		{
			var centroid = transformToObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , thisDataDisplay.BoxButton);
			var GuiRect =  ( new Rect (centroid.x - 100, centroid.y - 50, 200 , 100));
			GUI.color = new Color (0f, 0f, 0.4f, 0.8f);
			GUI.Label (GuiRect, thisDataDisplay.ReadoutText , readoutText);
		}
	}

	public DataDisplay SetDataDisplay {
		get { return thisDataDisplay;}
		set 
		{
			thisDataDisplay = value;
			readoutText = new GUIStyle();
			readoutText.alignment = TextAnchor.UpperLeft;
			readoutText.contentOffset = new Vector2 (35,15);
			//CHANGED ABOVE X VALUE FROM 15 TO 35 TO CORRECT OFFSET -- 06/10/15
			readoutText.fontSize = 10;
			readoutText.normal.textColor = GetColor(thisDataDisplay.Point.Id, 1f, 1f);
			activated = true;
		}
	}

	public void DestroyReadout ()
	{
		GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().PlayOneShot (GetComponent<AudioSource> ().clip);
		Destroy (thisDataDisplay.VectorX);
		Destroy (thisDataDisplay.VectorY);
	}
}

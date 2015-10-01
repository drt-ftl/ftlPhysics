using UnityEngine;
using System.Collections;

public class GeneralButton : ftlManager 
{
	public string hotKey;
	public Rect GuiRect = new Rect (0,0,10,10);
	private Color tb;
	CheckButtons cb = new CheckButtons();

	void Awake()
	{
		var tmpCol = Color.black;
		tmpCol.a = GetComponent<SpriteRenderer>().color.a;
	}
	void OnMouseDown()
	{
		if (GetComponent<SpriteRenderer> ().color == ButtonGrey || GetComponent<SpriteRenderer> ().color == Invisible)
			return;
		foreach (var touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began && tag != "FolderButton")
				return;
		}
		cb.PressButton (gameObject);
	}
	
	void OnMouseUp()
	{
		if (GetComponent<SpriteRenderer> ().color == ButtonGrey || GetComponent<SpriteRenderer> ().color == Invisible)
			return;
		foreach (var touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Ended && tag != "FolderButton")
				return;
		}
		cb.ReleaseButton (gameObject);
	}

	private void OnGUI()
	{
		var tmpCol = Color.black;
		tmpCol.a = GetComponent<SpriteRenderer>().color.a;
		var style = buttonText;
		style.normal.textColor = tmpCol;
		if (GetComponent<SpriteRenderer> ().color == Invisible) 
		{
			GetComponent<Collider>().enabled = false;
		}
		else
		{
			if (GetComponent<SpriteRenderer> ().color == ButtonGrey)
				GetComponent<Collider>().enabled = false;
			else
				GetComponent<Collider>().enabled = true;
			if (tag == "FolderButton")
			{
				style.normal.textColor = new Color ( 0.35f, 0.239f, 0.024f, 1f);
				style.font = GameFont;
				var centroid = transformToObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , gameObject);
				GuiRect = new Rect (centroid.x - 100, centroid.y - 50, 200 , 100);
				var thisName = name.Split ('\\');
				var shortName = thisName[thisName.Length - 1];
				GUI.Label (GuiRect, shortName, style);
			}
		}
	}
	public void press ()
	{
		if (!Camera.main.GetComponent<AudioSource> ().isPlaying)
			Camera.main.GetComponent<AudioSource> ().PlayOneShot (Camera.main.GetComponent<AudioSource> ().clip);
		GetComponent<SpriteRenderer> ().color = ButtonPressed;
	}

	public void release ()
	{
		GetComponent<SpriteRenderer> ().color = ButtonNormal;
	}
}

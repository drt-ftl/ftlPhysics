using UnityEngine;
using System.Collections;

public class Overlay : MonoBehaviour 
{

	Rect ThisRect;
	public Texture Mask;

	void Start () 
	{
		var center = gameObject.transform.position;
		var extents = gameObject.GetComponent<Collider>().bounds.extents * 1.15f / 0.9f;

		var corner = new Vector3 (center.x - extents.x, center.y - extents.y, 0);
		corner = Camera.main.WorldToScreenPoint(corner);
		corner.y = Screen.height - corner.y;

		var otherCorner = new Vector3 (center.x + extents.x, center.y + extents.y, 0);
		otherCorner = Camera.main.WorldToScreenPoint(otherCorner);
		otherCorner.y = Screen.height - otherCorner.y;

		extents = otherCorner - corner;
		ThisRect = new Rect(corner.x, corner.y + extents.y, extents.x, -extents.y);
	}
	

	void OnGUI () 
	{
		GUI.depth = 1;
		GUI.DrawTexture(ThisRect, Mask);
	}
}

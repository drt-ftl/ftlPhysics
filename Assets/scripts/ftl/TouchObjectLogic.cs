using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TouchObjectLogic : ftlManager 
{
	private int id = -1;
	private int pointId = -1;
	public bool activated = false;
	public DataDisplay thisDataDisplay = new DataDisplay();
	private int graphPointId = -1;
	public AudioClip touchDownSound;
	public AudioClip lineMoveSound;
	
	void OnTriggerEnter (Collider hit)
	{
		if (RealTime)
			return;
		var splitName = hit.name.Split ('_');
		if (splitName[0] == "Line") // Hit a line
		{
			GetComponent<AudioSource>().PlayOneShot(lineMoveSound);
			if (id < 0)
				id = Convert.ToInt16(splitName[1]); // Assigns dataDisplay to a specific touchLine
			if (id == Convert.ToInt16(splitName[1])) // Makes sure we are on the correct touchLine
			{
				pointId = Convert.ToInt16(splitName[2]);
				var thisFtlTouch = ftlTouches[id][pointId];
				if (!activated)
				{
					thisDataDisplay.Vector = vector;
					thisDataDisplay.BoxButtonSet = readoutBox;
					activated = true;
				}
				thisDataDisplay.Point = thisFtlTouch;

				if (graphPointId == -1)
				{
					graphPointId = graphPoints.Count;
					thisDataDisplay.GraphPointId = graphPointId;
					graphPoints.Add (graphPointId, thisFtlTouch);
				}
				else
				{
					graphPoints[graphPointId] = thisFtlTouch;
				}
			}
		}
	}

	void Update()
	{
		if (activated)
		{
			var extents = thisDataDisplay.BoxButton.GetComponent<Collider>().bounds.extents;
			var position = gameObject.transform.position;
			position.x += 1.1f * extents.x;
			position.y += 1.1f * extents.y;
			position.z = -0.1f;
			thisDataDisplay.BoxButton.transform.position = position;
		}
	}
}

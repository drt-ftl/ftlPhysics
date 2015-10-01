
using UnityEngine;
using System.Collections;
using System;

public class TimeManager : ftlManager 
{

	void Update () 
	{
		if (RealTime)
			return;
		foreach (var touch in ftlTouches)
		{
			var id = touch.Key;
			var normalColor = GetColor (id , 0.9f , 0.7f);
			foreach (var point in touch.Value)
			{
				if (!GameObject.Find ("Line_" + point.Id.ToString() + "_" + point.PointId.ToString()))
				{
					continue;
				}
				var line = GameObject.Find ("Line_" + point.Id.ToString() + "_" + point.PointId.ToString());

				if (point.EventTime > MAX_TIME || point.EventTime < MIN_TIME || point.EventTime >= CURRENT_TIME)
				{
					line.GetComponent<LineRenderer>().SetColors(Invisible, Invisible);
					line.GetComponent<Collider>().enabled = false;
				}
				else
				{
					line.GetComponent<LineRenderer>().SetColors(normalColor, normalColor);
					line.GetComponent<Collider>().enabled = true;
				}
			}
		}
	}
}

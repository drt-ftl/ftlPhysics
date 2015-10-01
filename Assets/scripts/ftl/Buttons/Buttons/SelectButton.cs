using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SelectButton : ftlManager 
{

	public void press ()
	{
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		GetComponent<GeneralButton> ().release();

		var newDirInfo = new DirectoryInfo (SelectedDirectory);
		MIN_TIME_HARD = 100000;
		MAX_TIME_HARD = 0;

		foreach (var file in newDirInfo.GetFiles())
		{
			var PointId = 0;
			if (!file.FullName.EndsWith(".meta"))
			{
				var reader = new StreamReader (File.OpenRead(file.FullName));
				while(!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					if (!String.IsNullOrEmpty(line) && !line.Contains("X"))
					{
						var splitLine = line.Split(',');
						var isGood = true;
						foreach (var segment in splitLine)
						{
							float f;
							int i;
							if (!int.TryParse(segment, out i) && !float.TryParse(segment, out f))
							{
								isGood = false;
							}
						}
						if (isGood)
						{
							var newFtlTouch = new ftlTouch();
							newFtlTouch.Id = Convert.ToInt16(splitLine[0]);
							newFtlTouch.PointId = Convert.ToInt16(splitLine[1]);
							newFtlTouch.EventTime = (float)Convert.ToDecimal(splitLine[2]);

							var worldPosition = new Vector3();
							worldPosition.x = (float)Convert.ToDecimal(splitLine[3]);
							worldPosition.y = (float)Convert.ToDecimal(splitLine[4]);
							worldPosition.z = 0;
							newFtlTouch.WorldPosition = worldPosition;

							var screenPosition = new Vector2();
							screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
							newFtlTouch.ScreenPosition = screenPosition;

							var averageVelocity = new Vector2();
							averageVelocity.x = (float)Convert.ToDecimal(splitLine[5]);
							averageVelocity.y = (float)Convert.ToDecimal(splitLine[6]);
							newFtlTouch.AverageVelocity = averageVelocity;

							newFtlTouch.Speed = (float)Convert.ToDecimal(splitLine[7]);

							newFtlTouch.LineDown = false;
							newFtlTouch.Active = true;
							if (!ftlTouches.ContainsKey (newFtlTouch.Id))
								ftlTouches.Add (newFtlTouch.Id, new List<ftlTouch>());

							if (newFtlTouch.EventTime < MIN_TIME_HARD)
								MIN_TIME_HARD = newFtlTouch.EventTime;
							if (newFtlTouch.EventTime > MAX_TIME_HARD)
								MAX_TIME_HARD = newFtlTouch.EventTime;

							ftlTouches[newFtlTouch.Id].Add (newFtlTouch);
						}
					}
				}
			}
		}
		USER_MESSAGE = "Files Loaded";
		USER_WARNING = SelectedDirectory;
		Looping = false;
		if (GameObject.Find (SelectedDirectory))
		{
			GameObject.Find (SelectedDirectory).GetComponent<FolderButton>().ShouldNotShow();
		}
		GetComponent<ButtonSliders> ().SlideOut ();
		MAX_TIME = MAX_TIME_HARD;
		MIN_TIME = MIN_TIME_HARD;
		Camera.main.GetComponent<ftlDataGatherer> ().Pause (true);
		START_TIME = Time.realtimeSinceStartup;
		GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().PlayOneShot (GameObject.FindGameObjectWithTag("BG").GetComponent<AudioSource> ().clip);
		ShowSliders ();
		if (GameObject.Find ("Loop"))
		{
			GameObject.Find ("Loop").GetComponent<ButtonSliders>().SlideIn();
		}
		ChangeGraphTimeScale (false);
	}
}

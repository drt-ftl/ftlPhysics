using UnityEngine;
using System.Collections;
using System.IO;

public class SaveButton : ftlManager 
{
	public void press ()
	{
		GetComponent<GeneralButton> ().press ();
	}
	
	public void release ()
	{
		GetComponent<GeneralButton> ().release();
		var path = GetNewDirName ();
		foreach (var touch in ftlTouches)
		{
			var makeThisFile = false;
			var testTouch = touch.Value;
			foreach (var testPoint in testTouch)
			if (touch.Value.Count >= 1 && testPoint.EventTime >= MIN_TIME && testPoint.EventTime <= MAX_TIME)
			{
				makeThisFile = true;
			}
			if (makeThisFile)
			{
				var touchNumber = touch.Key;
				var touchNumberString = touchNumber.ToString();
				var tw = File.CreateText(path + "/touch." + touchNumberString + ".csv");	
				tw.WriteLine("Touch,PointInTouch,Time,X,Y,Velocity(x),Velocity(y),Speed");
				var thisPoint = 0;
				foreach (var point in testTouch)
				{
					if (point.EventTime >= MIN_TIME && point.EventTime <= MAX_TIME)
					{
						tw.WriteLine(point.Id.ToString() 	// Writes out data for touch
						             + "," + thisPoint.ToString ()
						             + "," + point.EventTime.ToString("f4")
						             + "," + point.WorldPosition.x.ToString("f4")
						             + "," + point.WorldPosition.y.ToString("f4")
						             + "," + point.AverageVelocity.x.ToString("f4")
						             + "," + point.AverageVelocity.y.ToString("f4")
						             + "," + point.Speed.ToString("f4"));
						thisPoint++;
				}
				}
				tw.Close();
			}
		}
		GameObject.Find ("Load").GetComponent<ButtonSliders> ().SlideIn ();
		savedDataAvailableToLoad = true;
	}
}

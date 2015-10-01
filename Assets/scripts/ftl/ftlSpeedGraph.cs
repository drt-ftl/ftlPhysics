using System;
using System.IO;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using TouchScript;
using UnityEngine;


public class ftlSpeedGraph : ftlManager
{
	public float AxisDivisions = 20;
	float axisSpacing;
	private float scrollingOffset = 0;
	Vector3 xyMaxVector = new Vector3 (Screen.width , 0 , 0);
	Vector3 xyMinVector = new Vector3 (0 , Screen.height , 0);
	int border = 5;
	private Color gridColor = new Color (0f, 0f, 0f, 0.1f);
	private Material graphMaterial;
	private float scrollingOffsetFactor = 0.9f;
	private float timeScale = 200f;
	public float Angle = 0f;
	public Material material;
	public enum modes {Speed, VelocityX, VelocityY, SpeedSquared, Displacement}
	public modes yAxis = modes.Speed;
	public Texture2D GraphPoint;
	private float ssScale = 0.1f;
	public float yScale = 20f;
	public float graphBottomOffset = 0;

	public float TimeScaleModifier 
	{
		set { timeScale = value;}
	}
	private void Start()
	{
		axisSpacing = (Screen.height) / AxisDivisions;
		xyMaxVector = new Vector3 (Screen.width , 0 , 0);
		scrollingOffset = xyMaxVector.x * scrollingOffsetFactor;
		graphMaterial = material;
	}

	private void OnGUI()
	{
		GUI.depth = 6;
		//graphMaterial.renderQueue = 300;
		if (gameObject.GetComponent<Collider>().bounds.center.z >= -10)
		{
			var boxVertex1 = transformToObject ( new Vector3( 0 , 0 , 0) , gameObject);
			var boxVertex2 = transformToObject ( new Vector3( Screen.width , Screen.height , 0) , gameObject);
			var centroid = transformToObject( new Vector3( Screen.width/2 , Screen.height/2 , 0) , gameObject);
			var GuiCentroid = GUIUtility.ScreenToGUIPoint (new Vector2 (centroid.x, centroid.y));
			var GuiRect = GUIUtility.ScreenToGUIRect ( new Rect (centroid.x - 100, centroid.y - 50 , 200 , 100));
			GUIUtility.RotateAroundPivot (Angle, GuiCentroid);

			#region Scrolling

			if ((RealTime && CURRENT_TIME * timeScale >= scrollingOffset)
			    || (!RealTime && CURRENT_TIME <= MAX_TIME))
			{
				GRAPH_TIME_ELAPSED = CURRENT_TIME * timeScale - scrollingOffset + panGraph * 8;
			}
			else if (RealTime)
				GRAPH_TIME_ELAPSED = 0;

			#endregion

			for (float divisionY = 0 ; divisionY <= Screen.height ; divisionY += axisSpacing)
			{
				var yAxisVertex1 = transformToObject ( new Vector3( 0 , Screen.height - divisionY , 0) , gameObject);
				var yAxisVertex2 = transformToObject ( new Vector3( Screen.width , Screen.height - divisionY , 0 ), gameObject);
				GL.Begin (GL.LINES);
				graphMaterial.SetPass (0);
				GL.Color (gridColor);
				GL.Vertex (new Vector3( yAxisVertex1.x , yAxisVertex1.y , 0));
				GL.Vertex (new Vector3( yAxisVertex2.x , yAxisVertex2.y , 0));
				GL.End ();
			}

			for (float divisionX = 0 ; divisionX <= Screen.width ; divisionX += axisSpacing)
			{
				var xAxisVertex1 = transformToObject ( new Vector3( divisionX , 0 , 0) , gameObject);
				var xAxisVertex2 = transformToObject ( new Vector3( divisionX , Screen.height , 0 ), gameObject);
				GL.Begin (GL.LINES);
				graphMaterial.SetPass (0);
				GL.Color (gridColor);
				GL.Vertex (new Vector3( xAxisVertex1.x , xAxisVertex1.y , 0));
				GL.Vertex (new Vector3( xAxisVertex2.x , xAxisVertex2.y , 0));
				GL.End ();
			}

			foreach (var touch in ftlTouches)
			{
				var touchCounter = touch.Key;
				var traceColor = GetColor (touchCounter , 0.5f , 0.7f);
				graphMaterial.SetPass (0);
				GL.Begin (GL.LINES);
				GL.Color (traceColor);
				foreach (var point in touch.Value)
				{
					var innerCounter = point.PointId;
					if (innerCounter >= 2 && touch.Value[innerCounter - 1].EventTime * timeScale - GRAPH_TIME_ELAPSED >= xyMinVector.x 
					    && point.EventTime * timeScale - GRAPH_TIME_ELAPSED <= scrollingOffset 
					    && point.EventTime <= CURRENT_TIME
					    && point.EventTime >= MIN_TIME
					    && point.EventTime <= MAX_TIME)
					{	
						var timer = point.EventTime * timeScale - GRAPH_TIME_ELAPSED;
						var yValue = 0f;
						var yValuePrevious = 0f;

						if (yAxis == modes.VelocityX)
							yValue = Screen.height/2 - point.AverageVelocity.x * yScale;
						else if (yAxis == modes.VelocityY)
							yValue = Screen.height/2 - point.AverageVelocity.y * yScale;
						else if (yAxis == modes.SpeedSquared)
							yValue = Screen.height - Mathf.Pow (point.Speed , 2) * yScale * ssScale + graphBottomOffset;
						else if (yAxis == modes.Displacement)
							yValue = Screen.height - Mathf.Sqrt (Mathf.Pow (point.ScreenPosition.x - touch.Value[0].ScreenPosition.x , 2)
							         + Mathf.Pow (point.ScreenPosition.y - touch.Value[0].ScreenPosition.y , 2))
									 * yScale / 0.5f;
						else
							yValue = Screen.height - point.Speed * yScale;

						if (yValue <= xyMaxVector.y)
						{
							var normMax = xyMaxVector.y - Screen.height / 2;
							var normVal = yValue - Screen.height / 2;
							if (yScale > Mathf.Abs(normMax / normVal) * 20f)
								yScale = Mathf.Abs(normMax / normVal) * 20f;
							yValue = xyMaxVector.y;
						}
						if (yValue >= xyMinVector.y)
						{
							var normMin = xyMaxVector.y - Screen.height / 2;
							var normVal = yValue - Screen.height / 2;
							if (yScale > Mathf.Abs(normMin / normVal) * 20f)
								yScale = Mathf.Abs(normMin / normVal) * 20f;
							yValue = xyMinVector.y;
						}

						var transformData = transformToObject ( new Vector3 ( timer , yValue , 0 ) , gameObject );			
						timer = transformData.x;
						yValue = transformData.y;

						var previousPoint = touch.Value[innerCounter - 1];
						var timerPrev = previousPoint.EventTime * timeScale - GRAPH_TIME_ELAPSED;

						if (yAxis == modes.VelocityX)
							yValuePrevious = Screen.height/2 - previousPoint.AverageVelocity.x * yScale;
						else if (yAxis == modes.VelocityY)
							yValuePrevious = Screen.height/2 - previousPoint.AverageVelocity.y * yScale;
						else if (yAxis == modes.SpeedSquared)
							yValuePrevious = Screen.height - Mathf.Pow (previousPoint.Speed , 2) * yScale * ssScale + graphBottomOffset;
						else if (yAxis == modes.Displacement)
							yValuePrevious = Screen.height - Mathf.Sqrt (Mathf.Pow (previousPoint.ScreenPosition.x - touch.Value[0].ScreenPosition.x , 2)
							         + Mathf.Pow (previousPoint.ScreenPosition.y - touch.Value[0].ScreenPosition.y , 2))
								     * yScale / 0.5f;
						else
							yValuePrevious = Screen.height - previousPoint.Speed * yScale;

						if (yValuePrevious <= xyMaxVector.y)
							yValuePrevious = xyMaxVector.y;
						if (yValuePrevious >= xyMinVector.y)
							yValuePrevious = xyMinVector.y;

						var transformDataPrevious = transformToObject (new Vector3 ( timerPrev , yValuePrevious , 0 ) , gameObject);
						timerPrev = transformDataPrevious.x;
						yValuePrevious = transformDataPrevious.y;
						var previousPoints = new Vector2(timerPrev , yValuePrevious);

						GL.Vertex (new Vector3( previousPoints.x , previousPoints.y , 0 ));
						GL.Vertex (new Vector3( timer , yValue , 0 ));
					}
				}
				GL.End ();
			}

			#region TouchVector Indicators
			foreach (var graphPoint in graphPoints)
			{
				var timer = graphPoint.Value.EventTime * timeScale - GRAPH_TIME_ELAPSED;
				if (timer >= xyMinVector.x && timer <= xyMaxVector.x)
				{
					var _color = GetColor(graphPoint.Value.Id , 1f , 1f);
					// Use Below for Line Indicator
					var transformMin = transformToObject ( new Vector3( timer , 0 , 0) , gameObject);
					var transformMax = transformToObject ( new Vector3( timer , Screen.height , 0) , gameObject);
					GL.Begin (GL.LINES);
					graphMaterial.SetPass (0);
					GL.Color (_color);
					GL.Vertex (new Vector3( transformMin.x , transformMin.y , 0));
					GL.Vertex (new Vector3( transformMax.x , transformMax.y , 0));
					GL.End ();
					
					// Use Below for Circle Indicator
					var yValue = 0f;
					var yValuePrevious = 0f;
					
					if (yAxis == modes.VelocityX)
						yValue = Screen.height/2 - graphPoint.Value.AverageVelocity.x * yScale;
					else if (yAxis == modes.VelocityY)
						yValue = Screen.height/2 - graphPoint.Value.AverageVelocity.y * yScale;
					else if (yAxis == modes.SpeedSquared)
						yValue = Screen.height - Mathf.Pow (graphPoint.Value.Speed , 2) * yScale * ssScale + graphBottomOffset;
					else if (yAxis == modes.Displacement && ftlTouches.ContainsKey(graphPoint.Key))
						yValue = Screen.height - Mathf.Sqrt (Mathf.Pow (graphPoint.Value.ScreenPosition.x - ftlTouches[graphPoint.Key][0].ScreenPosition.x , 2)
						                                     + Mathf.Pow (graphPoint.Value.ScreenPosition.y - ftlTouches[graphPoint.Key][0].ScreenPosition.y , 2))
							* yScale / 0.5f;
					else
						yValue = Screen.height - graphPoint.Value.Speed * yScale;
					
					if (yValue <= xyMaxVector.y)
						yValue = xyMaxVector.y;
					if (yValue >= xyMinVector.y)
						yValue = xyMinVector.y;
					
					var transformData = transformToObject ( new Vector3 ( timer , yValue , 0 ) , gameObject );			
					timer = transformData.x;
					yValue = transformData.y;
					
					var x = graphPoint.Value.ScreenPosition.x;
					var y = Screen.height - graphPoint.Value.ScreenPosition.y;
					GUI.color = _color;
					GUI.DrawTexture(new Rect(timer - GraphPoint.width/8, yValue - GraphPoint.height/8, GraphPoint.width / 4, GraphPoint.height / 4), GraphPoint, ScaleMode.ScaleToFit);
				}
			}
			#endregion

			#region Draw Outer Boxes

			#endregion
			
			#region Label and Centerline for Velocities
			GUIStyle centered = new GUIStyle();
			centered.alignment = TextAnchor.MiddleCenter;
			GUI.color = new Color (0f, 0f, 0.7f, 0.4f);
			
			if (yAxis == modes.VelocityX)
			{
				GL.Begin (GL.LINES);
				graphMaterial.SetPass (0);
				GL.Color (new Color (0.1f, 0.1f, 1f, 0.4f));			
				GL.Vertex (new Vector3( boxVertex1.x , centroid.y , 0));
				GL.Vertex (new Vector3( boxVertex2.x , centroid.y , 0));
				GL.End ();
				//GUI.Label (GuiRect, "VELOCITY X" , centered);
			}
			else if (yAxis == modes.VelocityY)
			{
				GL.Begin (GL.LINES);
				graphMaterial.SetPass (0);
				GL.Color (new Color (0.1f, 0.1f, 1f, 0.4f));			
				GL.Vertex (new Vector3( boxVertex1.x , centroid.y , 0));
				GL.Vertex (new Vector3( boxVertex2.x , centroid.y , 0));
				GL.End ();
				//GUI.Label (GuiRect, "VELOCITY Y" , centered);
			}
//			else if (yAxis == modes.SpeedSquared)
//			{
//				GUI.Label (GuiRect, "SPEED SQUARED" , centered);
//			}
//			else if (yAxis == modes.Displacement)
//			{
//				GUI.Label (GuiRect, "DISPLACEMENT" , centered);
//			}
//			else
//				GUI.Label (GuiRect, "SPEED" , centered);
			
			#endregion
		}
	}


}

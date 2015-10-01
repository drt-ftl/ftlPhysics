using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ftlLineRenderer : ftlManager {
	
	public LineRenderer Line;
	public AudioSource scribbler;
	public Material BaseMaterial;
	private float SpeedMultiplier = 0.01f;
	private float lineObjectZ;
	private bool done = false;

	void OnEnable ()
	{
		scribbler.volume = 0f;
	}
	void Update () 
	{
		foreach (var touch in ftlTouches)
		{
			var Id = touch.Key;
			var lineColor = GetColor (Id , 0.9f , 0.7f);
			
			foreach (var point in touch.Value)
			{
				var PointNumber = touch.Value.IndexOf (point);
				if (PointNumber > 1)
				{
					if (point.LineDown)
						done = true;
					else done = false;
					var position = point.WorldPosition;
					var previousPosition = touch.Value[PointNumber - 1].WorldPosition;
					if (!point.LineDown && CURRENT_TIME >= point.EventTime && point.AverageVelocity != null && point.Active)
					{
						if (RealTime)
						{
							var vol = 0.3f * point.Speed / 30f;
							if (vol > 0.3f)
								vol = 0.5f;
							scribbler.volume = vol;
						}
						var line = Instantiate (Line) as LineRenderer;
						line.SetVertexCount (2);
						line.SetPosition (0 , previousPosition);
						line.SetPosition (1 , position);
						line.material = BaseMaterial;
						line.name = "Line_" + Id.ToString() + "_" + PointNumber.ToString();
						if (point.Discard)
							line.SetColors (new Color (0f,1f,0f,1f), new Color (1f,1f,1f,1f));
						else
							line.SetColors ( lineColor , lineColor );
						line.SetWidth (touch.Value[PointNumber - 1].Speed * SpeedMultiplier , point.Speed * SpeedMultiplier );
						line.gameObject.transform.position = position;
						var colliderDiameter = Mathf.Sqrt ( Mathf.Pow (position.x - previousPosition.x , 2) + Mathf.Pow (position.y - previousPosition.y , 2));
						line.gameObject.GetComponent<Collider>().transform.localScale = new Vector3 (colliderDiameter , colliderDiameter , colliderDiameter);
						point.LineDown = true;
						done = true;
					}
				}
			}
		}
		if (done || CURRENT_TIME >= MAX_TIME)
			scribbler.volume = 0;
	}
}

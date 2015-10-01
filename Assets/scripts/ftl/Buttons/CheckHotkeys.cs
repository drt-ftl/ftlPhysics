using UnityEngine;
using System.Collections;

public class CheckHotkeys 
{
	private Color _invisible = new Color (0f, 0f, 0f, 0f);
	private Color _buttonGrey = new Color (1f, 1f, 1f, 0.5f);

	public void PressButton (string keyName)
	{
		var hit = GameObject.Find (keyName);
		if (hit.GetComponent<SpriteRenderer>().color == _invisible || hit.GetComponent<SpriteRenderer>().color == _buttonGrey)
			return;

		switch (keyName) 
		{
			case "Analysis":
			{
				hit.GetComponent<AnalysisButton>().press ();
				hit.GetComponent<AnalysisButton>().release ();
				break;
			}
			case "Experiment":
			{
				hit.GetComponent<RealTimeButton>().press ();
				hit.GetComponent<RealTimeButton>().release ();
				break;
			}
			case "Save":
			{
				hit.GetComponent<SaveButton>().press ();
				hit.GetComponent<SaveButton>().release ();
				break;
			}
			case "Clear":
			{
				hit.GetComponent<ClearButton> ().press ();
				hit.GetComponent<ClearButton> ().release ();
				break;
			}
			case "Load":
			{
				hit.GetComponent<LoadButton> ().press ();
				hit.GetComponent<LoadButton> ().release ();
				break;
			}
			case "Back":
			{
				hit.GetComponent<BackButton> ().press ();
				hit.GetComponent<BackButton> ().release ();
				break;
			}
			case "Select":
			{
				hit.GetComponent<SelectButton> ().press ();
				hit.GetComponent<SelectButton> ().release ();
				break;
			}
			case "Loop":
			{
				hit.GetComponent<LoopButton> ().press ();
				hit.GetComponent<LoopButton> ().release ();
				break;
			}
			default:
			{
				break;
			}
		}
	}
}

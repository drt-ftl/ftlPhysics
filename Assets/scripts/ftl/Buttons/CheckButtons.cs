using UnityEngine;
using System.Collections;

public class CheckButtons 
{
	public void PressButton (GameObject hit)
	{

		switch (hit.name) 
		{
			case "Analysis":
			{
				if (Camera.main.GetComponent<ftlManager>().IsRealTime())
					hit.GetComponent<AnalysisButton>().press ();
				break;
			}
			case "Experiment":
			{
				if (!Camera.main.GetComponent<ftlManager>().IsRealTime())
					hit.GetComponent<RealTimeButton>().press ();
				break;
			}
			case "Save":
			{
				hit.GetComponent<SaveButton>().press ();
				break;
			}
			case "Clear":
			{
				hit.GetComponent<ClearButton> ().press ();
				break;
			}
			case "Load":
				{
					hit.GetComponent<LoadButton> ().press ();
					break;
				}
			case "Back":
				{
					hit.GetComponent<BackButton> ().press ();
					break;
				}
			case "Select":
				{
					hit.GetComponent<SelectButton> ().press ();
					break;
				}
			case "Loop":
			{
				hit.GetComponent<LoopButton> ().press ();
				break;
			}
			default:
			{
				break;
			}
		}
		if (hit.tag == "FolderButton")
		{
			hit.GetComponent<FolderButton>().press();
		}
	}

	public void ReleaseButton (GameObject hit)
	{
		switch (hit.name)
		{
		case "Analysis":
		{
			hit.GetComponent<AnalysisButton>().release ();
			break;
		}
		case "Experiment":
		{
			hit.GetComponent<RealTimeButton>().release ();
			break;
		}
		case "Save":
		{
			hit.GetComponent<SaveButton>().release ();
			break;
		}
		case "Clear":
		{
			hit.GetComponent<ClearButton> ().release ();
			break;
		}
		case "Load":
		{
			hit.GetComponent<LoadButton>().release ();
			break;
		}

		case "Back":
		{
			hit.GetComponent<BackButton>().release ();
			break;
		}
		case "Select":
		{
			hit.GetComponent<SelectButton>().release ();
			break;
		}
		default:
		{
			break;
		}
		}
		if (hit.tag == "FolderButton")
		{
			hit.GetComponent<FolderButton>().release();
		}
		if (hit.tag == "ReadoutButton")
		{
			hit.GetComponent<ReadoutButton>().DestroyReadout();
			MonoBehaviour.Destroy (hit);
		}
	}

}

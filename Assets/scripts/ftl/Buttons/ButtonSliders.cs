using UnityEngine;
using System.Collections;
using System.Threading;

public class ButtonSliders : MonoBehaviour 
{

	private Vector3 showPosition = new Vector3 ();
	private Vector3 hidePosition;
	public float slideAmount = 0f;
	public float slideTime = 0.5f;
	private Vector3 currentPosition = new Vector3();
	private bool moving = false;
	public enum dir {Vertical, Horizontal};
	public dir direction;
	public AudioSource servoSound;
	public enum SlideSelect {OnSlideIn, OnSlideOut,Both}
	public SlideSelect SoundEvent;

	void Awake()
	{
		showPosition = transform.position;
		hidePosition = showPosition;
		if (direction == dir.Horizontal)
			hidePosition.x += slideAmount;
		else
			hidePosition.y += slideAmount;
		currentPosition = showPosition;
	}
	
	public void SlideOut ()
	{
		if (IsShowing() && servoSound != null && !servoSound.isPlaying && (SoundEvent == SlideSelect.OnSlideOut || SoundEvent == SlideSelect.Both))
		{
			var clip = servoSound.clip;
			servoSound.PlayOneShot(clip);
			print ("Out: " + name);
		}
		StartCoroutine (Slide(showPosition, hidePosition, slideTime));
	}
	public void SlideIn ()
	{
		if (!IsShowing() && servoSound != null && ! servoSound.isPlaying && (SoundEvent == SlideSelect.OnSlideIn || SoundEvent == SlideSelect.Both))
		{
			var clip = servoSound.clip;
			servoSound.PlayOneShot(clip);
			print ("In: " + name);
		}
		StartCoroutine (Slide(hidePosition, showPosition, slideTime));
	}
	
	public void Hide()
	{
		transform.position = hidePosition;
	}
	public void Show()
	{
		transform.position = showPosition;
	}

	public bool IsShowing ()
	{
		if (Vector3.Distance (showPosition, transform.position) < 0.5f)
			return true;
		else
			return false;
	}

	IEnumerator Slide(Vector3 start, Vector3 end, float time)
	{
		if (!moving && Vector3.Distance(start, transform.position) < 0.5f) 
		{
			moving = true;
			float t = 0f;
			while (t < 1f) 
			{
				t += Time.deltaTime / time; // sweeps from 0 to 1 in time seconds
				transform.position = Vector3.Lerp(start, end, t); // set position proportional to t
				yield return 0; // leave the routine and return here in the next frame
			}
			moving = false; // finished moving		
		}
	}
}

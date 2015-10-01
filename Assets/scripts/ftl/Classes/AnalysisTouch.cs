using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;


public class AnalysisTouch
{

	private Dictionary<int, AnalysisTouch> _dictionary;
	private GameObject _touchObject;
	private ITouch _iTouch;

	public Dictionary<int, AnalysisTouch> SetDictionary
	{
		set{_dictionary = value;}
	}

	public GameObject TouchObject
	{
		get { return _touchObject;}
		set {_touchObject = value;}
	}

	public ITouch iTouch
	{
		get { return _iTouch;}
		set {_iTouch = value;}
	}

	public void Add()
	{
		if (!_dictionary.ContainsKey(_iTouch.Id))
		{
			_dictionary.Add (_iTouch.Id, this);
		}
	}
}

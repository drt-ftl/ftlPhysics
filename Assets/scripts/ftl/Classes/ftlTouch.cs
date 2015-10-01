using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;


public class ftlTouch 
{
	private int _id;
	private int _pointId;
	private float _time;
	private Dictionary<int, List<ftlTouch>> _dictionary;
	private int _numberOfSamplePoints = 3;
	private Vector2 _screenPosition;
	private Vector3 _worldPosition;
	private Vector2 _rawVelocity;
	private Vector2 _averageVelocity;
	private float _speed;
	private bool _active;
	private bool _lineDown;
	private ITouch _iTouch;
	private bool _discard = false;
	private GameObject camObject;


	public GameObject CamObject
	{
		get{return camObject;}
		set{camObject = value;}
	}

	public ITouch iTouch
	{
		get{return _iTouch;}
		set{_iTouch = value;}
	}

	public int Id
	{ 
		get { return _id;}
		set { _id = value;}
	}

	public int PointId 
	{ 	
		get {return _pointId;}
		set { _pointId = value;}
	}

	public Dictionary<int, List<ftlTouch>> SetDictionary
	{
		set{_dictionary = value;}
	}

	public void Add(bool HasTV)
	{
		_id = _iTouch.Id;
		if (!_dictionary.ContainsKey(Id))
		{
			_dictionary.Add (Id, new List<ftlTouch>());
		}
		_pointId = _dictionary [_id].Count;

		_screenPosition = _iTouch.Position;
		var window = CamObject.GetComponent<ftlManager> ().GetWindow ();
		if (!HasTV)
			_worldPosition = CamObject.GetComponent<ftlManager> ().transformToWindow (_screenPosition, window);
		else 
			_worldPosition = CamObject.GetComponent<Camera>().ScreenToWorldPoint (_screenPosition);
		_worldPosition.z = 0;
		if (_dictionary[_id].Count > 1)
		{
			var lastPoint = _dictionary[_id][_pointId - 1];
			var deltaTime = _time - lastPoint.EventTime;
			var deltaPosition = _worldPosition - lastPoint.WorldPosition;
			_rawVelocity = deltaPosition / deltaTime;

			var samplesToTake = 0;
			if (_pointId >= _numberOfSamplePoints)
			{
				samplesToTake = _numberOfSamplePoints;
			}
			else
			{
				samplesToTake = _pointId;
			}
			var runningTotal = Vector2.zero;
			for (int i = _pointId - samplesToTake; i < _pointId; i++)
			{
				runningTotal += _dictionary[_id][i].RawVelocity;
			}
			runningTotal += _rawVelocity;
			_averageVelocity = runningTotal / samplesToTake;
			var velSq = Mathf.Pow (_averageVelocity.x, 2) + Mathf.Pow(_averageVelocity.y, 2);
			_speed = Mathf.Sqrt(velSq);
			_active = true;
			_lineDown = false;
		}
		_dictionary [_id].Add (this);
	}

	public int SamplePoints
	{
		set { _numberOfSamplePoints = value;}
	}

	public float EventTime 
	{
		get{return _time;}
		set{_time = value;}
	}

	public Vector2 ScreenPosition
	{
		get { return _screenPosition;}
		set { _screenPosition = value;}
	}

	public Vector3 WorldPosition
	{
		get{return _worldPosition;}
		set { _worldPosition = value;}
	}

	public Vector2 RawVelocity
	{
		get{return _rawVelocity;}
	}

	public Vector2 AverageVelocity
	{
		get { return _averageVelocity;}
		set { _averageVelocity = value;}
	}

	public float Speed
	{
		get { return _speed;}
		set { _speed = value;}
	}

	public bool Active
	{
		get{return _active;}
		set{ _active = value;}
	}

	public bool LineDown
	{
		get{return _lineDown;}
		set{ _lineDown = value;}
	}

	public bool Discard
	{
		get{return _discard;}
		set{ _discard = value;}
	}
	
}

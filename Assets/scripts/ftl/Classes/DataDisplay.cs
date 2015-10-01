using System.Collections.Generic;
using UnityEngine;
using TouchScript;
using System;
using System.Collections;

public class DataDisplay
{
	ftlTouch _point;
	GameObject _vectorX;
	GameObject _vectorY;
	GameObject _vector;
	string _readout;
	static float _vectorScale = 0.008f;
	GameObject _boxButtonSet;
	GameObject _boxButton;

	public int GraphPointId { get; set; }
	public GameObject BoxButtonSet
	{
		set { _boxButtonSet = value;}
	}

	public GameObject BoxButton
	{
		get { return _boxButton;}
		set { _boxButton = value;}
		
	}

	public GameObject Vector
	{
		set { _vector = value;}
	}
	
	public GameObject VectorX
	{
		get { return _vectorX;}
		set { _vectorX = value;}

	}

	public GameObject VectorY
	{
		get { return _vectorY;}
		set { _vectorY = value;}
	}
	
	public ftlTouch Point
	{
		get {return _point;}
		set 
		{
			_point = value;
			var position = _point.WorldPosition;
			position.z = -0.1f;

			var xScale = new Vector3 (_point.AverageVelocity.x * _vectorScale , _point.AverageVelocity.x * _vectorScale , 1f);
			if (_vectorX == null)
				_vectorX = MonoBehaviour.Instantiate (_vector, position, Quaternion.identity) as GameObject;
			_vectorX.GetComponent<Renderer>().material.color = new Color (0.4f, 0.4f, 1.0f, 1.0f);
			_vectorX.transform.eulerAngles = new Vector3 (0f,0f,270f);
			_vectorX.transform.localScale = xScale;
			var xExtent = _vectorX.GetComponent<SpriteRenderer>().bounds.extents.x;
			if (xScale.y < 0)
				xExtent = -xExtent;
			_vectorX.transform.position = new Vector3 (position.x + xExtent, position.y, -0.1f);

			var yScale = new Vector3 (_point.AverageVelocity.y * _vectorScale, _point.AverageVelocity.y * _vectorScale, 1f);
			if (_vectorY == null)
				_vectorY = MonoBehaviour.Instantiate (_vector, position, Quaternion.identity) as GameObject;
			_vectorY.GetComponent<Renderer>().material.color = new Color (1.0f, 0.4f, 0.4f, 1.0f);
			_vectorY.transform.eulerAngles = new Vector3 (0f,0f,0f);
			_vectorY.transform.localScale = yScale;
			var yExtent = _vectorY.GetComponent<SpriteRenderer>().bounds.extents.y;
			if (yScale.y < 0)
				yExtent = -yExtent;
			_vectorY.transform.position = new Vector3 (position.x, position.y + yExtent, -0.1f);
			if (_boxButton == null)
				_boxButton = MonoBehaviour.Instantiate (_boxButtonSet, position, Quaternion.identity) as GameObject;
			_boxButton.tag = "ReadoutButton";
			_boxButton.GetComponent<Renderer>().material.color = Color.white;
			var boxScale = 0.1f;
			_boxButton.transform.localScale = new Vector3 (boxScale, boxScale, 1f);
			_boxButton.GetComponent<ReadoutButton>().SetDataDisplay = this;
		}
	}

	public string ReadoutText
	{
		get
		{
			var _worldPosition2 = new Vector2 (_point.WorldPosition.x, _point.WorldPosition.y);
			var _velocity2 = new Vector2 (_point.AverageVelocity.x, _point.AverageVelocity.y);
			
			var _id = "(Touch,Point): (" + _point.Id.ToString () + "," + _point.PointId.ToString() + ")" + "\r\n";
			var _eventTime = "Time: " + _point.EventTime.ToString ("f3") + "\r\n";
			var _position = "Position: " + _worldPosition2.ToString ("f3") + "\r\n";
			var _velocity = "Velocity: " + _point.AverageVelocity.ToString ("f3") + "\r\n";
			var _speed = "Speed: " + _point.Speed.ToString ("f3") + "\r\n";
			var _speedSquared = "Speed Squared: " + (Mathf.Pow(_point.Speed , 2)).ToString ("f3");
			var _readout = _id + _eventTime + _position + _velocity + _speed + _speedSquared;
			return _readout;}
		
		set { _point = Point;}
	}
}

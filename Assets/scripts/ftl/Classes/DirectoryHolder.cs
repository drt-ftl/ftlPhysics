using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DirectoryHolder
{
	private List <DirectoryHolder> _listOfDirectoryHolders = new List<DirectoryHolder> ();
	private List <FileInfo> _listOfFiles = new List<FileInfo> ();
	private string _parent;
	private GameObject _button;
	private string _fullName;

	public List<DirectoryHolder> ListOfDirectoryHolders
	{
		get { return _listOfDirectoryHolders;}
		set { _listOfDirectoryHolders = value;}
	}

	public string DirParent 
	{
		get { return _parent;}
		set { _parent = value;}
	}

	public List<FileInfo> ListOfFiles
	{
		get { return _listOfFiles;}
		set { _listOfFiles = value;}
	}

	public GameObject Button
	{
		get { return _button;}
		set { _button = value;}
	}

	public string FullName
	{
		get { return _fullName;}
		set { _fullName = value;}
	}

}

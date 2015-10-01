using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class DirectoryManager
{
	private Color _invisible = new Color (0f, 0f, 0f, 0f);
	private Color _buttonColor = new Color (1f, 1f, 1f, 1f);
	private Vector3 pos_Base = new Vector3 (-3.59f, 2.88f, 0f);
	private float xStep = 2.3f;
	private float yStep = 0.4f;
	private float yMin = -2.0f;
	private Dictionary <string, DirectoryHolder> completeDirectoryList = new Dictionary <string, DirectoryHolder>();
	
	#region new Directory
	public string GetNewDirectoryName ()
	{
		string year = DateTime.Today.Year.ToString ();
		string month = DateTime.Today.Month.ToString ();
		string day = DateTime.Today.Day.ToString ();
		string hour = DateTime.Now.Hour.ToString ();
		string minute = DateTime.Now.Minute.ToString ();
		string seconds = DateTime.Now.Second.ToString ();
		bool pm = false;
		
		if (DateTime.Now.Hour > 12)
		{
			hour = (DateTime.Now.Hour - 12).ToString ();
			if (DateTime.Now.Hour - 12 < 10)
			{
				hour = "0" + hour;
			}
			pm = true;
		}
		else
		{
			if (DateTime.Now.Hour < 10)
			{
				hour = "0" + hour;
			}
			pm = false;
		}
		
		if (DateTime.Today.Month < 10)
		{
			month = "0" + month;
		}
		
		if (DateTime.Today.Day < 10)
		{
			day = "0" + day;
		}
		
		if (DateTime.Now.Minute < 10)
		{
			minute = "0" + minute;
		}
		
		if (DateTime.Now.Second < 10)
		{
			seconds = "0" + seconds;
		}
		
		month = month + " - " + DateTime.Today.ToString ("MMMM");
		
		day = day + " - " + DateTime.Today.ToString ("dddd");
		
		string appendToFileName = "";
		
		if (!pm)
			appendToFileName = appendToFileName + "AM_";
		else
			appendToFileName = appendToFileName + "PM_";
		
		appendToFileName = appendToFileName + hour + "_" + minute + "_" + seconds;
		
		string basePath = Application.dataPath + "/logs";
		if (!System.IO.Directory.Exists (basePath))								// Looks for logs folder
			System.IO.Directory.CreateDirectory (basePath);						// Creates one if none is found
		
		string yearFolderPath = basePath + "/" + year;
		if (!System.IO.Directory.Exists (yearFolderPath))								// Looks for logs folder
			System.IO.Directory.CreateDirectory (yearFolderPath);
		
		string monthFolderPath = yearFolderPath + "/" + month;
		if (!System.IO.Directory.Exists (monthFolderPath))								// Looks for logs folder
			System.IO.Directory.CreateDirectory (monthFolderPath);
		
		string dayFolderPath = monthFolderPath + "/" + day;
		if (!System.IO.Directory.Exists (dayFolderPath))								// Looks for logs folder
			System.IO.Directory.CreateDirectory (dayFolderPath);
		
		string folderPath = dayFolderPath + "/" + appendToFileName;
		System.IO.Directory.CreateDirectory(folderPath);
		return folderPath;
	}
	#endregion

	#region Get Directories

	public Dictionary <string, DirectoryHolder> SetDictionary
	{
		set { completeDirectoryList = value;}
	}
	public Dictionary <string, DirectoryHolder> GetAllDirectories (GameObject _folderButton)
	{
		DirectoryInfo logsDirectory = new DirectoryInfo (Application.dataPath + "/logs");
		var logsDirectoryName = Application.dataPath + "/logs";

		if (logsDirectory.GetDirectories() != null)
		{
			var posYear = pos_Base;
			var posMonth = pos_Base;
			var posDay = pos_Base;
			var posTime = pos_Base;
			var fileSlot = MonoBehaviour.Instantiate(_folderButton, pos_Base, Quaternion.identity) as GameObject;
			fileSlot.name = "FileSlot";
			fileSlot.GetComponent<SpriteRenderer>().color = _invisible;
			foreach (var yearDirectory in logsDirectory.GetDirectories ())
			{
				var yearFolderHolder = new DirectoryHolder();
				yearFolderHolder.DirParent = logsDirectoryName;
				yearFolderHolder.Button = MonoBehaviour.Instantiate(_folderButton, posYear, Quaternion.identity) as GameObject;
				yearFolderHolder.Button.name = yearDirectory.FullName;

				foreach (var monthDirectory in yearDirectory.GetDirectories())
				{
					var monthFolderHolder = new DirectoryHolder();
					monthFolderHolder.DirParent = yearDirectory.FullName;
					monthFolderHolder.Button = MonoBehaviour.Instantiate(_folderButton, posMonth, Quaternion.identity) as GameObject;
					monthFolderHolder.Button.name = monthDirectory.FullName;
					monthFolderHolder.Button.GetComponent<SpriteRenderer>().color = _invisible;

					foreach (var dayDirectory in monthDirectory.GetDirectories())
					{
						var dayFolderHolder = new DirectoryHolder();
						dayFolderHolder.DirParent = monthDirectory.FullName;
						dayFolderHolder.Button = MonoBehaviour.Instantiate(_folderButton, posDay, Quaternion.identity) as GameObject;
						dayFolderHolder.Button.name = dayDirectory.FullName;
						dayFolderHolder.Button.GetComponent<SpriteRenderer>().color = _invisible;

						foreach (var timeDirectory in dayDirectory.GetDirectories())
						{
							var timeFolderHolder = new DirectoryHolder();
							timeFolderHolder.DirParent = dayDirectory.FullName;
							timeFolderHolder.Button = MonoBehaviour.Instantiate(_folderButton, posTime, Quaternion.identity) as GameObject;
							timeFolderHolder.Button.name = timeDirectory.FullName;
							timeFolderHolder.Button.GetComponent<SpriteRenderer>().color = _invisible;

							foreach (var fileInfo in timeDirectory.GetFiles())
							{
								if (!fileInfo.Name.EndsWith(".meta"))
									timeFolderHolder.ListOfFiles.Add (fileInfo);
							}
							dayFolderHolder.ListOfDirectoryHolders.Add (timeFolderHolder);
							completeDirectoryList.Add (timeDirectory.FullName, timeFolderHolder);
							if (posTime.y >= yMin)
							{
								posTime.y -= yStep;
							}
							else
							{
								posTime.y = pos_Base.y;
								posTime.x += xStep;
							}
						}
						posTime.x = pos_Base.x;
						posTime.y = pos_Base.y;
						monthFolderHolder.ListOfDirectoryHolders.Add (dayFolderHolder);
						completeDirectoryList.Add (dayDirectory.FullName, dayFolderHolder);
						if (posDay.y >= yMin)
						{
							posDay.y -= yStep;
						}
						else
						{
							posDay.y = pos_Base.y;
							posDay.x += xStep;
						}
					}
					posDay.x = pos_Base.x;
					posDay.y = pos_Base.y;
					yearFolderHolder.ListOfDirectoryHolders.Add (monthFolderHolder);
					completeDirectoryList.Add (monthDirectory.FullName, monthFolderHolder);
					if (posMonth.y >= yMin)
					{
						posMonth.y -= yStep;
					}
					else
					{
						posMonth.y = pos_Base.y;
						posMonth.x += xStep;
					}
				}
				posMonth.x = pos_Base.x;
				posMonth.y = pos_Base.y;
				completeDirectoryList.Add (yearDirectory.FullName, yearFolderHolder);
				if (posYear.y >= yMin)
				{
					posYear.y -= yStep;
				}
				else
				{
					posYear.y = pos_Base.y;
					posYear.x += xStep;
				}
			}
			return completeDirectoryList;
		}
		else return null;
	}
	#endregion
}

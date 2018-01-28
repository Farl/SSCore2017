using UnityEngine;
using System.Collections;
using SS;

[System.Serializable]
public class DebugEventInfo
{
	public string eventID;
	public bool paramBool;
	public DebugEventInfo(string _eventID, bool _paramBool)
	{
		eventID = _eventID;
		paramBool = _paramBool;
	}
}

public class DebugMenu : MonoBehaviour
{
	bool debugMenuShow = false;

	bool waitZeroInput = false;
	
	[Auto]
	public DebugEventInfo[] debugEvent;

	void Start()
	{
	}

	void Update()
	{
		// Update Input
		if (waitZeroInput)
		{
			if (Input.touchCount == 0)
				waitZeroInput = false;
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Backspace) || Input.touchCount == 3)
			{
				debugMenuShow = !debugMenuShow;
				waitZeroInput = true;
			}
		}
	}

	void OnGUI()
	{
		if (!debugMenuShow)
			return;
		int segW = 5;
		int segH = 5;

		float layoutW = Screen.width / (float)segW;
		float layoutH = Screen.height / (float)segH;

		GUILayoutOption[] layout = new GUILayoutOption[] {GUILayout.Width(layoutW), GUILayout.Height(layoutH)};

		int id = 0;

		foreach (DebugEventInfo info in debugEvent)
		{
			if (id % segW == 0)
			{
				GUILayout.BeginHorizontal(layout);
			}
			if (GUILayout.Button(info.eventID, layout))
			{
                EventMessage em = new EventMessage(info.eventID, this, info.paramBool);
				EventManager.Broadcast(em);
			}
			if (id % segW == segW - 1)
			{
				GUILayout.EndHorizontal();
			}
			id++;
		}
	}
}

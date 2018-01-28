using UnityEngine;
using System.Collections;

public class SSGUIHandler
{
	SSGUI.DrawGUIDelegate onDraw;
	public SSGUIHandler(SSGUI.DrawGUIDelegate _onDraw)
	{
		if (_onDraw != null)
		{
			onDraw = _onDraw;
			SSGUI.Register(onDraw);
		}
	}
	
	~SSGUIHandler()
	{
		if (onDraw != null)
		{
			SSGUI.Unregister(onDraw);
		}
	}
}

public class SSGUI : MonoBehaviour
{
	public delegate void DrawGUIDelegate();

	static DrawGUIDelegate onDrawGUI;
	
	public static void Register(DrawGUIDelegate func)
	{
		onDrawGUI = onDrawGUI + func;
	}

	public static void Unregister(DrawGUIDelegate func)
	{
		onDrawGUI = onDrawGUI - func;
	}

	static Vector2 scrollPos = Vector2.zero;

	void OnGUI()
	{
		/*
		int elementW = Screen.width / 6;
		int elementH = Screen.height / 6;
		GUILayoutOption[] layout = new GUILayoutOption[] {GUILayout.Width(elementW), GUILayout.Height(elementH)};
		*/
		
		GUI.skin.button.fontSize = Screen.height / 16;
		GUI.skin.toggle.fontSize = Screen.height / 16;
		GUI.skin.label.fontSize = Screen.height / 16;
		GUI.skin.textArea.fontSize = Screen.height / 16;
		GUI.skin.textField.fontSize = Screen.height / 16;
		
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		scrollPos = GUILayout.BeginScrollView(scrollPos);

		if (onDrawGUI != null)
		{
			onDrawGUI();
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}

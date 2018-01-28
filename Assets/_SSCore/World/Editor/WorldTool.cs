using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SS
{
	public class WorldTool
	{		
		[PostProcessScene]
		public static void PostProcessScene()
		{
			Debug.LogFormat("Scene {0}", EditorApplication.currentScene);
			Debug.Log("[SS] (PostProcessScene) World Init");
			World.Init();
		}
	}
}

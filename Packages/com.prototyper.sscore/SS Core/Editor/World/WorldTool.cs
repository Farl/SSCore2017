using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SS.Core
{
	public class WorldTool
	{
		[PostProcessScene]
		public static void PostProcessScene()
		{
            //World.Init(); // Move to RuntimeInitializeOnLoadMethod in WorldInstance
        }
    }
}

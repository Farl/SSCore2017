using UnityEngine;
using System.Collections;

namespace SS
{
    [AddComponentMenu("")]
	public class WorldInstance : StaticSingleton<WorldInstance>
	{
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            World.Init();
        }

        void Start()
		{
			World.Start();
		}
		
		void Update()
		{
			World.Update();
		}
		
		void LateUpdate()
		{
			World.LateUpdate();
		}

		// This should be the only OnGUI in the game
		void OnGUI()
		{
			World.OnGUI();
		}
	}
}

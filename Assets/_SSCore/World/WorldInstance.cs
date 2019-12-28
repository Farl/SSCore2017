using UnityEngine;
using System.Collections;

namespace SS
{
    [AddComponentMenu("")]
	public class WorldInstance : MonoBehaviour
	{
        public static WorldInstance Instance;

		void Awake()
		{
			if (Instance != null)
			{
				Destroy(this.gameObject);
			}
			else
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
		}

        private void OnDestroy()
        {
            Debug.Log("WorldInstance.OnDestroy");
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

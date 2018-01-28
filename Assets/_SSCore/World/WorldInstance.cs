using UnityEngine;
using System.Collections;

namespace SS
{
	public class WorldInstance : MonoBehaviour
	{
		void Awake()
		{
			if (World.instance != null)
			{
				Destroy(this.gameObject);
			}
			else
			{
				World.instance = this;
				DontDestroyOnLoad(gameObject);
			}
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

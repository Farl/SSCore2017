using UnityEngine;
using System.Collections;
using SS;

public class InitManager : MonoBehaviour
{
	public EventArray eventInitial;
	public EventArray eventShutdown;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		eventInitial.Broadcast(this);
	}

	void OnDestroy()
	{
		eventShutdown.Broadcast(this);
	}
}

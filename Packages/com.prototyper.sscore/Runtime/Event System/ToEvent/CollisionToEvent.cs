using UnityEngine;
using System.Collections;
using SS;

public enum ColliderEventType
{
	OnCollisionEnter,
	OnCollisionExit,
	OnCollisionStay,
	OnTriggerEnter,
	OnTriggerExit,
	OnTriggerStay,
	OnUIHover,
}

public class CollisionToEvent : SimpleEventSource
{
	
	public ColliderEventType m_type;
	
	void Broadcast<T>(ColliderEventType type, T param)
	{
		if (type == m_type)
		{
			DoEvent();
		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		Broadcast<Collision>(ColliderEventType.OnCollisionEnter, col);
	}
	
	void OnCollisionExit(Collision col)
	{
		Broadcast<Collision>(ColliderEventType.OnCollisionExit, col);
	}
	
	void OnCollisionStay(Collision col)
	{
		Broadcast<Collision>(ColliderEventType.OnCollisionStay, col);
	}
	
	void OnTriggerEnter(Collider col)
	{
		Broadcast<Collider>(ColliderEventType.OnTriggerEnter, col);
	}
	
	void OnTriggerExit(Collider col)
	{
		Broadcast<Collider>(ColliderEventType.OnTriggerExit, col);
	}
	
	void OnTriggerStay(Collider col)
	{
		Broadcast<Collider>(ColliderEventType.OnTriggerStay, col);
	}
	
	void OnUIHover(bool isHover)
	{
		Broadcast<bool>(ColliderEventType.OnUIHover, isHover);
	}
}

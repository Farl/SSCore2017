using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS
{
	// Event Delegate
	public delegate void EventDelegate(EventMessage em, ref object paramRef);
	
	// Event Info
	public class EventInfo
	{
		public string m_eventID;
		public EventDelegate m_delegate;
		
		public EventInfo(string eventID)
		{
			m_eventID = eventID;
		}
		public void AddDelegate(EventDelegate ed)
		{
			m_delegate += ed;
		}
		public void RemoveDelegate(EventDelegate ed)
		{
			m_delegate -= ed;
		}
	}

	//
	public class EventMessage
	{
		public string eventID;
		public UnityEngine.Object origSrc;
		public bool paramBool;
		public string paramString;
		public object[] paramExtra;
        public bool useTimeScale;
		public Vector2 delayTime;
		public GameObject targetObj;

		// Global event
		public EventMessage(
			string _eventID,
			UnityEngine.Object _origSrc = null,
			bool _paramBool = true,
			string _paramString = null,
			Vector2 _delayTime = default(Vector2),
            bool useTimeScale = true,
			object[] _paramExtra = null
			)
        {
            targetObj = null;
            eventID = _eventID;
			origSrc = _origSrc;
			paramBool = _paramBool;
			paramString = _paramString;
			delayTime = _delayTime;
            this.useTimeScale = useTimeScale;
			paramExtra = _paramExtra;
		}

		// Object event
		public EventMessage(
			GameObject _targetObj,
			string _eventID,
			UnityEngine.Object _origSrc = null,
			bool _paramBool = true,
			string _paramString = null,
			Vector2 _delayTime = default(Vector2),
            bool useTimeScale = true,
            object[] _paramExtra = null
			)
		{
			targetObj = _targetObj;
			eventID = _eventID;
			origSrc = _origSrc;
			paramBool = _paramBool;
			paramString = _paramString;
			delayTime = _delayTime;
            this.useTimeScale = useTimeScale;
            paramExtra = _paramExtra;
		}
	}

	// Event Manager
	public static class EventManager
	{
		// Event Table. event ID and event delegate mapping
		public static Hashtable eventTable = new Hashtable();

		//
		static EventManager()
		{
			AddEventListener("Debug", DebugFunc);
		}

		// Debug
		static void DebugFunc(EventMessage em, ref object paramRef)
		{
			Debug.LogFormat("{0}, {1}, {2}, {3}, {4}, {5}", em.eventID, em.origSrc, em.paramBool, em.paramString, paramRef, em.paramExtra);
			return;
		}
		
		static EventInfo GetEventInfo(string eventID)
		{
			if (!eventTable.ContainsKey(eventID))
			{
				eventTable[eventID] = new EventInfo(eventID);
			}
			return (EventInfo)eventTable[eventID];
		}

		// Add Event Listener
		public static void AddEventListener(string eventID, EventDelegate ed, GameObject go = null)
		{
			if (go != null)
			{
				ObjectEventManager objManager = go.GetComponent<ObjectEventManager>();
				if (objManager == null)
					objManager = go.AddComponent<ObjectEventManager>();
				if (objManager != null)
					objManager.AddEventListener(eventID, ed, go);
			}
			else
			{
				EventInfo ei = GetEventInfo (eventID);
				if (ei != null)
					ei.AddDelegate(ed);
			}
		}

		// Remove Event Listener
		public static void RemoveEventListener(string eventID, EventDelegate ed, GameObject go = null)
		{
			if (go != null)
			{
				ObjectEventManager objManager = go.GetComponent<ObjectEventManager>();
				if (objManager == null)
					objManager = go.AddComponent<ObjectEventManager>();
				if (objManager != null)
					objManager.RemoveEventListener(eventID, ed, go);
			}
			else
			{
				EventInfo ei = GetEventInfo (eventID);
				if (ei != null)
					ei.RemoveDelegate(ed);
			}
		}

		#region Broadcast Global Event
		// Send Global Event
		public static void Broadcast(EventMessage em, ref object paramRef)
        {
            // delay
            if (em.delayTime.x > 0 && em.delayTime.y > 0)
            {
                EventTimer.AddTimer(em, em.delayTime, em.useTimeScale);
            }
            else
            {
                // clear delay
                em.delayTime = Vector2.zero;

                EventInfo ei = GetEventInfo(em.eventID);
                if (ei != null && ei.m_delegate != null)
                {
                    ei.m_delegate(em, ref paramRef);
                }
            }
            return;
        }
        public static void Broadcast(EventMessage em)
        {
            object obj = null;
            Broadcast(em, ref obj);
        }
        #endregion

        #region Object Event

        // Send Object Event

        public static void SendObjectEvent(EventMessage em, ref object paramRef)
		{
            if (em.targetObj)
            {
                if (em.delayTime.x > 0 && em.delayTime.y > 0)
                {
                    EventTimer.AddTimer(em, em.delayTime, em.useTimeScale);
                }
                else
                {
                    // clear delay
                    em.delayTime = Vector2.zero;

                    // Send to object event manager
                    ObjectEventManager[] objManagers = em.targetObj.GetComponentsInChildren<ObjectEventManager>(true);
                    foreach (ObjectEventManager objManager in objManagers)
                    {
                        objManager.SendObjectEvent(em, ref paramRef);
                    }
                }
            }
			return;
        }
        public static void SendObjectEvent(EventMessage em)
        {
            object obj = null;
            SendObjectEvent(em, ref obj);
        }
        #endregion
    }
}
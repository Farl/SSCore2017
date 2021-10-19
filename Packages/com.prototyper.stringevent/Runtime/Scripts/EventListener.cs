using UnityEngine;
using System.Collections;
using System;

namespace SS
{
	public class EventListener : MonoBehaviour
	{
		[HideInInspector] public bool listenOnlyEnabled = true;
		[HideInInspector] public string m_eventID;
		[HideInInspector] public bool m_recvGlobal = true;
		[HideInInspector] public bool m_recvLocal = false;

		protected bool enableListen = true;
		private bool m_intialized;

		public void Init()
		{
			if (m_intialized)
				return;
			m_intialized = true;
			if (m_recvGlobal)
				EventManager.AddEventListener(m_eventID, OnEvent);
			if (m_recvLocal)
				EventManager.AddEventListener(m_eventID, OnEvent, gameObject);
		}

		public void Delete()
		{
			if (!m_intialized)
				return;
			if (m_recvGlobal)
				EventManager.RemoveEventListener(m_eventID, OnEvent);
			if (m_recvLocal)
				EventManager.RemoveEventListener(m_eventID, OnEvent, gameObject);
		}

		protected virtual void Awake () {
			Init ();
		}

		protected virtual void OnDestroy() {
			Delete ();
        }

        protected virtual void OnEnable()
        {
            enableListen = true;
        }

        protected virtual void OnDisable()
        {
            enableListen = false;
        }

        [ContextMenu("Receive True")]
		public void TestEventTrue()
        {
            object obj = null;
            EventMessage em = new EventMessage(m_eventID, null, true);
            OnReceive(em, ref obj);
        }

		[ContextMenu("Receive False")]
		public void TestEventFalse()
        {
            object obj = null;
            EventMessage em = new EventMessage(m_eventID, null, false);
            OnReceive(em, ref obj);
        }

		// Full one
		protected virtual void OnEvent(EventMessage em, ref object paramRef)
		{
            OnEvent(em.paramBool);
        }

        // Simple one
        protected virtual void OnEvent(bool paramBool)
        {

        }

        protected virtual void OnReceive(EventMessage em, ref object paramRef)
        {

            OnEvent(em, ref paramRef);
        }

		// Call event directly
		public void Simulate(bool paramBool = true)
		{
            object obj = null;
            EventMessage em = new EventMessage(m_eventID, null, paramBool);
            OnReceive(em, ref obj);
		}
	}
}
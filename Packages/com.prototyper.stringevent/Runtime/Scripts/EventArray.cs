using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS
{
	
	[System.Serializable]
	public class EventMsg
	{
		public string m_eventID;
		public List<GameObject> m_targetObj;
		public bool m_paramBool;
		public string m_paramString;
		public Vector2 m_delayTime;
		public bool m_useTimeScale;
	}
	
	[System.Serializable]
	public class EventArray
	{
		public List<EventMsg> eventArray;

        public static EventArray operator+ (EventArray left, EventArray right)
        {
            EventArray ea = new EventArray();
            ea.eventArray = new List<EventMsg>();
            ea.eventArray.AddRange(left.eventArray);
            ea.eventArray.AddRange(right.eventArray);
            return ea;
        }

		public void Broadcast(Object obj)
		{
			if (eventArray == null)
				return;

			foreach (EventMsg info in eventArray)
			{
				bool bSend = false;

                EventMessage em = new EventMessage(info.m_eventID, obj, info.m_paramBool, info.m_paramString, info.m_delayTime, info.m_useTimeScale, null);

				if (info.m_targetObj != null)
				{
					foreach (GameObject target in info.m_targetObj)
					{
						if (target != null)
						{
                            em.targetObj = target;

                            EventManager.SendObjectEvent(em);
							bSend = true;
						}
					}
				}

				if (!bSend)
				{
					EventManager.Broadcast(em);
				}
			}
		}
	}

}
using UnityEngine;
using System.Collections;

namespace SS
{	
	public class EventTimer : MonoBehaviour
	{
		[SerializeField]
		int timerCount = 0;
		
		class BroadcastInfo
		{
			public EventMessage em;
            public System.Action callback;
			public float delayTime;
			public bool useTimeScale;
		}
		
		public static EventTimer Get(GameObject go)
		{
			EventTimer et = go.GetComponent<EventTimer>();
			if (et == null)
				et = go.AddComponent<EventTimer>();
			return et;
		}
		
		void AddTimerInternal(BroadcastInfo bi)
		{
			timerCount++;
			StartCoroutine("Broadcast", bi);
		}
		
		IEnumerator Broadcast(BroadcastInfo bi)
		{
			if (bi.useTimeScale)
				yield return new WaitForSeconds(bi.delayTime);
			else
			{
				float pauseEndTime = Time.realtimeSinceStartup + bi.delayTime;
				while (Time.realtimeSinceStartup < pauseEndTime)
				{
					yield return 0;
				}
			}

			if (bi.em.targetObj)
			{
				// Object Event
				EventManager.SendObjectEvent(bi.em);
			}
			else
			{
				// Global Event
				EventManager.Broadcast(bi.em);
			}
			
			// remove timer
			timerCount--;
			if (timerCount <= 0)
				Destroy(this);
		}

		static GameObject timerObj = new GameObject("EventTimer");

		static GameObject GetGameObject(UnityEngine.Object obj)
		{
			GameObject go = timerObj;
			if (obj != null)
			{
				if (obj.GetType().IsSubclassOf(typeof(Component)))
				{
					Component comp = obj as Component;
					if (comp != null)
						go = comp.gameObject;
				}
				else if (obj.GetType() == typeof(GameObject) || obj.GetType ().IsSubclassOf(typeof(GameObject)))
				{
					GameObject gameObj = obj as GameObject;
					if (gameObj != null)
						go = gameObj;
				}
			}
			return go;
		}
		
		public static void AddTimer(EventMessage em, Vector2 delayTime, bool useTimeScale)
		{
			// Get GameObject from Object
			GameObject attachObj = GetGameObject(em.origSrc);
			
			BroadcastInfo bi = new BroadcastInfo();
			bi.em = em;
			
			// Calculate delay time
			bi.delayTime = Random.Range(Mathf.Min (delayTime[0], delayTime[1]), Mathf.Max (delayTime[0], delayTime[1]));
			bi.useTimeScale = useTimeScale;
			
			EventTimer et = EventTimer.Get(attachObj);
			if (et != null)
				et.AddTimerInternal(bi);
		}

        public static void AddTimer(float time, System.Action callback, object param, bool useTimeScale)
        {
            BroadcastInfo bi = new BroadcastInfo();
            bi.callback = callback;
            bi.delayTime = time;
            bi.useTimeScale = useTimeScale;
            EventTimer et = EventTimer.Get(GetGameObject(null));
            if (et != null)
            {
                et.AddTimerInternal(bi);
            }
        }
	}
	
}
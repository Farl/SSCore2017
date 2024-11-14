using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_VISUAL_SCRIPTING
using Unity.VisualScripting;
#endif
using UnityEngine.Events;

namespace SS
{
    [RequireComponent(typeof(ObjectID))]
    public class RemoteEventReciever : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public string eventName;
            public UnityEvent unityEvent;
        }
        [SerializeField] private bool forwardToVisualScripting = true;
        [SerializeField] private List<Data> eventData = new List<Data>();

#if USE_VISUAL_SCRIPTING
        [System.NonSerialized] public ScriptMachine scriptMachine;
        [System.NonSerialized] public StateMachine stateMachine;
#else
        [System.NonSerialized] public MonoBehaviour scriptMachine;
        [System.NonSerialized] public MonoBehaviour stateMachine;
#endif

        public void Awake()
        {

#if USE_VISUAL_SCRIPTING
            if (!scriptMachine)
            {
                scriptMachine = GetComponent<ScriptMachine>();
            }
            if (!stateMachine)
            {
                stateMachine = GetComponent<StateMachine>();
            }
#endif
        }

        public void OnRecieve(string eventName)
        {
#if USE_VISUAL_SCRIPTING
            if (forwardToVisualScripting)
            {
                if (scriptMachine)
                {
                    scriptMachine.TriggerUnityEvent(eventName);
                }
                if (stateMachine)
                {
                    stateMachine.TriggerUnityEvent(eventName);
                }
            }
#endif
            var data = eventData.Find(x => x.eventName == eventName);
            if (data != null && data.unityEvent != null)
            {
                data.unityEvent.Invoke();
            }
        }
    }
}

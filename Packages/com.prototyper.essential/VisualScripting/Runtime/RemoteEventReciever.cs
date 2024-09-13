using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
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
        [System.NonSerialized] public ScriptMachine scriptMachine;
        [System.NonSerialized] public StateMachine stateMachine;

        public void Awake()
        {
            if (!scriptMachine)
            {
                scriptMachine = GetComponent<ScriptMachine>();
            }
            if (!stateMachine)
            {
                stateMachine = GetComponent<StateMachine>();
            }
        }

        public void OnRecieve(string eventName)
        {
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
            var data = eventData.Find(x => x.eventName == eventName);
            if (data != null && data.unityEvent != null)
            {
                data.unityEvent.Invoke();
            }
        }
    }
}

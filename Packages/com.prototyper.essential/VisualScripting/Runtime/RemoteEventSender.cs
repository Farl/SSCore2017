using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    
    public class RemoteEventSender : MonoBehaviour
    {
        [System.Serializable]
        public class SenderData
        {
            public List<string> recieverIDs = new List<string>();
            public string eventName;
        }

        public List<SenderData> senders = new List<SenderData>();

        public void Send(int index)
        {
            if (index < senders.Count)
            {
                var sender = senders[index];
                foreach (var recieverID in sender.recieverIDs)
                {
                    var reciever = ObjectMap.GetComponentByName<RemoteEventReciever>(recieverID);
                    if (reciever)
                    {
                        reciever.OnRecieve(sender.eventName);
                    }
                }
            }
        }
    }
}

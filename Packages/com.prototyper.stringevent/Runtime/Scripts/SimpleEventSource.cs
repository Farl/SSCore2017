using UnityEngine;
using System.Collections;

namespace SS
{
	public class SimpleEventSource : MonoBehaviour
	{
		public EventArray eventArray;
		
		[ContextMenu("Do Event")]
		public void DoEvent ()
		{
			eventArray.Broadcast(this);
		}
	}
}


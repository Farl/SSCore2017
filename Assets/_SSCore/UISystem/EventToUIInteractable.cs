using UnityEngine;
using System.Collections;

namespace SS
{
	public class EventToUIInteractable : SwitchEventListener
	{
		public bool recursive;
		public string flag;
        /*
		protected override void OnEvent (string eventID, bool paramBool, string paramString, Object origSource, ref object paramRef, params object[] paramEX)
		{
			base.OnEvent (eventID, paramBool, paramString, origSource, ref paramRef, paramEX);

			if (recursive)
			{
				UILocker.LockRecursive(gameObject, flag, !paramBool);
			}
			else
			{
				UILocker.Lock(gameObject, flag, !paramBool);
			}
		}
        */
	}

}
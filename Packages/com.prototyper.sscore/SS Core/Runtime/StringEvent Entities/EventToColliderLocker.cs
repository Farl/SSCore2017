using UnityEngine;
using System.Collections;
using SS;

namespace SS.Core
{
    public class EventToColliderLocker : EventListener
    {
        public GameObject targetObj;
        public string flag = "default";
        public bool unlock;
        public bool bRecursive = true;

        protected override void OnEvent(EventMessage em, ref object paramRef)
        {
            base.OnEvent(em, ref paramRef);

            if (targetObj == null)
                targetObj = gameObject;

            if (unlock)
            {
                if (!em.paramBool)
                {
                    ColliderLocker.Lock(targetObj, flag, bRecursive);
                }
                else
                {
                    ColliderLocker.Unlock(targetObj, flag, bRecursive);
                }
            }
            else
            {
                if (em.paramBool)
                {
                    ColliderLocker.Lock(targetObj, flag, bRecursive);
                }
                else
                {
                    ColliderLocker.Unlock(targetObj, flag, bRecursive);
                }
            }
        }
    }

}
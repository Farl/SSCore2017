using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Core
{

    public class InitBehaviour : MonoBehaviour
    {
        public virtual int InitOrder
        {
            get { return 0; }
        }

        public virtual bool NeedWait
        {
            get { return false; }
        }

        protected int initHandle = -1;

        protected void Finish()
        {
            if (initHandle >= 0)
                InitManager.Finish(initHandle);
        }

        public virtual void OnInit()
        {
        }

        protected virtual void Awake()
        {
            initHandle = InitManager.Add(this, NeedWait);
        }
    }

}
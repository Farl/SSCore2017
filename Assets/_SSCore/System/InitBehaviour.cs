using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{

    public class InitBehaviour : MonoBehaviour
    {
        public virtual int InitOrder
        {
            get { return 0; }
        }

        public virtual void OnInit()
        {
        }

        protected virtual void Awake()
        {
            InitManager.Add(this);
        }
    }

}
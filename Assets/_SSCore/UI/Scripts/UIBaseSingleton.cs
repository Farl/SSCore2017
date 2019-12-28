﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIBaseSingleton<T> : UIBase where T : UIBase
    {
        private T _instance;
        public T Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (Instance)
            {
                Destroy(this);
            }
            else
            {
                Instance = this as T;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

}
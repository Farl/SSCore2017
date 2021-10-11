using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class StaticSingleton<T>: MonoBehaviour
        where T : MonoBehaviour
    {
        #region API
        public static T Instance
        {
            get
            {
                if (!IsAlive)
                {
                    var go = new GameObject($"(Singleton) {nameof(T)}");
                    _instance = go.AddComponent<T>();
                }
                return _instance;
            }
        }
        public static bool IsAlive
        {
            get
            {
                return _instance != null;
            }
        }
        #endregion

        #region Protected
        protected virtual void Awake()
        {
            if (IsAlive)
            {
                Destroy(this);
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
        }
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        #endregion

        private static T _instance;


    }
}

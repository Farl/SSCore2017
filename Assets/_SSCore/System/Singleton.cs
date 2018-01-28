using UnityEngine;

namespace SS
{

    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        private static object _lock = new object();

        protected virtual void Awake()
        {
            if (!_instance)
            {
                lock (_lock)
                {
                    _instance = this as T;
                }
            }
            else
            {
                Destroy(this);
            }
        }

        public static bool IsAlive
        {
            get
            {
                return _instance != null;
            }
        }

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarningFormat
                            (@"[Singleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T));
                        return null;
                    }
                }

                // For multi-thread protection
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            if (Debug.isDebugBuild)
                            {
                                Debug.LogErrorFormat
                                    (@"[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopenning the scene might fix it.");
                                return _instance;
                            }
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject(typeof(T).ToString(), typeof(T));
                        }
                        else
                        {
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }

}
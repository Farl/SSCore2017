// Create by Claude.AI
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace SS
{

    // Helper class to check if we're on the main thread
    public static class UnityThread
    {
        private static int mainThreadId;
        public static bool isMainThread => System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
    }

    // Singleton to handle main thread dispatching
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher instance;
        private readonly System.Collections.Generic.Queue<Action> executionQueue =
            new System.Collections.Generic.Queue<Action>();

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UnityMainThreadDispatcher>();
                    if (instance == null)
                    {
                        var go = new GameObject("UnityMainThreadDispatcher");
                        instance = go.AddComponent<UnityMainThreadDispatcher>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        public void Enqueue(Action action)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}
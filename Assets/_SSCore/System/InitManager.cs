using UnityEngine;
using System.Collections.Generic;

namespace SS
{
    public class InitManager : MonoBehaviour
    {
        private static List<InitBehaviour> initBehaviours = new List<InitBehaviour>();
        private static List<InitBehaviour> removedBehaviours = new List<InitBehaviour>();
        private static HashSet<int> initQueue = new HashSet<int>();
        private static int initQueueIndex = -1;
        private static bool _isInitializing = false;

        public EventArray eventInitial;

        public static int Add(InitBehaviour initBehaviour, bool waitingQueue = false)
        {
            initBehaviours.Add(initBehaviour);
            if (waitingQueue)
            {
                initQueue.Add(++initQueueIndex);
                return initQueueIndex;
            }
            return -1;
        }

        public static void Remove(InitBehaviour initBehaviour)
        {
            if (_isInitializing)
                removedBehaviours.Add(initBehaviour);
            else
                initBehaviours.Remove(initBehaviour);
        }

        public static void Finish(int handle)
        {
            if (initQueue.Contains(handle))
                initQueue.Remove(handle);
        }

        public static bool IsEmpty()
        {
            return initBehaviours.Count <= 0 && initQueue.Count <= 0;
        }

        void Awake()
        {
            eventInitial.Broadcast(this);
        }

        void OnDestroy()
        {
        }

        private void Start()
        {
            _isInitializing = true;

            // Sort
            initBehaviours.Sort((x, y) => x.InitOrder.CompareTo(y.InitOrder));

            // Init
            foreach (InitBehaviour b in initBehaviours)
            {
                if (!removedBehaviours.Contains(b))
                    b.OnInit();
            }

            // Clear
            initBehaviours.Clear();
            removedBehaviours.Clear();

            _isInitializing = false;
        }
    }
}

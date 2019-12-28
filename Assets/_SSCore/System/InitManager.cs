using UnityEngine;
using System.Collections.Generic;

namespace SS
{
    public class InitManager : MonoBehaviour
    {
        private static List<InitBehaviour> initBehaviours = new List<InitBehaviour>();
        private static List<InitBehaviour> removedBehaviours = new List<InitBehaviour>();
        private static bool _isInitializing = false;

        public EventArray eventInitial;
        public EventArray eventShutdown;

        public static void Add(InitBehaviour initBehaviour)
        {
            initBehaviours.Add(initBehaviour);
        }

        public static void Remove(InitBehaviour initBehaviour)
        {
            if (_isInitializing)
                removedBehaviours.Add(initBehaviour);
            else
                initBehaviours.Remove(initBehaviour);
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            eventInitial.Broadcast(this);
        }

        void OnDestroy()
        {
            eventShutdown.Broadcast(this);
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

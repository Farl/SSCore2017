using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace SS.Core
{
    public class AnimatorHandler : MonoBehaviour
    {

        public bool keepAnimatorControllerStateOnDisable = false;
        private Animator animator;
        private AnimatorControllerParameter[] parameters;
        Dictionary<string, AnimatorControllerParameter> parameterMap = new Dictionary<string, AnimatorControllerParameter>();

        public class AnimatorLayer
        {
            public Dictionary<string, AnimatorStateInfo> stateMap = new Dictionary<string, AnimatorStateInfo>();
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator)
            {
                // Runtime animator controller
                var a = animator.runtimeAnimatorController;
                if (a != null)
                {
                    if (a.GetType() == typeof(AnimatorOverrideController))
                    {
                        var ao = (AnimatorOverrideController)a;
                        a = ao.runtimeAnimatorController;
                    }
                }

                // Parameters
                parameters = animator.parameters;
                foreach (var p in parameters)
                {
                    parameterMap.Add(p.name, p);
                }

                RecursiveDump(animator.playableGraph);

                animator.keepAnimatorStateOnDisable = keepAnimatorControllerStateOnDisable;

            }
        }

        private void RecursiveDump(PlayableGraph graph)
        {
            var count = graph.GetRootPlayableCount();
            for (int i = 0; i < count; i++)
            {
                var p = graph.GetRootPlayable(i);
                Debug.Log(p.ToString());
                RecursiveDump(p);
            }
        }

        private void RecursiveDump(Playable p)
        {
            if (!p.IsValid<Playable>())
                return;
            var pCount = p.GetOutputCount<Playable>();
            for (int j = 0; j < pCount; j++)
            {
                var ip = p.GetOutput<Playable>(j);
                Debug.Log(ip.ToString());
                RecursiveDump(ip);
            }
        }
    }

}

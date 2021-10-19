using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class EventToAnimatorParameter : EventListener
    {
        public Animator animator;

        protected override void Awake()
        {
            base.Awake();
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
        }

        [System.Serializable]
        public class AnimatorParameter
        {
            public AnimatorControllerParameterType type;
            public string name;
            public float numericalValue;
        }

        public AnimatorParameter[] datas;

        protected override void OnEvent(EventMessage em, ref object paramRef)
        {
            base.OnEvent(em, ref paramRef);
            if (!animator)
                return;
            foreach (var data in datas)
            {
                switch (data.type)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(data.name, data.numericalValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(data.name, (int)data.numericalValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(data.name, data.numericalValue != 0);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        animator.SetTrigger(data.name);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

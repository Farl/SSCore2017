using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS
{
    public class AnimatorStateBehaviour : StateMachineBehaviour
    {
        [Flags]
        public enum SendStateEvent
        {
            Enter = 1 << 0,
            Exit = 1 << 1,
            Update = 1 << 2,
            Move = 1 << 3,
            IK = 1 << 4
        }

        public SendStateEvent stateEvent;

        private T GetInterface<T>(Animator animator) where T : IAnimatorState
        {
            return animator.GetComponentInParent<T>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateEvent.HasFlag(SendStateEvent.Enter))
            {
                var stateEnter = GetInterface<IAnimatorStateEnter>(animator);
                if (stateEnter != null)
                    stateEnter.OnStateEnter(animator, stateInfo, layerIndex);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateEvent.HasFlag(SendStateEvent.Exit))
            {
                var stateExit = GetInterface<IAnimatorStateExit>(animator);
                if (stateExit != null)
                    stateExit.OnStateExit(animator, stateInfo, layerIndex);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateEvent.HasFlag(SendStateEvent.Update))
            {
                var stateUpdate = GetInterface<IAnimatorStateUpdate>(animator);
                if (stateUpdate != null)
                    stateUpdate.OnStateUpdate(animator, stateInfo, layerIndex);
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateEvent.HasFlag(SendStateEvent.Move))
            {
                var stateMove = GetInterface<IAnimatorStateMove>(animator);
                if (stateMove != null)
                    stateMove.OnStateMove(animator, stateInfo, layerIndex);
            }
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateEvent.HasFlag(SendStateEvent.IK))
            {
                var stateIK = GetInterface<IAnimatorStateIK>(animator);
                if (stateIK != null)
                    stateIK.OnStateIK(animator, stateInfo, layerIndex);
            }
        }
    }

}
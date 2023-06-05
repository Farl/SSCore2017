using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIStateBehaviour : StateMachineBehaviour
    {
        private UIEntity entity;

        private UIEntity GetPlayer(Animator animator)
        {
            if (entity == null)
                entity = animator.GetComponentInParent<UIEntity>();
            return entity;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var entity = GetPlayer(animator);
            if (entity)
            {
                entity.OnEnterAnimatorState(stateInfo, layerIndex);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var entity = GetPlayer(animator);
            if (entity)
            {
                entity.OnUpdateAnimatorState(stateInfo, layerIndex);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var entity = GetPlayer(animator);
            if (entity)
            {
                entity.OnExitAnimatorState(stateInfo, layerIndex);
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public interface IAnimatorState
    {

    }
    public interface IAnimatorStateEnter: IAnimatorState
    {
        public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
    public interface IAnimatorStateExit : IAnimatorState
    {
        public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
    public interface IAnimatorStateUpdate : IAnimatorState
    {
        public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
    public interface IAnimatorStateMove : IAnimatorState
    {
        public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
    public interface IAnimatorStateIK : IAnimatorState
    {
        public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}

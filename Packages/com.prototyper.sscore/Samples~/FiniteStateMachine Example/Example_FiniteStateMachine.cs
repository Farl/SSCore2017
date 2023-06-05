using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SS.Core
{
    public class Example_FiniteStateMachine : MonoBehaviour
    {
        public enum State
        {
            S0,
            S1,
            S2,
        }

        private FiniteStateMachine<State> stateMachine = new FiniteStateMachine<State>();

        private void Awake()
        {

            stateMachine.AddState(State.S0, Enter0, Leave0);
            stateMachine.AddState(State.S1, Enter1, Leave1);
            stateMachine.AddState(State.S2, Enter2, Leave2);

        }

        [ContextMenu("S0")]
        void SetState0()
        {
            SetNextState(State.S0);
        }
        [ContextMenu("S1")]
        void SetState1()
        {
            SetNextState(State.S1);
        }
        [ContextMenu("S2")]
        void SetState2()
        {
            SetNextState(State.S2);
        }

        public void SetNextState(State nextState)
        {
            stateMachine.SetNextState(nextState);
        }

        void Enter0()
        {
            Debug.Log("Enter 0");
        }
        void Enter1()
        {
            Debug.Log("Enter 1");
        }
        void Enter2()
        {
            Debug.Log("Enter 2");
        }
        void Leave0()
        {
            Debug.Log("Leave 0");
        }
        void Leave1()
        {
            Debug.Log("Leave 1");
        }
        void Leave2()
        {
            Debug.Log("Leave 2");
        }
    }

}

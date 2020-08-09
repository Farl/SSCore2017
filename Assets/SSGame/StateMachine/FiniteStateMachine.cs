/**
 * FSM
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FiniteStateMachine<T> where T : struct, IConvertible
{
    private class StateInfo
    {
        public T state;
        public System.Action enterAction;
        public System.Action leaveAction;
        public System.Action updateAction;
    }
    private Dictionary<T, StateInfo> stateMap = new Dictionary<T, StateInfo>();
    private T currentState;
    private T nextState;
    private StateInfo currentStateInfo;
    private StateInfo nextStateInfo;

    public FiniteStateMachine()
    {
        currentState = default(T);
        currentStateInfo = new StateInfo() { state = currentState };
        stateMap.Add(currentState, currentStateInfo);
    }

    public void AddState(T state, System.Action enterAction = null, System.Action leaveAction = null, System.Action updateAction = null)
    {
        StateInfo si;
        if (!stateMap.TryGetValue(state, out si))
        {
            si = new StateInfo() { state = state };
            stateMap.Add(state, si);
        }
        if (si != null)
        {
            if (enterAction != null)
                si.enterAction += enterAction;
            if (leaveAction != null)
                si.leaveAction += leaveAction;
            if (updateAction != null)
                si.updateAction += updateAction;
        }
    }

    public T GetCurrentState()
    {
        return currentState;
    }

    public T GetNextState()
    {
        return nextState;
    }

    private StateInfo CheckCanEnter(T nextState)
    {
        StateInfo si = null;
        if (stateMap.TryGetValue(nextState, out si))
        {
            
        }
        return si;
    }

    private void Leave(T nextState, StateInfo nextStateInfo)
    {
        if (currentStateInfo != null && currentStateInfo.leaveAction != null)
        {
            currentStateInfo.leaveAction();
        }
    }

    private void Enter(T _nextState, StateInfo _nextStateInfo)
    {
        if (_nextStateInfo != null && _nextStateInfo.enterAction != null)
        {
            nextState = _nextState;
            nextStateInfo = _nextStateInfo;
            nextStateInfo.enterAction();
        }
    }

    public void SetNextState(T nextState)
    {
        StateInfo nextStateInfo = CheckCanEnter(nextState);
        if (nextStateInfo != null)
        {
            Leave(nextState, nextStateInfo);
            Enter(nextState, nextStateInfo);
            currentState = nextState;
            currentStateInfo = nextStateInfo;
        }
    }

    public void Update()
    {
        if (currentStateInfo != null && currentStateInfo.updateAction != null)
        {
            currentStateInfo.updateAction();
        }
    }
}

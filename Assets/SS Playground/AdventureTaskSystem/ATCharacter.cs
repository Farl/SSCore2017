using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SS.Core;

public class ATCharacter : MonoBehaviour {

    public enum State
    {
        Observe,
        Action
    }

    [SerializeField] Text debugStateText;

    [SerializeField] float observation = 10;
    [SerializeField] float strength = 5;
    
    #pragma warning disable 0414
    [SerializeField] float repair = 5;
    [SerializeField] float agile = 5;

	FiniteStateMachine<State> stateMachine = new FiniteStateMachine<State>();
    private AdventureTask currTask;
    private ATNode currNode;
    private Animator _animator;

	
	void Awake ()
    {
        _animator = GetComponent<Animator>();
        stateMachine.AddState(State.Observe, OnEnterState, OnLeaveState, OnUpdateObserve);
        stateMachine.AddState(State.Action, OnEnterState, OnLeaveState, OnUpdateAction);
    }

    void OnEnterState()
    {
        switch (stateMachine.GetNextState())
        {
            case State.Observe:
                _animator.Play("Observe");
                break;
            case State.Action:
                if (currNode)
                {
                    _animator.Play(currNode.animID);
                }
                break;
        }
    }

    void OnLeaveState()
    {

    }

    void OnUpdateObserve()
    {
        if (currTask)
        {
            if (!currNode)
            {
                List<ATNode> visableNode = currTask.visiableNodes;
                if (visableNode.Count <= 0)
                {
                }
                else
                {
                    // TODO: Random or sort
                    foreach (ATNode node in visableNode)
                    {
                        currNode = node;
                        break;
                    }
                }
            }
            else
            {
                if (currNode.Observe(observation * Time.deltaTime))
                {
                    transform.position = currNode.transform.position + new Vector3(0, 0, -1);
                    stateMachine.SetNextState(State.Action);
                }
                else
                {

                }
            }
        }
    }

    void OnUpdateAction()
    {
        if (currNode)
        {
            switch (currNode.nodeType)
            {
                case ATNode.Type.Goal:
                    break;
                default:
                    {
                        if (currNode.Attack(strength * Time.deltaTime))
                        {
                            currNode.gameObject.SetActive(false);
                            currNode = null;
                            if (currTask)
                            {
                                currTask.Refresh();
                            }
                        }
                        else
                        {

                        }
                    }
                    break;
            }
        }
        else
        {
            stateMachine.SetNextState(State.Observe);
        }
    }
	
	public void OnUpdate (AdventureTask task) {
        if (debugStateText)
        {
            debugStateText.text = stateMachine.GetCurrentState().ToString();
        }
        currTask = task;
        stateMachine.Update();
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.Core;

public class RFCharacter : MonoBehaviour
{
    public enum State
    {
        None,
        Idle,
        HardAttack,
        Attack,
        Defense
    }
    enum LocomotionState
    {
        Idle,
        Move
    }

    public float moveSpeed = 5;

    FiniteStateMachine<State> stateMachine = new FiniteStateMachine<State>();
    FiniteStateMachine<LocomotionState> locomotionStateMachine = new FiniteStateMachine<LocomotionState>();
    Vector2 moveAxis = Vector2.zero;
    Vector3 moveVec = Vector3.zero;
    Animator animator;
    new Rigidbody rigidbody;
    private Transform _cachedTransform;
    private Transform cachedTransform
    {
        get
        {
            if (_cachedTransform == null)
                _cachedTransform = transform;
            return _cachedTransform;
        }
    }

    private IRFController controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<IRFController>();

        stateMachine.AddState(State.Idle, OnEnterState, OnLeaveState, OnUpdateState);
        stateMachine.AddState(State.HardAttack, OnEnterState, OnLeaveState, OnUpdateState);
        stateMachine.AddState(State.Attack, OnEnterState, OnLeaveState, OnUpdateState);
        stateMachine.AddState(State.Defense, OnEnterState, OnLeaveState, OnUpdateState);

        locomotionStateMachine.AddState(LocomotionState.Idle, OnEnterLocomotion, OnLeaveLocomotion, OnUpdateLocomotion);
        locomotionStateMachine.AddState(LocomotionState.Move, OnEnterLocomotion, OnLeaveLocomotion, OnUpdateLocomotion);

        if (RhythmFight.Instance)
        {
            RhythmFight.Instance.Register(this);
        }
    }

    private void OnDestroy()
    {
        if (RhythmFight.Instance)
        {
            RhythmFight.Instance.Unregister(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Kill height
        if (cachedTransform.position.y < -2)
        {
            if (controller == null)
            {

                // Reload scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
        UpdateInput();
        
        stateMachine.Update();
        locomotionStateMachine.Update();
    }

    void UpdateInput()
    {

        RFCharacter enemy = RhythmFight.Instance.GetNearestEnemy(this);
        if (controller != null)
        {
            controller.SetEnemy(enemy);
            controller.UpdateInput(out State targetState, out moveVec);
            if (targetState != State.None)
                stateMachine.SetNextState(targetState);

            return;
        }
        else
        {
            // Player
            if (Input.GetKeyDown(KeyCode.I))
            {
                stateMachine.SetNextState(State.HardAttack);
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                stateMachine.SetNextState(State.Attack);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                stateMachine.SetNextState(State.Defense);
            }
            moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

    }

    void OnEnterState()
    {
        Debug.Log(stateMachine.GetNextState());
        switch (stateMachine.GetNextState())
        {
            case State.Idle:
                break;
            case State.HardAttack:
                if (animator)
                {
                    animator.SetTrigger("hardAttack");
                }
                break;
            case State.Attack:
                if (animator)
                {
                    animator.SetTrigger("attack");
                }
                break;
            case State.Defense:
                if (animator)
                {
                    animator.SetTrigger("defense");
                }
                break;
        }
    }

    void OnLeaveState()
    {

    }

    void OnUpdateState()
    {
        switch (stateMachine.GetCurrentState())
        {
            case State.Idle:
                break;
            case State.HardAttack:
                if (stateMachine.timer > 3)
                {
                    stateMachine.SetNextState(State.Idle);
                }
                break;
            case State.Attack:
                if (stateMachine.timer > 3)
                {
                    stateMachine.SetNextState(State.Idle);
                }
                break;
            case State.Defense:
                if (stateMachine.timer > 3)
                {
                    stateMachine.SetNextState(State.Idle);
                }
                break;
            default:
                break;
        }

    }

    void OnEnterLocomotion()
    {

    }

    void OnLeaveLocomotion()
    {

    }

    void OnUpdateLocomotion()
    {
        if (controller == null)
            moveVec = Vector3.zero;

        switch (locomotionStateMachine.GetNextState())
        {
            case LocomotionState.Idle:
            case LocomotionState.Move:
                {
                    if (controller != null)
                    {
                        
                    }
                    else
                    {
                        Camera cam = Camera.main;
                        if (cam)
                        {
                            Vector3 camForward = cam.transform.forward;
                            Vector3 camRight = cam.transform.right;

                            moveVec = camForward * moveAxis.y + camRight * moveAxis.x;
                        }
                    }

                    if (moveVec.magnitude > 0.01f)
                        transform.forward = moveVec.normalized;
                }
                break;
            default:
                break;
        }
    }

    private void OnAnimatorMove()
    {
        if (rigidbody)
        {
            Vector3 v = rigidbody.velocity;
            Vector3 newV = animator.velocity + moveVec * moveSpeed;
            rigidbody.velocity = new Vector3(newV.x, newV.y + v.y, newV.z);
        }
    }

    public void Beat()
    {
        if (controller != null)
        {
            controller.Beat();
        }    
    }
}

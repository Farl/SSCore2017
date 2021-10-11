using UnityEngine;
using System.Collections;

public class PTCharacter : MonoBehaviour {
	public new Rigidbody rigidbody;
	public PTController controller;
	public Animator animator;
	public PTCharacterAttribute attribute;

	enum State {
		INVALID = -1,
		IDLE,
		RUN,
		HIT,
		ATTACK,
		CLIMB,
	}

	enum MotionState {
		INVALID = -1,
		IDLE,
		RUN,
		HIT,
		ATTACK,
		CLIMB,
	}

	private State currState;
	private float stateTimer = 0;
	private MotionState currMotion;
	private bool inCinematic;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (controller) {
			// Update input
		}

		UpdateState ();

		UpdateMotionControl ();
	}

	#region State
	void UpdateState()
	{
		switch (currState) {

		case State.INVALID:
			SetState (State.IDLE);
			break;
		case State.IDLE:
			if (controller) {
				if (controller.targetVec.magnitude > 0.01f) {
					SetState (State.RUN);
				}
			}
			break;
		case State.RUN:
			if (controller) {
				if (controller.targetVec.magnitude < 0.01f) {
					SetState (State.IDLE);
				}
			}
			break;
		case State.HIT:
			break;
		case State.ATTACK:
			break;
		case State.CLIMB:
			break;
		default:
			break;
		}

		stateTimer += Time.deltaTime;
	}

	void SetState(PTCharacter.State nextState)
	{
		if (CanEnterState (nextState)) {
			LeaveState (nextState);
			EnterState (nextState);
			stateTimer = 0;
			currState = nextState;
		}
	}

	bool CanEnterState(PTCharacter.State nextState)
	{
		return true;
	}

	void EnterState(PTCharacter.State nextState)
	{
		switch (nextState) {

		case State.INVALID:
			break;
		case State.IDLE:
			SetMotion (MotionState.IDLE);
			break;
		case State.RUN:
			SetMotion (MotionState.RUN);
			break;
		case State.HIT:
			SetMotion (MotionState.HIT);
			break;
		case State.ATTACK:
			SetMotion (MotionState.ATTACK);
			break;
		case State.CLIMB:
			SetMotion (MotionState.CLIMB);
			break;
		default:
			break;
		}
	}

	void LeaveState(PTCharacter.State nextState)
	{
		switch (currState) {

		case State.INVALID:
			break;
		case State.IDLE:
			break;
		case State.RUN:
			break;
		case State.HIT:
			break;
		case State.ATTACK:
			break;
		case State.CLIMB:
			break;
		default:
			break;
		}
	}
	#endregion State

	#region MotionControl
	void UpdateMotionControl()
	{
		switch (currMotion) {

		case MotionState.INVALID:
			break;
		case MotionState.IDLE:
			break;
		case MotionState.RUN:
			if (controller) {
				if (rigidbody) {
					Vector3 desireVelocity = controller.targetVec.normalized * ((attribute) ? attribute.maxSpeed : 1.0f);

					float origUp = rigidbody.velocity.y;

					desireVelocity = desireVelocity * Mathf.Min (controller.targetVec.magnitude, 1.0f) / 1.0f;
					desireVelocity.y = origUp;

					rigidbody.velocity = desireVelocity;
				}
			}
			break;
		case MotionState.HIT:
			break;
		case MotionState.ATTACK:
			break;
		case MotionState.CLIMB:
			break;
		default:
			break;
		}
	}

	void SetMotion(PTCharacter.MotionState nextMotion)
	{
		if (CanEnterMotion (nextMotion)) {
			LeaveMotion (nextMotion);
			EnterMotion (nextMotion);
			currMotion = nextMotion;
		}
	}

	bool CanEnterMotion(PTCharacter.MotionState nextMotion)
	{
		return true;
	}

	void EnterMotion(PTCharacter.MotionState nextMotion)
	{
		switch (nextMotion) {

		case MotionState.INVALID:
			break;
		case MotionState.IDLE:
			break;
		case MotionState.RUN:
			break;
		case MotionState.HIT:
			break;
		case MotionState.ATTACK:
			break;
		case MotionState.CLIMB:
			break;
		default:
			break;
		}
	}

	void LeaveMotion(PTCharacter.MotionState nextMotion)
	{
		switch (currMotion) {

		case MotionState.INVALID:
			break;
		case MotionState.IDLE:
			break;
		case MotionState.RUN:
			break;
		case MotionState.HIT:
			break;
		case MotionState.ATTACK:
			break;
		case MotionState.CLIMB:
			break;
		default:
			break;
		}
	}
	#endregion MotionControl
}

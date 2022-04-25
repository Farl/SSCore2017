using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFAIController : MonoBehaviour
{
    private bool beat;

    public void UpdateInput(out RFCharacter.State state, out Vector3 moveVec)
    {
        state = RFCharacter.State.None;
        moveVec = Vector3.zero;
        if (beat)
        {
            state = RFCharacter.State.Attack;
            beat = false;
        }
    }

    public void Beat()
    {
        beat = true;
    }
}

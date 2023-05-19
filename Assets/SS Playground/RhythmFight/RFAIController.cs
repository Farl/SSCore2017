using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRFController
{
    void SetEnemy(RFCharacter enemy);
    void UpdateInput(out RFCharacter.State state, out Vector3 moveVec);
    void Beat();
}
public class RFAIController : MonoBehaviour, IRFController
{
    public float maxSpeed = 1;
    private bool beat;
    private RFCharacter enemy;
    
    public void SetEnemy(RFCharacter enemy)
    {
        this.enemy = enemy;
    }

    public void UpdateInput(out RFCharacter.State state, out Vector3 moveVec)
    {
        state = RFCharacter.State.None;
        moveVec = Vector3.zero;

        if (beat)
        {
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                state = RFCharacter.State.Attack;
            }
            else
            {

                if (enemy)
                {
                    Vector3 targetVec = enemy.transform.position - transform.position;
                    Debug.DrawLine(enemy.transform.position, transform.position);

                    moveVec = Vector3.ClampMagnitude(targetVec, maxSpeed);
                }
            }
            beat = false;
        }
    }

    public void Beat()
    {
        beat = true;
    }
}

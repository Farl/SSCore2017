using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;

public class Example_InitBehaviour_WaitSeconds : InitBehaviour {

    public float waitSeconds = 5;
    public bool toggle = false;

    public override int InitOrder
    {
        get
        {
            return base.InitOrder;
        }
    }

    public override bool NeedWait
    {
        get
        {
            return true;
        }
    }

    IEnumerator Start()
    {
        toggle = false;
        yield return new WaitForSecondsRealtime(waitSeconds);
        toggle = true;
        Finish();
    }
}

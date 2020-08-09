using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;

public class Example_InitBehaviour : InitBehaviour {

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
        yield return new WaitForSecondsRealtime(5);
        Finish();
    }
}

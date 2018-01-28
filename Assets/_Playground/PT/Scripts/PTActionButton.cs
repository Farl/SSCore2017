using UnityEngine;
using System.Collections;

public class PTActionButton : PTTrigger {

    protected override Color GetGizmosMeshColor(bool focused)
    {
        return new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    protected override Color GetGizmosWireColor(bool focused)
    {
        return new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
    
}

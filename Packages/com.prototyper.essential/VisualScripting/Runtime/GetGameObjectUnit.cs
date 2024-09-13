/**
 * GetGameObjectUnit.cs
 * Input name of GameObject and return GameObject from ObjectMap
 */

#if USE_VISUAL_SCRIPTING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Get GameObject")]
    [UnitShortTitle("Get GameObject")]
    public class GetGameObjectUnit : Unit
    {
        [DoNotSerialize, PortLabelHidden] public ValueInput nameInput;
        [DoNotSerialize, PortLabelHidden] public ValueOutput output;

        protected override void Definition()
        {
            nameInput = ValueInput<string>(nameof(nameInput), null);
            output = ValueOutput<GameObject>(nameof(output), (flow) =>
            {
                var name = flow.GetValue<string>(nameInput);
                return ObjectMap.GetGameObjectByName(name);
            });
            Requirement(nameInput, output);
        }
    }
}
#endif
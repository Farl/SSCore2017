#if USE_VISUAL_SCRIPTING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SS
{
    [UnitCategory("SS")]
    [UnitTitle("Send Remote Event")]
    [UnitShortTitle("Send Remote Event")]
    public class RemoteEventUnit : Unit
    {
        [DoNotSerialize, PortLabelHidden] public ControlInput Input;
        [DoNotSerialize, PortLabel("Object ID")] public ValueInput objectIDInput;
        [DoNotSerialize, PortLabel("Event Name")] public ValueInput eventNameInput;
        [DoNotSerialize, PortLabelHidden] public ControlOutput Output;

        protected override void Definition()
        {
            Input = ControlInput(nameof(Input), Send);
            objectIDInput = ValueInput<string>(nameof(objectIDInput), string.Empty);
            eventNameInput = ValueInput<string>(nameof(eventNameInput), string.Empty);
            Output = ControlOutput(nameof(Output));
            Succession(Input, Output);
            Requirement(eventNameInput, Input);
        }

        public ControlOutput Send(Flow flow)
        {
            var objectID = flow.GetValue<string>(objectIDInput);
            var eventName = flow.GetValue<string>(eventNameInput);
            var reciever = ObjectMap.GetComponentByName<RemoteEventReciever>(objectID);
            if (reciever != null)
            {
                reciever.OnRecieve(eventName);
            }
            return Output;
        }
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class EventToUnityEvent : EventListener
{
    [System.Serializable]
    public class MyUnityEvent : UnityEvent<MyUnityEvent>
    {

    }
    public MyUnityEvent onEvent;

    protected override void OnEvent(bool paramBool)
    {
        base.OnEvent(paramBool);
        MyUnityEvent e = new MyUnityEvent();
        onEvent.Invoke(e);
    }
}

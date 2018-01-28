using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using SS;

public class QuickButton : UIBehaviour, IPointerClickHandler
{
	public enum ButtonType
	{
		Once,
		Repeat,
	}

	public ButtonType type = ButtonType.Once;
	
	// Event delegates triggered on click.
	[FormerlySerializedAs("onClick")]
	[SerializeField]
	private Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();

	public EventTrigger.TriggerEvent m_triggerEvent = new EventTrigger.TriggerEvent();

	public EventArray eventArray;

	private string soundID = "UI_Click";
	public string overrideSoundID;

	private PointerEventData pointerEventData;

	public bool scaleEffect = true;

	private QuickPanel page;

	private float autoLockOnceTime = 1.5f;
	private float autoLockRepeatTime = 0.1f;
	private float delayInvokeTime = 0.2f;

	private string lockFlag = "QuickButton.AutoLock";

	protected override void Start()
	{
		base.Start();

		// if you don't name each button, this may be fail
		lockFlag = string.Format("{0}.{1}", name, lockFlag);

		// button scale effect
		if (scaleEffect)
		{
			gameObject.AddComponent<EventToShake>();
		}

		// copy event from other script (because we need to control)
		if (m_OnClick.GetPersistentEventCount() <= 0)
		{
			// copy click event from EventTrigger
			EventTrigger trg = GetComponent<EventTrigger>();
			if (trg && trg.triggers != null)
			{
				foreach(EventTrigger.Entry entry in trg.triggers)
				{
					if (entry.eventID == EventTriggerType.PointerClick)
					{
						m_triggerEvent = entry.callback;
						entry.callback = new EventTrigger.TriggerEvent();
					}
				}
			}
			
			// copy click event from Button
			Button but = GetComponent<Button>();
			if (but && but.onClick != null)
			{
				m_OnClick = but.onClick;
				but.onClick = new Button.ButtonClickedEvent();
			}
		}

		// register page
		page = GetComponentInParent<QuickPanel>();
	}
	
	public Button.ButtonClickedEvent onClick
	{
		get { return m_OnClick; }
		set { m_OnClick = value; }
	}
	
	private void Press()
	{
		// prevent Timer error
		if (this == null)
			return;

		if (!IsActive())
			return;

		// click event
		m_OnClick.Invoke();

		// trigger event
		m_triggerEvent.Invoke(pointerEventData);

		// string event
		eventArray.Broadcast(this);
	}

	void Lock(bool isLock)
	{
		// lock interactive
		if (page)
		{
			if (isLock)
				page.Lock(lockFlag);
			else
				page.Unlock(lockFlag);

			UILocker.LockRecursive(page.gameObject, lockFlag, isLock);
		}
	}

	void Unlock()
	{
		// prevent Timer error
		if (this == null)
			return;

		Lock(false);
	}

	void PlaySound()
	{
        EventMessage em = new EventMessage((string.IsNullOrEmpty(overrideSoundID)) ? soundID : overrideSoundID);
        EventManager.Broadcast(em);
	}
	
	// Trigger all registered callbacks.
	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;

		pointerEventData = eventData;

		{
			EventTimer.AddTimer(delayInvokeTime, Press, null, false);
		}

		// lock
		Lock(true);
		switch (type)
		{
		case ButtonType.Once:
		{
			EventTimer.AddTimer(autoLockOnceTime, Unlock, null, false);
		}
			break;
		case ButtonType.Repeat:
		{
			EventTimer.AddTimer(autoLockRepeatTime, Unlock, null, false);
		}
			break;
		}

		// Play sound
		PlaySound();
	}
}
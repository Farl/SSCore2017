using UnityEngine;
using System.Collections;
using SS;

[DisallowMultipleComponent]
public class QuickPanel : MonoBehaviour
{
	public bool startEnabled;

	public bool destroyWhenClose;

	private bool active;

	private bool isRegisterEventListener;

	public string inSound = "UI_In";

	public string outSound = "UI_Out";

	private StringFlag lockFlag = new StringFlag();

	private CanvasGroup canvasGroup;

	private UIBlocker blocker;

	private int destroyCount;
	
	public void Lock(string flag)
	{
		lockFlag.AddFlag(flag);

		if (canvasGroup)
		{
			canvasGroup.blocksRaycasts = false;
		}
	}

	public void Unlock(string flag)
	{
		lockFlag.RemoveFlag(flag);
		
		if (lockFlag.IsEmpty() && canvasGroup)
		{
			canvasGroup.blocksRaycasts = true;
		}
	}

	[ContextMenu("Dump Locker")]
	public void DumpLocker()
	{
		lockFlag.Dump();
	}

	private void Awake()
	{
		SetActive(startEnabled);

		Register();

		canvasGroup = GetComponent<CanvasGroup>();
		if (!canvasGroup)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();

		// UI blocker
		blocker = GetComponent<UIBlocker>();
		if (!blocker)
		{
			blocker = gameObject.AddComponent<UIBlocker>();
			UISettings.UIPageData data = UISettings.GetPageData(name);
			if (data != null)
			{
				blocker.Setup(name, data.blockUI);
			}
		}
		if (blocker)
		{
			blocker.ForceStart();
		}
	}

	// call from spawner
	public void ForceStart()
	{
		// TODO: call EventListener Start
		SetActive(startEnabled);
		Register();
	}

	public bool IsActive()
	{
		return active;
	}

	private void Register()
	{
		if (!isRegisterEventListener)
		{
			name = name.Replace("(Clone)", "");
			UISystem.RegisterObject(gameObject, name);
			EventManager.AddEventListener(UISystem.GetPageEventID(name), OnEvent);
			isRegisterEventListener = true;
		}
	}

	private void Unregister()
	{
		if (isRegisterEventListener)
		{
			UISystem.UnregisterObject(gameObject, name);
			EventManager.RemoveEventListener(UISystem.GetPageEventID(name), OnEvent);
		}
	}
	
	[ContextMenu("Open")]
	public void Open()
	{
		InTransition();
	}
	[ContextMenu("Close")]
	public void Close()
	{
		OutTransition();
	}

    private void InTransition()
    {
        active = true;

        if (blocker)
        {
            blocker.Lock();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            if (trans)
            {
                QuickShelf comp = trans.GetOrAddComponent<QuickShelf>();
                comp.Init();
                comp.In();
            }
        }

        EventMessage em = new EventMessage(inSound, _paramBool: true);
        EventManager.Broadcast(em);
    }
	
	private void OutTransition()
	{
		destroyCount = 0;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform trans = transform.GetChild (i);
			if (trans)
			{
				QuickShelf comp = trans.GetOrAddComponent<QuickShelf>();
				comp.Out(this);
				destroyCount++;
			}
		}

        EventMessage em = new EventMessage(outSound, _paramBool: true);
        EventManager.Broadcast(em);

		if (destroyCount <= 0)
		{
			Closed();
		}
	}

	private void Closed()
	{
		active = false;
		
		if (blocker)
		{
			blocker.Unlock();
		}

		if (destroyWhenClose)
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		Unregister();
	}
	
	private void Start()
	{
		if (startEnabled)
		{
			InTransition();
		}
	}

    private void OnEvent(EventMessage em, ref object paramRef)
	{
		if (em.paramBool)
		{
			InTransition();
		}
		else
		{
			OutTransition();
		}
	}

	private void SetActive(bool activate)
	{
		active = activate;
		
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform trans = transform.GetChild (i);
			if (trans)
			{
				trans.gameObject.SetActive(active);
			}
		}
	}

	public void ShelfClosed(QuickShelf shelf)
	{
		if (shelf)
		{
			destroyCount--;
			if (destroyCount <= 0)
			{
				Closed();
			}
		}
	}
}

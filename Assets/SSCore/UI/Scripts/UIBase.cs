using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{

	[DisallowMultipleComponent]
	public class UIBase : MonoBehaviour
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

        private static bool _isInitializing;
        public bool IsInitializing
        {
            get
            {
                return _isInitializing;
            }
            set
            {
                _isInitializing = value;
            }
        }

        public string uiType;

        private bool _isInit;
        public bool IsInit
        {
            get { return _isInit; }
            set { _isInit = value; }
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public virtual void Init()
        {
            if (_isInit)
            {
                // Force awake
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
            else
            {

            }
        }

        public void Show()
        {
            ShowImmediately();
            OnShow();
        }

        protected virtual void OnShow()
        {

        }

        public void Hide()
        {
            HideImmediately();
            OnHide();
        }

        protected virtual void OnHide()
        {

        }

        void ShowImmediately()
        {
            gameObject.SetActive(true);
        }

        void HideImmediately()
        {
            gameObject.SetActive(false);
        }

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

		protected virtual void Awake()
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
				Transform trans = transform.GetChild(i);
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

		protected virtual void OnDestroy()
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
				Transform trans = transform.GetChild(i);
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

}
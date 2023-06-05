using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS.Core
{

	[DisallowMultipleComponent]
	public class UIBase : MonoBehaviour
	{
		public enum ActivateMethod
		{
			Auto = 0,
			Legacy = 1,
		}

		[SerializeField]
		private ActivateMethod m_ActivateMethod;

		public bool startEnabled;

        public bool destroyWhenClose;

		public bool IsActive { get; private set; } = false;
		public bool IsShow { get; private set; } = false;
		public bool IsShowing { get; private set; } = false;
		public bool IsHiding { get; private set; } = false;

        private bool isRegisterEventListener;

        public string inSound = "UI_In";

        public string outSound = "UI_Out";

        private StringFlag lockFlag = new StringFlag();

		private CanvasGroup _canvasGroup;
        private CanvasGroup canvasGroup
        {
            get
            {
				if (_canvasGroup == null)
					_canvasGroup = GetComponent<CanvasGroup>();
				if (_canvasGroup == null)
					_canvasGroup = gameObject.AddComponent<CanvasGroup>();
				return _canvasGroup;
            }
			set
            {
				_canvasGroup = value;
            }
        }

        private UIBlocker blocker;

        private int destroyCount;

		private Transform currTransform;
		public Transform GetTransform()
        {
			if (currTransform == null)
				currTransform = transform;
			return currTransform;
        }


        private bool IsInitializing { get; set; }

		public string UIType
        {
            get
            {
				if (Application.isPlaying)
				{
					if (string.IsNullOrEmpty(uiType))
						uiType = gameObject.name;
				}
				return uiType;
            }
        }

		[SerializeField]
        private string uiType;

        public bool IsInit { get; private set; }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

		protected virtual void OnInit()
        {

        }

        public void Init()
        {
            if (!IsInit && !IsInitializing)
            {
				IsInitializing = true;

                // Force awake
                gameObject.SetActive(false);
                gameObject.SetActive(true);

				SetActive(startEnabled);
				OnInit();

				IsInitializing = false;
				IsInit = true;
			}
        }

        public void Show()
        {
			IsShow = true;
			IsShowing = true;
            ShowImmediately();
            OnShow();
			IsShowing = false; // TODO:
		}

        protected virtual void OnShow()
        {

        }

        public void Hide()
		{
			IsShow = false;
			IsHiding = true;
			HideImmediately(); // TODO:
			OnHide();
			IsHiding = false; // TODO:
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
			Register();

			if (canvasGroup)
            {
				// TODO
            }

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

		private void Register()
		{
			if (!isRegisterEventListener)
			{
				name = name.Replace("(Clone)", "");
				UISystem.Register(this);
				EventManager.AddEventListener(UISystem.GetPageEventID(name), OnEvent);
				isRegisterEventListener = true;
			}
		}

		private void Unregister()
		{
			if (isRegisterEventListener)
			{
				UISystem.Unregister(this);
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
			IsActive = true;

			if (blocker)
			{
				blocker.Lock();
			}

			for (int i = 0; i < GetTransform().childCount; i++)
			{
				Transform trans = GetTransform().GetChild(i);
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
			for (int i = 0; i < GetTransform().childCount; i++)
			{
				Transform trans = GetTransform().GetChild(i);
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
			IsActive = false;

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
			IsActive = activate;

			switch (m_ActivateMethod)
            {
				default:
				case ActivateMethod.Auto:
					gameObject.SetActive(IsActive);
					break;

				case ActivateMethod.Legacy:

					for (int i = 0; i < GetTransform().childCount; i++)
					{
						Transform trans = GetTransform().GetChild(i);
						if (trans)
						{
							trans.gameObject.SetActive(IsActive);
						}
					}
					break;
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
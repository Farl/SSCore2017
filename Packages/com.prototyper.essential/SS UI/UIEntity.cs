using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIEntity : MonoBehaviour
    {
        #region Enums / Classes
        public enum UpdateMethod
        {
            ActiveAndEnabled = 0,
            Always = 1,
            Never = -1,
        }
        #endregion

        #region Inspector
        [SerializeField] protected Animator animator;
        [SerializeField] private string overrideTypeName;
        [SerializeField] protected bool showOnAwake = false;
        [SerializeField] protected UpdateMethod updateMethod = UpdateMethod.Never;
        #endregion

        #region Public
        public bool IsShow { get; private set; }
        public bool IsShowing { get; protected set; } = false;
        public bool IsHiding { get; protected set; } = false;
        public System.Action OnShowBegin;
        public System.Action OnShowComplete;
        public System.Action OnHideBegin;
        public System.Action OnHideComplete;
        public virtual bool IsUpdating
        {
            get
            {
                switch (updateMethod)
                {
                    case UpdateMethod.Never:
                        return false;
                    default:
                    case UpdateMethod.ActiveAndEnabled:
                        return isActiveAndEnabled;
                    case UpdateMethod.Always:
                        return true;
                }
            }
        }

        public string TypeName
        {
            get
            {
                if (!string.IsNullOrEmpty(overrideTypeName))
                    return overrideTypeName;
                return this.GetType().Name;
            }
        }
        public virtual void OnUpdate()
        {

        }

        public virtual void OnRootInitialize()
        {
            gameObject.SetActive(showOnAwake);
        }
        public void Show()
        {
            Show(null);
        }

        public void Hide()
        {
            Hide(null);
        }

        public virtual void Show(params object[] parameters)
        {
            OnShowBegin?.Invoke();
            if (!overrideShow)
                gameObject.SetActive(true);
            OnShow(parameters);
            IsShow = true;
            if (!overrideShow)
                OnShowComplete?.Invoke();
        }

        public virtual void Hide(params object[] parameters)
        {
            OnHideBegin?.Invoke();
            if (!overrideHide)
                gameObject.SetActive(false);
            OnHide(parameters);
            IsShow = false;
            if (!overrideHide)
                OnHideComplete?.Invoke();
        }

        public void ShowUI(string typeName)
        {
            UIManager.ShowUI(typeName);
        }
        public void HideUI(string typeName)
        {
            UIManager.HideUI(typeName);
        }

        public virtual void OnExitAnimatorState(AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnEnterAnimatorState(AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnUpdateAnimatorState(AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void SendMessage(params object[] args)
        {
        }
        #endregion

        #region Protected / Private
        protected bool overrideShow = false;
        protected bool overrideHide = false;
        private void Awake()
        {
            OnEntityAwake();
        }
        private void OnDestroy()
        {
            OnEntityDestroy();
        }

        protected virtual void OnEntityAwake()
        {
            UIManager.Register(this);
        }

        protected virtual void OnEntityDestroy()
        {
            UIManager.Unregister(this);
        }

        protected virtual void OnShow(params object[] parameters)
        {
        }

        protected virtual void OnHide(params object[] parameters)
        {
        }
        #endregion
    }
}

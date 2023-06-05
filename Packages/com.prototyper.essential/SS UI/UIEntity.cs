using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIEntity : MonoBehaviour
    {
        [SerializeField]
        protected Animator animator;

        [SerializeField]
        private string overrideTypeName;

        [SerializeField]
        protected bool showOnAwake = false;

        public bool IsShow { get; private set; }
        public bool IsShowing { get; protected set; } = false;
        public bool IsHiding { get; protected set; } = false;

        protected bool overrideShow = false;
        protected bool overrideHide = false;

        public enum UpdateMethod
        {
            ActiveAndEnabled = 0,
            Always = 1,
            Never = -1,
        }

        [SerializeField]
        protected UpdateMethod updateMethod = UpdateMethod.Never;

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
            get {
                if (!string.IsNullOrEmpty(overrideTypeName))
                    return overrideTypeName;
                return this.GetType().Name;
            }
        }

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

        public virtual void OnUpdate()
        {

        }

        public virtual void OnRootInitialize()
        {
            gameObject.SetActive(showOnAwake);
        }

        protected virtual void OnShow(params object[] parameters)
        {

        }

        protected virtual void OnHide(params object[] parameters)
        {

        }

        public void Show()
        {
            Show(null);
        }

        public void Hide()
        {
            Hide(null);
        }

        public void Show(params object[] parameters)
        {
            if (!overrideShow)
                gameObject.SetActive(true);
            OnShow(parameters);
            IsShow = true;
        }

        public void Hide(params object[] parameters)
        {
            if (!overrideHide)
                gameObject.SetActive(false);
            OnHide(parameters);
            IsShow = false;
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
    }
}

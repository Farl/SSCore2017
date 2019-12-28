using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIBase : MonoBehaviour
    {
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

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {

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
    }

}
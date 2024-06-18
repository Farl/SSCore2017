using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIEntityAnimated : UIEntity
    {
        #region Inspector
        [SerializeField] protected bool m_IsAnimated = false;
        #endregion

        #region Public
        public override void OnRootInitialize()
        {
            base.OnRootInitialize();
            if (m_IsAnimated)
            {
                overrideShow = true;
                overrideHide = true;
            }
            if (showOnAwake)
            {
                Show();
            }
        }
        public override void OnEnterAnimatorState(AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnEnterAnimatorState(stateInfo, layerIndex);
            if (m_IsAnimated)
            {
                if (IsShowing)
                {
                    if (stateInfo.IsName("Opened"))
                    {
                        IsShowing = false;
                        OnShowed();
                    }
                }
                if (IsHiding && stateInfo.IsName("Closed"))
                {
                    IsHiding = false;
                    gameObject.SetActive(false);
                    OnHided();
                }
            }
        }
        #endregion

        #region Private / Protected
        protected override void OnShow(params object[] parameters)
        {
            base.OnShow(parameters);
            if (m_IsAnimated)
            {
                gameObject.SetActive(true);
                if (animator)
                    animator.SetBool("isShow", true);
                IsShowing = true;
            }
            else
            {
                OnShowed();
            }
        }

        protected override void OnHide(params object[] parameters)
        {
            base.OnHide(parameters);
            if (m_IsAnimated)
            {
                if (animator)
                    animator.SetBool("isShow", false);
                IsHiding = true;
            }
            else
            {
                OnHided();
            }
        }

        /// <summary>
        /// Animation is finished
        /// </summary>
        protected virtual void OnHided()
        {
            OnHideComplete?.Invoke();
        }

        /// <summary>
        /// Animation is finished
        /// </summary>
        protected virtual void OnShowed()
        {
            OnShowComplete?.Invoke();
        }
        #endregion
    }
}

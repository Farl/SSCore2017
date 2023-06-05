using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIEntityAnimated : UIEntity
    {
        [SerializeField]
        protected bool m_IsAnimated = false;

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
                    }
                }
                if (IsHiding && stateInfo.IsName("Closed"))
                {
                    IsHiding = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}

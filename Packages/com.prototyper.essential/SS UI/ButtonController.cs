using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SS
{
    public class ButtonController : MonoBehaviour
    {
        [SerializeField]
        protected Button button;

        [SerializeField]
        protected Animator animator;

        [SerializeField]
        protected string onClickSoundEventID = "ButtonClick";

        public virtual void AddListener(UnityAction act)
        {
            if (button != null)
            {
                button.onClick.AddListener(act);
            }
        }

        public virtual bool isInteractable
        {
            get
            {
                if (button != null)
                    return button.IsInteractable();
                return false;
            }
            set
            {
                if (button != null)
                {
                    button.interactable = value;
                }
            }
        }


        protected virtual void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            if (button == null)
            {
                button = GetComponent<Button>();
            }
            if (button != null)
            {
                button.onClick.AddListener(OnClickNonSystemCall);
            }
        }

        protected virtual void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClickNonSystemCall);
            }
        }

        protected virtual void OnEnable()
        {
            if (button != null)
            {
            }
        }

        protected void OnClickSystemCall()
        {
            OnClick(true);
        }

        protected void OnClickNonSystemCall()
        {
            OnClick(false);
        }

        protected virtual void OnClick(bool systemCall)
        {
            SoundSystem.PlayOneShot(onClickSoundEventID);
        }
    }
}

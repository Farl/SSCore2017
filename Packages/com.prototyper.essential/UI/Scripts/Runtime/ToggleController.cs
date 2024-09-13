using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace SS
{
    public class ToggleController : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private string isOnParameterName = "isOn";

        [SerializeField]
        private string isOnSoundEventID = "ToggleOn";

        [SerializeField]
        private string isOffSoundEventID = "ToggleOff";

        public ToggleGroup group => toggle ? toggle.group : null;

        public Graphic targetGraphic => toggle ? toggle.targetGraphic : null;

        public bool IsOn => toggle ? toggle.isOn : false;

        public static bool MuteAll { get; set; } = false;

        public void SendOnValueChanged(bool value)
        {
            if (toggle)
            {
                toggle.onValueChanged?.Invoke(value);
            }
        }

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            if (toggle == null)
            {
                toggle = GetComponent<Toggle>();
            }
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(
                    (b) =>
                    {
                        OnValueChanged(b, false);
                    }
                );
            }
        }

        private void OnEnable()
        {
            if (toggle != null)
            {
                OnValueChanged(toggle.isOn, true);
            }
        }

        private void OnValueChanged(bool b, bool systemCall)
        {
            if (animator != null)
            {
                animator.SetBool(isOnParameterName, b);
            }
            if (!systemCall)
            {
                if (!MuteAll)
                    SoundSystem.PlayOneShot(b ? isOnSoundEventID : isOffSoundEventID);
            }
        }

        public void SetIsOn(bool b, bool systemCall = false)
        {
            if (!toggle)
                return;
            if (!systemCall)
            {
                toggle.isOn = b;
            }
            else
            {
                toggle.SetIsOnWithoutNotify(b);
                OnValueChanged(b, systemCall);
            }
        }

        public void AddListener(UnityAction<bool> callback)
        {
            if (toggle == null)
            {
                toggle = GetComponent<Toggle>();
            }
            if (toggle)
            {
                toggle.onValueChanged.AddListener(callback);
            }
        }

        public void RemoveListener(UnityAction<bool> callback)
        {
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(callback);
            }
        }
    }
}

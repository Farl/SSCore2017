using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
namespace SS
{
    public class DialogPanel : MonoBehaviour, IAnimatorStateEnter
    {
        #region Inspector
        [SerializeField] protected bool showDebugInfo = false;
        [SerializeField] protected Animator animator;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected TextTableMapping titleText;
        [SerializeField] protected TextTableMapping descriptionText;
        [SerializeField] protected List<ButtonData> buttonDatas = new List<ButtonData>();
        [SerializeField] protected float soundLengthPerText = 0.1f;
        [SerializeField] protected Vector2 paddingTime = new Vector2(0.1f, 0.1f);
        [SerializeField] protected bool allowTextIDOnly = false;
        #endregion

        #region Public
        [Serializable]
        public class ButtonData
        {
            public Button button;

            public TextTableMapping buttonText;

            public Image buttonImage;
        }

        public virtual void Initialize()
        {
            if (canvasGroup)
            {
                canvasGroup.alpha = 0;
            }
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
            // Force awake
            gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public virtual void Renew(DialogRequestData requestData)
        {
            // Fast forward Hide action
            if (hidingRequestData != null)
            {
                // Previous request is hiding
                Debug.LogWarning($"Previous request {hidingRequestData.descID} is hiding, may cause some issue", this);
            }
            hidingRequestData = requestData;
            shouldKeepRequest = false;

            // clear listener
            foreach (var b in buttonDatas)
            {
                b.button.onClick.RemoveAllListeners();
            }

            // Skip animation setup
            // ...

            // Fast forward OnClose action
            state = State.Closed;

            if (hidingRequestData == null)
                return;

            CloseButton(hidingRequestData);
            CloseSound(hidingRequestData);

            gameObject.SetActive(false);

            // Backup
            var backupRequestData = hidingRequestData;
            var backupKeepRequest = shouldKeepRequest;

            // Clear first
            hidingRequestData = null;
            shouldKeepRequest = false;

            // Use backup data to call callback and dialog system
            CallCallback(false, backupRequestData, backupKeepRequest);
        }

        protected virtual void CallCallback(bool show, DialogRequestData requestData, bool keepRequest = false)
        {
            if (show)
            {
                requestData.onShow?.Invoke(requestData);
                requestData.dialogSystem.OnRequestShow(requestData);
            }
            else
            {
                requestData.onHide?.Invoke(requestData, keepRequest);
                requestData.dialogSystem.OnRequestHide(requestData, keepRequest);
            }
        }

        public virtual bool Setup(DialogRequestData requestData)
        {
            var text = "";
            if (TextTable.TryGetText(requestData.descID, out text))
            {
                // Do nothing
            }
            else if (!allowTextIDOnly)
            {
                text = requestData.descID;
            }
            if (!string.IsNullOrEmpty(text))
            {
                CallCallback(true, requestData);
                // Activate game object
                gameObject.SetActive(true);
                SetupText(requestData);
                SetupSound(requestData);
                SetupButton(requestData);
                // Initialize state
                state = State.Init;
                SetupVisual();
            }
            else
            {
                CallCallback(true, requestData);
                // Just close it
                CallCallback(false, requestData);
            }

            return true;
        }

        public virtual bool IsHiding(DialogRequestData requestData)
        {
            if (requestData != null && requestData == hidingRequestData)
                return true;
            return false;
        }

        public virtual void Hide(DialogRequestData requestData, bool keepRequest = false)
        {
            UnityEngine.Assertions.Assert.IsNull(hidingRequestData, $"There should not be exist any hiding request data");
            hidingRequestData = requestData;
            shouldKeepRequest = keepRequest;

            OnHide(requestData);
        }

        public virtual void OnStateEnter(Animator _animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (showDebugInfo)
                Debug.Log($"OnStateEnter state = {CurrentStateName()}");
            if (stateInfo.IsName("Closed"))
            {
                OnClosed();
            }
            else if (stateInfo.IsName("Opened"))
            {
                OnOpened();
            }
        }

        #endregion

        #region Private & Protected

        protected enum State
        {
            Invalid = -1,
            Init,
            Opened,
            QueueHide,
            Closed
        }

        protected DialogRequestData hidingRequestData = null;
        protected bool shouldKeepRequest = false;
        protected State state = State.Invalid;
        protected Coroutine buttonSetupCoroutine;
        protected Coroutine soundSetupCoroutine;

        protected virtual void SetupVisual()
        {
            if (animator)
            {
                animator.SetBool("isShow", true);
            }
            else
            {
                if (canvasGroup)
                {
                    canvasGroup.alpha = 1;
                }
            }
        }

        protected virtual void SetupText(DialogRequestData requestData)
        {
            // Title
            if (TextTable.TryGetText(requestData.titleID, out var text))
            {
                titleText.gameObject.SetActive(true);
                titleText.SetTextID(requestData.titleID);
            }
            else
            {
                titleText.gameObject.SetActive(false);
            }

            descriptionText?.SetTextID(requestData.descID);
        }

        protected virtual void SetupSound(DialogRequestData requestData)
        {
            // Sound
            if (!string.IsNullOrEmpty(requestData.soundEventID))
            {
                requestData.soundLength = Mathf.Max(requestData.soundLength, SoundSystem.GetEventLength(requestData.soundEventID));
            }
            if (soundSetupCoroutine != null)
            {
                StopCoroutine(soundSetupCoroutine);
            }
            soundSetupCoroutine = StartCoroutine(SetupSoundCoroutine(requestData));
        }

        protected virtual IEnumerator SetupSoundCoroutine(DialogRequestData requestData)
        {
            yield return new WaitForSeconds(paddingTime.x);
            if (requestData.soundLength > 0)
            {
                SoundSystem.Play(requestData.soundEventID);
                yield return new WaitForSeconds(requestData.soundLength);
            }
            else
            {
                if (TextTable.TryGetText(requestData.descID, out var desc))
                {
                    yield return new WaitForSeconds(desc.Length * soundLengthPerText);
                }
            }
            yield return new WaitForSeconds(paddingTime.y);
            requestData.onSoundEnd?.Invoke(requestData);
            if (requestData.autoClose)
            {
                CloseDialog();
            }
            soundSetupCoroutine = null;
        }

        protected virtual void SetupButton(DialogRequestData requestData)
        {
            // Button setup with delay
            if (buttonSetupCoroutine != null)
            {
                StopCoroutine(buttonSetupCoroutine);
            }
            ActivateButton(requestData, false);
            buttonSetupCoroutine = StartCoroutine(SetupButtonCoroutine(requestData, requestData.soundLength));
        }

        protected virtual void OnButtonClick(DialogRequestData requestData, int buttonIndex)
        {
            requestData.buttonSetup[buttonIndex].onButton?.Invoke();
            CloseDialog();
        }

        protected virtual void ActivateButton(DialogRequestData requestData, bool activate)
        {
            for (int i = 0; i < buttonDatas.Count; i++)
            {
                var buttonData = buttonDatas[i];
                if (buttonData == null || buttonData.button == null)
                    continue;
                var button = buttonData.button;
                if (activate)
                {
                    var currButtonSetup = (requestData.buttonSetup != null && i < requestData.buttonSetup.Length) ? requestData.buttonSetup[i] : null;

                    if (currButtonSetup != null &&
                        (currButtonSetup.onButton != null || !string.IsNullOrEmpty(currButtonSetup.buttonText)))
                    {
                        if (buttonData.buttonText != null)
                            buttonData.buttonText.SetTextID(currButtonSetup.buttonText);
                        button.gameObject.SetActive(true);

                        // Setup button click event listener
                        var currRequestData = requestData;
                        var currButtonIdx = i;
                        button.onClick.AddListener(
                            () =>
                            {
                                OnButtonClick(currRequestData, currButtonIdx);
                            });
                    }
                    else
                    {
                        button.gameObject.SetActive(false);
                    }
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        protected virtual IEnumerator SetupButtonCoroutine(DialogRequestData requestData, float delayLength)
        {
            if (requestData.autoClose && delayLength <= 0)
            {
                delayLength = 1 + paddingTime.x;
            }
            if (!requestData.delayButton)
            {
                ActivateButton(requestData, true);
            }
            yield return new WaitForSeconds(delayLength);

            if (requestData.delayButton)
            {
                ActivateButton(requestData, true);
            }
            buttonSetupCoroutine = null;
        }

        protected virtual void CloseDialog()
        {
            // Call Next to check if there is any same type dialog in queue
            DialogUI.Next();
        }

        protected virtual void OnClosed()
        {
            state = State.Closed;

            if (hidingRequestData == null)
                return;

            CloseButton(hidingRequestData);
            CloseSound(hidingRequestData);

            gameObject.SetActive(false);

            // Backup
            var backupRequestData = hidingRequestData;
            var backupKeepRequest = shouldKeepRequest;

            // Clear first
            hidingRequestData = null;
            shouldKeepRequest = false;

            // Call dialog system
            backupRequestData.dialogSystem.OnRequestHide(backupRequestData, backupKeepRequest);
            
            // Use backup data to invoke
            backupRequestData.onHide?.Invoke(backupRequestData, backupKeepRequest);
        }

        protected virtual void CloseButton(DialogRequestData requestData)
        {
            if (buttonSetupCoroutine != null)
            {
                StopCoroutine(buttonSetupCoroutine);
            }
        }

        protected virtual void CloseSound(DialogRequestData requestData)
        {
            if (!string.IsNullOrEmpty(requestData.soundEventID))
            {
                if (soundSetupCoroutine != null)
                {
                    StopCoroutine(soundSetupCoroutine);
                }
                SoundSystem.Stop(requestData.soundEventID, .1f);
            }
        }

        protected virtual void OnHide(DialogRequestData requestData)
        {
            // Clear listener to prevent exception
            foreach (var b in buttonDatas)
            {
                b.button.onClick.RemoveAllListeners();
            }

            if (animator)
            {
                // Animation
                if (showDebugInfo)
                    Debug.Log($"OnHide state = {state} stateInfo = {CurrentStateName()} IsTransition = {animator.IsInTransition(0)}");

                if (state == State.Closed)
                    state = State.QueueHide;
                else
                    animator.SetBool("isShow", false);
            }
            else
            {
                // No animation
                if (canvasGroup)
                {
                    canvasGroup.alpha = 0;
                }
                OnClosed();
            }
        }

        protected virtual void OnOpened()
        {
            if (state == State.QueueHide)
            {
                if (animator)
                    animator.SetBool("isShow", false);
            }
            else
                state = State.Opened;
        }

        protected virtual string CurrentStateName()
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Opened"))
                return ("Opened");
            if (stateInfo.IsName("Closed"))
                return ("Closed");
            if (stateInfo.IsName("Show"))
                return ("Show");
            if (stateInfo.IsName("Hide"))
                return ("Hide");
            if (stateInfo.IsName("Init"))
                return ("Init");
            return stateInfo.fullPathHash.ToString();
        }

        #endregion
    }
}

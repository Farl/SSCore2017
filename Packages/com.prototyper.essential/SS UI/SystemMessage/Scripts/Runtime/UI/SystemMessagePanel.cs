using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace SS
{
    public class SystemMessagePanel : MonoBehaviour, IAnimatorStateEnter
    {
        #region Inspector
        [SerializeField]
        private bool showDebugInfo = false;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private TextTableMapping titleText;
        [SerializeField]
        private TextTableMapping descriptionText;
        [SerializeField]
        private List<ButtonData> buttonDatas = new List<ButtonData>();
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

        public virtual bool Setup(SystemMessageUI.RequestData requestData)
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

            for (int i = 0; i < buttonDatas.Count; i++)
            {
                var currButtonSetup = i < requestData.buttonSetup.Length ? requestData.buttonSetup[i]: null;

                if (currButtonSetup != null &&
                    (currButtonSetup.onButton != null || !string.IsNullOrEmpty(currButtonSetup.buttonText)))
                {
                    buttonDatas[i].buttonText?.SetTextID(currButtonSetup.buttonText);
                    buttonDatas[i].button.gameObject.SetActive(true);

                    // Setup button click event listener
                    var currRequestData = requestData;
                    var currButtonIdx = i;
                    buttonDatas[i].button.onClick.AddListener(
                        () => {
                            Hide(currRequestData);
                            currRequestData.buttonSetup[currButtonIdx].onButton?.Invoke();
                        });
                }
                else
                {
                    buttonDatas[i].button.gameObject.SetActive(false);
                }
            }

            // Show
            requestData.onShow?.Invoke(requestData);

            gameObject.SetActive(true);
            state = State.Init;
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
            return true;
        }

        public virtual bool IsHiding(SystemMessageUI.RequestData requestData)
        {
            if (requestData != null && requestData == hidingRequestData)
                return true;
            return false;
        }

        public virtual void Hide(SystemMessageUI.RequestData requestData, bool keepRequest = false)
        {
            UnityEngine.Assertions.Assert.IsNull(hidingRequestData, $"There should not be exist any hiding request data");
            hidingRequestData = requestData;
            shouldKeepRequest = keepRequest;

            // clear listener
            foreach (var b in buttonDatas)
            {
                b.button.onClick.RemoveAllListeners();
            }

            if (animator)
            {
                if (showDebugInfo)
                    Debug.Log($"OnHide state = {state} stateInfo = {CurrentStateName()} IsTransition = {animator.IsInTransition(0)}");

                if (state == State.Closed)
                    state = State.QueueHide;
                else
                    animator.SetBool("isShow", false);
            }
            else
            {
                if (canvasGroup)
                {
                    canvasGroup.alpha = 0;
                }
                OnClosed();
            }
        }

        public void OnStateEnter(Animator _animator, AnimatorStateInfo stateInfo, int layerIndex)
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

        private enum State
        {
            Invalid = -1,
            Init,
            Opened,
            QueueHide,
            Closed
        }

        private SystemMessageUI.RequestData hidingRequestData = null;
        private bool shouldKeepRequest = false;
        private State state = State.Invalid;

        private void OnClosed()
        {
            state = State.Closed;

            if (hidingRequestData == null)
                return;

            gameObject.SetActive(false);

            // Backup
            var backupRequestData = hidingRequestData;
            var backupKeepRequest = shouldKeepRequest;

            // Clear first
            hidingRequestData = null;
            shouldKeepRequest = false;

            // Use backup data to invoke
            backupRequestData.onHide?.Invoke(backupRequestData, backupKeepRequest);
        }

        private void OnOpened()
        {
            if (state == State.QueueHide)
            {
                if (animator)
                    animator.SetBool("isShow", false);
            }
            else
                state = State.Opened;
        }

        protected string CurrentStateName()
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

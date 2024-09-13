using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SS
{
    public class SystemMessageUI : UIEntity
    {
        [SerializeField]
        private bool showDebugInfo = false;

        private static int currHandle = -1;

        public enum Priority
        {
            Normal = 0,
            Low = -1,
            High = 1,
        }

        public class RequestData
        {
            public int priority = 0;
            public int handle = -1;
            public string typeName;
            public string titleID;
            public string descID;
            public ButtonSetup[] buttonSetup;
            public Action<RequestData> onShow;
            public Action<RequestData, bool> onHide;
            public SystemMessagePanel currPanel;
            public object[] additionalData;
        }

        public class ButtonSetup
        {
            public string buttonText;
            public Action onButton;
        }

        private static List<RequestData> requestDatas = new List<RequestData>();
        private static RequestData currRequestData = null;

        private static SystemMessageUI Instance;

        [SerializeField]
        private List<SystemMessagePanel> panels = new List<SystemMessagePanel>();

        private static void OnRequestShow(RequestData rd)
        {

        }

        private static void OnRequestHide(RequestData rd, bool keepRequest)
        {
            if (Instance != null && Instance.showDebugInfo)
            {
                if (keepRequest)
                    Debug.Log($"[{Time.realtimeSinceStartup}] {rd.handle} Keep {rd.titleID} {rd.descID}");
                else
                    Debug.Log($"[{Time.realtimeSinceStartup}] {rd.handle} Hide {rd.titleID} {rd.descID}");
            }
            if (currRequestData != rd)
            {
                Debug.LogError($"Unmatch request {rd.handle} != {currRequestData.handle}");
                return;
            }
            if (keepRequest)
            {
                // Enqueue and Sort
                requestDatas.Add(currRequestData);
                requestDatas.Sort((x, y) => y.priority.CompareTo(x.priority));
            }

            // Finish and Next
            currRequestData = null;
            TryDequeue();
        }

        public static int Request(RequestData requestData)
        {
            if (requestData == null)
                return -1;
            // Setup handler ID
            requestData.handle = ++currHandle;
            // Show debug info
            if (Instance != null && Instance.showDebugInfo)
            {
                Debug.Log($"[{Time.realtimeSinceStartup}] {requestData.handle} Request {requestData.titleID} {requestData.descID}");
            }
            // Insert system callback
            requestData.onShow += OnRequestShow;
            requestData.onHide += OnRequestHide;
            // Enqueue and Sort
            requestDatas.Add(requestData);
            requestDatas.Sort((x, y) => y.priority.CompareTo(x.priority));
            // Dequeue
            TryDequeue();
            return requestData.handle;
        }

        public static int Request(string typeName, string titleID, string descID = null,
            params ButtonSetup[] buttonSetup)
        {
            var requestData = new RequestData()
            {
                typeName = typeName,
                titleID = titleID,
                descID = descID,
                priority = 0,
                additionalData = null,
                buttonSetup = buttonSetup,
            };
            return Request(requestData);
        }

        public static int Request(string typeName, string titleID, string descID = null, int priority = 0,
            params ButtonSetup[] buttonSetup)
        {
            var requestData = new RequestData()
            {
                typeName = typeName,
                titleID = titleID,
                descID = descID,
                priority = priority,
                additionalData = null,
                buttonSetup = buttonSetup,
            };
            return Request(requestData);
        }

        private static bool TryDequeue()
        {
            var nextRequestData = (requestDatas.Count > 0) ? requestDatas[0] : null;
            if (currRequestData != null)
            {
                if (nextRequestData != null && nextRequestData.priority > currRequestData.priority)
                {
                    if (currRequestData.currPanel && !currRequestData.currPanel.IsHiding(currRequestData))
                    {
                        // Close current request temporary (Don't remove current request)
                        currRequestData.currPanel?.Hide(currRequestData, keepRequest: true);
                    }
                    else
                    {
                        // Just wait
                    }
                }
                return false;
            }

            if (requestDatas.Count > 0)
            {
                currRequestData = requestDatas[0];
                requestDatas.RemoveAt(0);
                if (Instance)
                {
                    return Instance.Setup(currRequestData);
                }
            }
            return false;
        }

        public static void Close(int handle)
        {
            if (handle < 0)
                return;
            if (Instance != null && Instance.showDebugInfo)
            {
                Debug.Log($"[{Time.realtimeSinceStartup}] {handle} Call close");
            }
            if (currRequestData != null && currRequestData.handle == handle)
            {
                if (currRequestData.currPanel && !currRequestData.currPanel.IsHiding(currRequestData))
                {
                    currRequestData.currPanel.Hide(currRequestData);
                }
                else
                {
                    // Just wait
                }
            }
            else
            {
                requestDatas.RemoveAll((x) => x.handle == handle);
            }
        }

        public static bool Contains(int handle)
        {
            if (handle < 0)
                return false;

            if (currRequestData != null && currRequestData.handle == handle)
            {
                return true;
            }

            var reqeustData = requestDatas.Find((x) => x.handle == handle);
            if (reqeustData != null)
            {
                return true;
            }
            return false;
        }

        public static bool DialogExist
        {
            get
            {
                return currRequestData != null || requestDatas.Count > 0;
            }
        }

        #region Instance
        private bool Setup(RequestData requestData)
        {
            requestData.currPanel = panels.Find((x) => string.IsNullOrEmpty(requestData.typeName) || x.name == requestData.typeName);
            if (requestData.currPanel == null)
                return false;

            return requestData.currPanel.Setup(requestData);
        }
        #endregion

        #region override

        public override void OnRootInitialize()
        {
            base.OnRootInitialize();
            if (Instance == null)
            {
                Instance = this;

                foreach (var panel in panels)
                {
                    panel?.Initialize();
                }
            }
        }

        protected override void OnEntityAwake()
        {
            base.OnEntityAwake();
        }

        protected override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            if (Instance == this)
                Instance = null;
        }

        #endregion

        #region Test

        public void Test(int count)
        {
            showDebugInfo = true;
            StartCoroutine(Test_Coroutine(count));
        }

        [ContextMenu("Test 0")]
        public void Test0()
        {
            Test(0);
        }

        [ContextMenu("Test 1")]
        public void Test1()
        {
            Test(1);
        }

        [ContextMenu("Test 2")]
        public void Test2()
        {
            Test(2);
        }

        [ContextMenu("Test 3")]
        public void Test3()
        {
            Test(3);
        }

        private IEnumerator Test_Coroutine(int count)
        {
            Debug.Log(count);
            var msgHandle = SystemMessageUI.Request(null, "ConsumeStepReadingTitle", "ConsumeStepReadingDesc", priority: -1);

            for (int i = 0; i < count; i++)
                yield return null;

            SystemMessageUI.Close(msgHandle);
        }
        #endregion
    }
}

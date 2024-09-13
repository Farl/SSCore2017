using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS
{
    public interface IDialogSystem
    {
        bool ShowDebugInfo { get; }
        bool Setup(DialogRequestData requestData);

        void OnRequestShow(DialogRequestData requestData);
        void OnRequestHide(DialogRequestData requestData, bool keepRequest);
    }

    public class DialogUIBase<T> : UIEntity, IDialogSystem where T : DialogRequestData
    {
        #region Inspector
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private List<DialogPanel> panels = new List<DialogPanel>();
        #endregion 

        #region Static
        private static IDialogSystem Instance;
        private static int currHandle = -1;
        private static List<T> requestDatas = new List<T>();
        private static T currRequestData = null;

        public static int Request(DialogRequestData requestData)
        {
            T rd = (T)requestData;

            rd.handle = ++currHandle;
            //rd.onShow += OnRequestShow;
            //rd.onHide += OnRequestHide;
            rd.dialogSystem = Instance;

            // Enqueue and Sort
            requestDatas.Add(rd);
            requestDatas.Sort((x, y) => y.priority.CompareTo(x.priority));

            // Dequeue
            TryDequeue();

            return rd.handle;
        }

        public static int Request(string typeName, string titleID, string descID = null,
            params DialogButtonSetup[] buttonSetup)
        {
            return Request(typeName, titleID, descID, priority: 0, null, buttonSetup);
        }

        public static int Request(string typeName, string titleID, string descID = null, int priority = 0,
            params DialogButtonSetup[] buttonSetup)
        {
            return Request(typeName, titleID, descID, priority, null, buttonSetup);
        }

        private static int Request(string typeName, string titleID, string descID = null,
            int priority = 0, object[] additionalData = null,
            params DialogButtonSetup[] buttonSetup)
        {
            var rd = new DialogRequestData()
            {
                typeName = typeName,
                titleID = titleID,
                descID = descID,
                priority = priority,
                buttonSetup = buttonSetup,
            };
            if (Instance != null && Instance.ShowDebugInfo)
            {
                Debug.Log($"[{Time.realtimeSinceStartup}] {rd.handle} Request {titleID} {descID}");
            }

            rd.additionalData = additionalData;

            return Request(rd);
        }

        private static void TryDequeue()
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
                return;
            }

            if (requestDatas.Count > 0)
            {
                currRequestData = requestDatas[0];
                requestDatas.RemoveAt(0);
                if (Instance != null)
                {
                    if (Instance.Setup(currRequestData))
                    {
                        return;
                    }
                }
            }
        }

        public static void Next()
        {
            bool renew = false;
            T nextRequestData = (requestDatas.Count > 0) ? requestDatas[0] : null;
            if (nextRequestData != null)
            {
                if (currRequestData != null && nextRequestData != null && currRequestData.typeName == nextRequestData.typeName)
                {
                    // Renew panel by next data
                    currRequestData.currPanel.Renew(currRequestData);
                    renew = true;
                }
            }
            if (renew == false && currRequestData != null)
            {
                Close(currRequestData.handle);
            }
        }

        public static void Close(int handle)
        {
            if (handle < 0)
                return;
            if (Instance != null && Instance.ShowDebugInfo)
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
        #endregion

        #region Public
        public bool ShowDebugInfo => showDebugInfo;

        public bool Setup(DialogRequestData requestData)
        {
            requestData.currPanel = panels.Find((x) => string.IsNullOrEmpty(requestData.typeName) || x.name == requestData.typeName);
            if (requestData.currPanel == null)
                return false;

            return requestData.currPanel.Setup(requestData);
        }

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
        #endregion

        #region Private / Protected

        public virtual void OnRequestShow(DialogRequestData requestData)
        {
            T rd = (T)requestData;
        }

        public virtual void OnRequestHide(DialogRequestData requestData, bool keepRequest)
        {
            T rd = (T)requestData;
            if (Instance != null && Instance.ShowDebugInfo)
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

        protected override void OnEntityAwake()
        {
            base.OnEntityAwake();
        }

        protected override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            if (Instance == (IDialogSystem)this)
            {
                Instance = null;
                currRequestData = null;
                requestDatas.Clear();
            }
        }

        #endregion
    }
}

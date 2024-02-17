using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

namespace SS
{
    public class DebugMenu : UIEntity
    {
        public Transform templateRoot;
        public Transform pageRoot;
        public Transform templatePage;

        public Toggle pageToggleTemplate;
        public DebugMenuLog logTemplate;
        public float timeScale = 0.0f;

        private bool input;
        private ElementPage currPage;

#if ENABLE_INPUT_SYSTEM
        private UnityEngine.InputSystem.Controls.KeyControl compKey;
        private UnityEngine.InputSystem.Controls.KeyControl key;
#endif

        private void InitKeyboard()
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                compKey = keyboard.FindKeyOnCurrentKeyboardLayout("Right Shift");
                key = keyboard.FindKeyOnCurrentKeyboardLayout("Backspace");
            }
#endif
        }

        [Serializable]
        public class ElementPage
        {
            [NonSerialized]
            public bool isInit = false;
            public string name;
            public Transform rootTransform;
            public Transform contentRootTransform;
        }

        public List<ElementPage> pageList = new List<ElementPage>();


        private enum ElementDataType
        {
            Toggle,
            Button,
            Dropdown,
        }

        private class ElementData
        {
            public bool isInit = false;
            public ElementDataType type = ElementDataType.Toggle;
            public string page = defaultPageName;
            public string label;
            public Action<object, UnityEngine.Object> onValueChanged;
            public Action<UnityEngine.Object> onShow;
            public object value;
            public List<string> stringList;
            public UnityEngine.Object obj;
            public DebugMenuElement element;
        }

        public static Action<bool> onMenuToggle;
        public static float TimeScale
        {
            get
            {
                return _timeScale;
            }
        }
        private static float _timeScale = 0.0f;
        private const string defaultPageName = "Default";
        private Toggle currPageToggle = null;
        private static Dictionary<string, ElementData> dataSet = new Dictionary<string, ElementData>();
        private static Dictionary<string, ElementPage> pages = new Dictionary<string, ElementPage>();

        public static void Remove(string label)
        {
            if (dataSet.TryGetValue(label, out var elementData))
            {
                // Destroy
                if (elementData.element)
                {
                    Destroy(elementData.element.gameObject);
                }
                dataSet.Remove(label);
            }
        }

        private static bool CheckAdd(string page, string label)
        {
            if (dataSet.ContainsKey(label))
                return false;
            if (!string.IsNullOrEmpty(page))
                AddPage(page);
            else
                AddPage(defaultPageName);
            return true;
        }

        public static bool AddDropdown(string page, string label, Action<int, UnityEngine.Object> onChanged, List<string> stringList, int defaultValue, Action<UnityEngine.Object> onShow = null)
        {
            if (!CheckAdd(page, label))
                return false;
            var ed = new ElementData()
            {
                type = ElementDataType.Dropdown,
                label = label,
                page = page,
                value = defaultValue,
                stringList = stringList,
            };
            ed.onValueChanged += (idx, obj) => { onChanged((int)idx, obj); };
            ed.onShow += onShow;
            dataSet.Add(label, ed);
            return true;
        }

        public static bool AddToggle(string page, string label, Action<bool, UnityEngine.Object> onChanged, bool defaultValue, Action<UnityEngine.Object> onShow = null)
        {
            if (!CheckAdd(page, label))
                return false;
            var ed = new ElementData()
            {
                type = ElementDataType.Toggle,
                label = label,
                page = page,
                value = defaultValue
            };
            ed.onValueChanged += (b, obj) => { onChanged((bool)b, obj); };
            ed.onShow += onShow;
            dataSet.Add(label, ed);
            return true;
        }

        public static bool AddToggle(string page, string label, Func<bool> getter, Action<bool> setter)
        {
            if (!CheckAdd(page, label))
                return false;
            var ed = new ElementData()
            {
                type = ElementDataType.Toggle,
                label = label,
                page = page,
                value = getter?.Invoke()
            };
            ed.onValueChanged += (b, obj) => { setter?.Invoke((bool)b); };
            ed.onShow += (obj) => {
                var toggle = obj as Toggle;
                if (toggle != null) toggle.isOn = (bool)(getter?.Invoke());
            };
            dataSet.Add(label, ed);
            return true;
        }

        public static bool AddButton(string page, string label, Action<UnityEngine.Object> onClick, Action<UnityEngine.Object> onShow = null)
        {
            if (!CheckAdd(page, label))
                return false;
            var ed = new ElementData()
            {
                type = ElementDataType.Button,
                label = label,
                page = page,
            };
            ed.onValueChanged += (o, obj) => { onClick?.Invoke(obj); };
            ed.onShow += onShow;

            dataSet.Add(label, ed);
            return true;
        }

        public static void AddPage(string pageName)
        {
            if (pages.ContainsKey(pageName))
                return;

            var ep = new ElementPage()
            {
                isInit = false,
                name = pageName
            };

            pages.Add(ep.name, ep);
        }

        private Transform GetPageRoot(string pageName)
        {
            if (!string.IsNullOrEmpty(pageName) && pages.TryGetValue(pageName, out var p))
            {
                return p.rootTransform;
            }
            return GetPageRoot(defaultPageName);
        }

        private Transform GetPageContentRoot(string pageName)
        {
            if (!string.IsNullOrEmpty(pageName) && pages.TryGetValue(pageName, out var p))
            {
                return p.contentRootTransform? p.contentRootTransform: p.rootTransform;
            }
            return GetPageContentRoot(defaultPageName);
        }

        private void SpawnElement(ElementData ed)
        {
            if (templateRoot)
            {
                var template = templateRoot.Find(ed.type.ToString());
                if (template != null)
                {
                    var elementRoot = GetPageContentRoot(ed.page);

                    var newEleGo = GameObject.Instantiate(template, elementRoot);
                    ed.element = newEleGo.GetComponent<DebugMenuElement>();
                    if (ed.element)
                    {
                        switch (ed.type)
                        {
                            case ElementDataType.Toggle:
                                ed.obj = ed.element.InitToggle(ed.label, ed.onValueChanged, (bool)ed.value);
                                break;
                            case ElementDataType.Button:
                                ed.obj = ed.element.InitButton(ed.label, ed.onValueChanged);
                                break;
                            case ElementDataType.Dropdown:
                                ed.obj = ed.element.InitDropdown(ed.label, ed.onValueChanged, ed.stringList, (int)ed.value);
                                break;
                        }
                    }
                }
            }
        }

        private void SwitchPage(string pageName)
        {
            if (currPage != null && currPage.rootTransform)
            {
                currPage.rootTransform.gameObject.SetActive(false);
            }
            if (pages.TryGetValue(pageName, out var p))
            {
                if (p.rootTransform)
                {
                    p.rootTransform.gameObject.SetActive(true);
                }
                currPage = p;

                // OnShow event
                foreach (var kvp in dataSet)
                {
                    var ed = kvp.Value;
                    if (GetPageRoot(ed.page) == currPage.rootTransform)
                    {
                        ed.onShow?.Invoke(ed.obj);
                    }
                }
            }
        }

        protected override void OnShow(params object[] parameters)
        {
            base.OnShow(parameters);
            _timeScale = timeScale;
            onMenuToggle?.Invoke(true);

            // Init pages step 1
            foreach (var p in pageList)
            {
                if (!p.isInit)
                {
                    if (!string.IsNullOrEmpty(p.name))
                    {
                        if (pages.ContainsKey(p.name))
                        {
                            pages[p.name].rootTransform = p.rootTransform;
                            pages[p.name].contentRootTransform = p.contentRootTransform;
                        }
                        else
                        {
                            var ep = new ElementPage()
                            {
                                name = p.name,
                                rootTransform = p.rootTransform,
                                contentRootTransform = p.contentRootTransform
                            };
                            pages.Add(p.name, ep);
                        }
                    }
                    p.isInit = true;
                }
            }

            // Init pages step 2
            foreach (var kvp in pages)
            {
                var p = kvp.Value;
                var pName = p.name;
                if (!p.isInit)
                {
                    // New page toggle
                    pageToggleTemplate.gameObject.SetActive(false);
                    var go = GameObject.Instantiate(pageToggleTemplate.gameObject, pageToggleTemplate.transform.parent);
                    go.name = pName;
                    go.SetActive(true);
                    var text = go.GetComponentInChildren<TMP_Text>(false);
                    if (text != null)
                    {
                        text.text = pName;
                    }
                    var toggle = go.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.isOn = false;
                        // Toggle callback to switch page
                        toggle.onValueChanged.AddListener((b) =>
                        {
                            if (b)
                            {
                                currPageToggle = toggle;
                                SwitchPage(pName);
                                //Debug.Log(pName);
                            }
                        });
                    }

                    // New root
                    if (p.rootTransform == null && templatePage)
                    {
                        templatePage.gameObject.SetActive(false);
                        var pageGo = GameObject.Instantiate(templatePage.gameObject, templatePage.parent);
                        pageGo.SetActive(false);
                        pageGo.name = pName;
                        p.rootTransform = pageGo.transform;
                    }

                    //
                    if (toggle && currPageToggle == null && pName == defaultPageName)
                    {
                        currPageToggle = toggle;
                    }

                    p.isInit = true;
                }

                if (pName != defaultPageName && p.rootTransform)
                {
                    p.rootTransform.gameObject.SetActive(false);
                }
            }

            if (currPageToggle)
            {
                currPageToggle.isOn = false;
                currPageToggle.isOn = true;
            }

            // Init elements
            foreach (var kvp in dataSet)
            {
                var ed = kvp.Value;
                if (!ed.isInit)
                {
                    SpawnElement(ed);
                    ed.isInit = true;
                }
                //ed.onShow?.Invoke(ed.obj);    // Move to switch page
            }
        }
        protected override void OnHide(params object[] parameters)
        {
            base.OnHide(parameters);
            onMenuToggle?.Invoke(false);
        }

        public override void OnRootInitialize()
        {
            base.OnRootInitialize();

#if FINAL
            Destroy(gameObject);
#else
            InitKeyboard();

            if (templateRoot)
                templateRoot.gameObject.SetActive(false);

            // Language select
            var languages = TextTable.GetSupportLanguage();
            var languageStrings = new List<string>();
            foreach (var l in languages)
            {
                languageStrings.Add(l.ToString());
            }
            DebugMenu.AddDropdown(page: null, label: "Language", defaultValue: 0,
                stringList: languageStrings,
                onChanged: (idx, obj) =>
                {
                    var dropdown = obj as TMP_Dropdown;
                    if (dropdown)
                    {
                        if (SystemLanguage.TryParse<SystemLanguage>(dropdown.options[idx].text, out var l))
                        {
                            TextTable.SetCurrentLanguage(l);
                        }
                    }
                },
                onShow: (obj) =>
                {
                    var dropdown = obj as TMP_Dropdown;
                    var l = TextTable.GetCurrentLanguage();
                    if (dropdown)
                    {
                        var idx = dropdown.options.FindIndex(d => d.text == l.ToString());
                        if (idx > 0)
                            dropdown.value = idx;
                        else
                        {
                            var newOpts = new List<string>();
                            newOpts.Add(l.ToString());
                            dropdown.AddOptions(newOpts);
                            dropdown.value = dropdown.options.Count - 1;
                        }
                    }
                }
            );

            // Graphic device type
            DebugMenu.AddButton(page: "Advance", label: "Graphic Device Type", onClick: null,
                onShow: (obj) =>
                {
                    var comp = obj as Component;
                    if (comp)
                    {
                        var text = comp.GetComponentInChildren<TMP_Text>();
                        if (text)
                            text.SetText($"{SystemInfo.graphicsDeviceType.ToString()}");
                    }
                });

            // Quality settings level
            DebugMenu.AddDropdown(page: "Advance", label: "Quality", defaultValue: QualitySettings.GetQualityLevel(),
                stringList: new List<string>(new string[] { "0", "1", "2" }),
                onChanged: (idx, obj) =>
                {
                    QualitySettings.SetQualityLevel(idx, false);
                },
                onShow: (obj) =>
                {
                    var dropdown = obj as TMP_Dropdown;
                    if (dropdown)
                    {
                        dropdown.value = QualitySettings.GetQualityLevel();
                    }
                }
            );

            //
            InitLoggerMenu();

#if ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Enable();
#endif

#endif

        }

        protected override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            if (IsShow)
                onMenuToggle?.Invoke(false);
            foreach (var kvp in dataSet)
            {
                kvp.Value.isInit = false;
            }
            foreach (var kvp in pages)
            {
                kvp.Value.isInit = false;
            }
        }

        public override void OnUpdate()
        {
            bool prevInput = input;
            input = false;
#if ENABLE_INPUT_SYSTEM

            if (key != null)
            {
                input = key.IsPressed() && compKey.IsPressed();
            }

            if (EnhancedTouchSupport.enabled)
            {
                var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
                var touchCount = touches.Count;
                if (touchCount >= 3)
                    input = true;
            }
#else
#endif
            if (prevInput != input && input)
            {
                if (!IsShow)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

#region Logger
        private bool isLog = false;
        private bool isLogWarning = false;
        private bool isLogError = true;

        private void InitLoggerMenu()
        {
            if (logTemplate)
                logTemplate.gameObject.SetActive(false);

            // Logger
            Application.logMessageReceived += (string condition, string stackTrace, LogType typ) =>
            {
                if (logTemplate && ((isLog && typ == LogType.Log) ||
                (isLogWarning && (typ == LogType.Warning || typ == LogType.Exception)) ||
                (isLogError && (typ == LogType.Error || typ == LogType.Assert))))
                {
                    logTemplate.gameObject.SetActive(false);
                    var go = GameObject.Instantiate(logTemplate.gameObject, logTemplate.transform.parent);
                    if (go)
                    {
                        go.SetActive(true);
                        var logComp = go.GetComponent<DebugMenuLog>();
                        if (logComp)
                        {
                            switch (typ)
                            {
                                case LogType.Error:
                                    condition = $"<color=red>{condition}</color>";
                                    break;
                                case LogType.Assert:
                                    condition = $"<color=red>{condition}</color>";
                                    break;
                                case LogType.Warning:
                                    condition = $"<color=orange>{condition}</color>";
                                    break;
                                case LogType.Log:
                                    break;
                                case LogType.Exception:
                                    condition = $"<color=orange>{condition}</color>";
                                    break;
                            }
                            logComp.log.text = condition;
                            logComp.callStack.text = stackTrace;
                            logComp.callStack.gameObject.SetActive(false);
                        }
                    }
                }
            };

            DebugMenu.AddToggle("Log", "Log", defaultValue: isLog,
                onChanged: (value, obj) =>
                {
                    isLog = value;
                },
                onShow: (obj) =>
                {
                }
            );

            DebugMenu.AddToggle("Log", "Log Warning", defaultValue: isLogWarning,
                onChanged: (value, obj) =>
                {
                    isLogWarning = value;
                },
                onShow: (obj) =>
                {
                }
            );

            DebugMenu.AddToggle("Log", "Log Error", defaultValue: isLogError,
                onChanged: (value, obj) =>
                {
                    isLogError = value;
                },
                onShow: (obj) =>
                {
                }
            );

            DebugMenu.AddButton("Log", "Clear Log",
                onClick: (obj) =>
                {
                    if (logTemplate)
                    {
                        var lt = logTemplate.transform;
                        foreach (Transform t in lt.parent)
                        {
                            if (t != lt)
                            {
                                Destroy(t.gameObject);
                            }
                            else
                            {
                                t.gameObject.SetActive(false);
                            }
                        }
                    }
                },
                onShow: (obj) =>
                {
                }
            );
        }
#endregion
    }
}

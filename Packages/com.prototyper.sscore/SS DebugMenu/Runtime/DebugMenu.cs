/**
 * Debug Menu
 * debug events
 * debug scene select
 */

using UnityEngine;
using System.Collections.Generic;
using System;

namespace SS
{
    [System.Serializable]
    public class DebugEventInfo
    {
        public string eventID;
        public bool paramBool;
        public DebugEventInfo(string _eventID, bool _paramBool)
        {
            eventID = _eventID;
            paramBool = _paramBool;
        }
    }

    public class DebugMenu : UIBaseSingleton<DebugMenu>
    {
        [SerializeField]
        KeyCode toggleKeyCode = KeyCode.Backspace;

        [SerializeField]
        int toggleTouchCount = 3;

        [SerializeField]
        private GameObject _buttonListButtonTemplate;
        private bool _buttonListIsInit { get; set; }

        [Serializable]
        public class ButtonList
        {
            public string name;
            public Transform buttonRoot;
            public GameObject buttonTemplate;

            private bool IsInit { get; set; }
            public void Init(GameObject template)
            {
                if (!IsInit)
                {
                    if (buttonTemplate == null)
                        buttonTemplate = template;
                    if (buttonTemplate)
                        buttonTemplate.SetActive(false);
                    IsInit = true;
                }
            }

            public void AddButtonListButton(Action onButtonClick, string displayText)
            {
                var go = Instantiate(buttonTemplate, buttonRoot);
                UnityEngine.UI.Button button = go.GetComponentInChildren<UnityEngine.UI.Button>();
                if (button)
                {
                    button.onClick.AddListener(() =>
                    {
                        onButtonClick();
                    });
                }
                UnityEngine.UI.Text text = go.GetComponentInChildren<UnityEngine.UI.Text>();
                if (text)
                {
                    text.text = displayText;
                }
                go.SetActive(true);
            }
        }

        [SerializeField]
        private List<ButtonList> buttonLists = new List<ButtonList>();

        bool debugMenuShow = false;

        bool waitZeroInput = false;

        public bool drawOnGUI = false;

        private UIBase panel = null;

        [Auto]
        public DebugEventInfo[] debugEvent;

        private void InitializeButtonList()
        {
            if (_buttonListIsInit)
                return;
            // Initalize button list
            if (_buttonListButtonTemplate)
                _buttonListButtonTemplate.SetActive(false);
            foreach (var bl in buttonLists)
            {
                bl.Init(_buttonListButtonTemplate);
            }
            _buttonListIsInit = true;
        }

        protected override void OnInit()
        {
            base.OnInit();

            panel = GetComponent<UIBase>();

            InitializeButtonList();

            // scene buttons
            var bl = GetButtonList("Scene");
            if (bl != null)
            {
                RuntimePlayerSettings.GetInstance((playerSettings) =>
                {
                    if (playerSettings != null)
                    {
                        foreach (string scenePath in playerSettings.scenes)
                        {
                            var id = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                            bl.AddButtonListButton(() =>
                            {
                                UnityEngine.SceneManagement.SceneManager.LoadScene(id);
                            },
                            id);
                        }
                    }
                });
            }

            World.RegisterOnGUI(DrawGUI, this);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        public ButtonList GetButtonList(string name)
        {
            if (!_buttonListIsInit)
            {
                Debug.LogError("DebugMenu is not intialize yet");
                return null;
            }
            var bl = buttonLists.Find(x => x.name == name);
            if (bl == null)
            {
                Debug.LogError($"Can't find {name} in button list");
            }

            return bl;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            World.UnregisterOnGUI(DrawGUI, this);
        }

        void Update()
        {
            // Update Input
            if (waitZeroInput)
            {
                if (Input.touchCount == 0)
                    waitZeroInput = false;
            }
            else
            {
                if (Input.GetKeyDown(toggleKeyCode) || Input.touchCount == toggleTouchCount)
                {
                    debugMenuShow = !debugMenuShow;
                    if (debugMenuShow)
                    {
                        panel.Open();
                    }
                    else
                    {
                        panel.Close();
                    }
                    waitZeroInput = true;
                }
            }
        }

        void DrawGUI()
        {
            if (!debugMenuShow || !drawOnGUI)
                return;
            int segW = 5;
            int segH = 5;

            float layoutW = Screen.width / (float)segW;
            float layoutH = Screen.height / (float)segH;

            GUILayoutOption[] layout = { GUILayout.Width(layoutW), GUILayout.Height(layoutH) };

            int id = 0;

            foreach (DebugEventInfo info in debugEvent)
            {
                if (id % segW == 0)
                {
                    GUILayout.BeginHorizontal(layout);
                }
                if (GUILayout.Button(info.eventID, layout))
                {
                    EventMessage em = new EventMessage(info.eventID, this, info.paramBool);
                    EventManager.Broadcast(em);
                }
                if (id % segW == segW - 1)
                {
                    GUILayout.EndHorizontal();
                }
                id++;
            }
        }
    }

}
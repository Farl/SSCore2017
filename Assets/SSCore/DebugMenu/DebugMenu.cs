/**
 * Debug Menu
 * debug events
 * debug scene select
 */

using UnityEngine;
using System.Collections.Generic;

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
        private GameObject _sceneButtonTemplate;
        private Transform _sceneButtonRoot;

        [SerializeField]
        private GameObject _stageButtonTemplate;
        private Transform _stageButtonRoot;

        bool debugMenuShow = false;

        bool waitZeroInput = false;

        public bool drawOnGUI = false;

        private UIBase panel = null;

        [Auto]
        public DebugEventInfo[] debugEvent;

        protected override void Awake()
        {
            base.Awake();

            panel = GetComponent<UIBase>();

            // scene buttons
            if (_sceneButtonTemplate)
            {
                _sceneButtonRoot = _sceneButtonTemplate.transform.parent;
                _sceneButtonTemplate.SetActive(false);
                foreach (string scenePath in RuntimePlayerSettings.Instance.scenes)
                {
                    var id = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                    GameObject go = Instantiate(_sceneButtonTemplate, _sceneButtonRoot);
                    UnityEngine.UI.Button button = go.GetComponentInChildren<UnityEngine.UI.Button>();
                    if (button)
                    {
                        button.onClick.AddListener(() =>
                        {
                            UnityEngine.SceneManagement.SceneManager.LoadScene(id);
                        });
                    }
                    UnityEngine.UI.Text text = go.GetComponentInChildren<UnityEngine.UI.Text>();
                    if (text)
                    {
                        text.text = id;
                    }
                    go.SetActive(true);
                }
            }

            World.RegisterOnGUI(DrawGUI, this);
        }

        // stage buttons
        public void CreateStageButtons(List<Stage> stages)
        {
            if (stages != null && _stageButtonTemplate)
            {
                _stageButtonRoot = _stageButtonTemplate.transform.parent;
                _stageButtonTemplate.SetActive(false);
                foreach (var stage in stages)
                {
                    var id = stage.name;
                    GameObject go = Instantiate(_stageButtonTemplate, _stageButtonRoot);
                    UnityEngine.UI.Button button = go.GetComponentInChildren<UnityEngine.UI.Button>();
                    if (button)
                    {
                        button.onClick.AddListener(() =>
                        {
                            StageSystem.SetNextStage(id);
                        });
                    }
                    UnityEngine.UI.Text text = go.GetComponentInChildren<UnityEngine.UI.Text>();
                    if (text)
                    {
                        text.text = id;
                    }
                    go.SetActive(true);
                }
            }
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
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.touchCount == 3)
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
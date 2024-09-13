using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SS
{
    public class FPSUI : UIEntity
    {
        [SerializeField]
        private float updateInterval = 0.5F;

        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        private static bool isShow { get; set; } = false;
        private static int targetFPS = 60;

        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval
        private string _format;
        private float _fps;

        protected override void OnEntityAwake()
        {
            base.OnEntityAwake();
            if (!_text)
                _text = GetComponentInChildren<TMP_Text>();

            if (!_canvasGroup)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup)
                _canvasGroup.alpha = isShow? 1: 0;

            DebugMenu.AddToggle(null, "Show FPS",
                onChanged: (b, obj) =>
                {
                    isShow = b;
                    if (_canvasGroup)
                    {
                        _canvasGroup.alpha = (isShow) ? 1 : 0;
                    }
                },
                onShow: (obj) =>
                {
                    var toggle = obj as Toggle;
                    if (toggle)
                    {
                        toggle.isOn = isShow;
                    }
                    if (_canvasGroup)
                    {
                        _canvasGroup.alpha = (isShow) ? 1 : 0;
                    }
                },
                defaultValue: isShow
            );

            // Target framerate
            Application.targetFrameRate = targetFPS;

            DebugMenu.AddToggle(page: null, label: "FPS 60", defaultValue: targetFPS == 60,
                onChanged: (b, obj) =>
                {
                    targetFPS = (b) ? 60 : 30;
                    Application.targetFrameRate = targetFPS;

                },
                onShow: (obj) =>
                {
                    Toggle toggle = obj as Toggle;
                    if (toggle)
                    {
                        toggle.isOn = targetFPS == 60;
                    }
                }
            );
        }

        protected override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            DebugMenu.Remove("FPS 60");
            DebugMenu.Remove("Show FPS");
        }

        private void Start()
        {
            timeleft = updateInterval;
        }

        void SetColor(Color color)
        {
            if (_text)
            {
                _text.color = color;
            }
        }

        void SetText(string text)
        {
            if (_text)
            {
                _text.text = text;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            timeleft -= Time.unscaledDeltaTime;
            accum += 1f / Time.unscaledDeltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                _fps = accum / frames;
                _format = $"{_fps:F2} FPS";
                SetText(_format);

                if (_fps < 60)
                    SetColor(Color.yellow);
                else if (_fps < 30)
                    SetColor(Color.red);
                else
                    SetColor(Color.green);

                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
    }
}

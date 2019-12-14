/**
 *  FPS
 **/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SS
{
    public class GUITextFPS : MonoBehaviour
    {

        public float updateInterval = 0.5F;

        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval
        private string _format;
        private float _fps;

        private Text _text;
        private GUIText _guiText;

        private void Awake()
        {
            _text = GetComponent<Text>();
            _guiText = GetComponent<GUIText>();
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
            if (_guiText)
            {
                _guiText.color = color;
            }
        }

        void SetText(string text)
        {
            if (_text)
            {
                _text.text = text;
            }
            if (_guiText)
            {
                _guiText.text = text;
            }
        }

        private void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                _fps = accum / frames;
                _format = System.String.Format("{0:F2} FPS", _fps);
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

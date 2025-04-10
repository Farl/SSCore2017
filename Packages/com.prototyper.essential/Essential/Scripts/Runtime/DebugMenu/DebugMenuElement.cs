using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace SS
{
    public class DebugMenuElement : MonoBehaviour
    {
        public TextTableMapping label;
        public TMP_Text valueText;

        public Toggle toggle;
        public Button button;
        public TMP_Dropdown dropdown;
        public Slider slider;

        private Action<object, UnityEngine.Object> _onValueChanged;
        private string[] _stringArray;
        private object _value = 0;
        private enum ValueType
        {
            StringArray,
            Enum,
            Float,
            Int,
        }
        
        public void OnValueChanged(string stringValue)
        {
            _onValueChanged?.Invoke(stringValue, null);
        }

        public void OnValueChanged(float floatValue)
        {
            _onValueChanged?.Invoke(floatValue, null);
        }

        public void OnValueChanged(int intValue)
        {
            _onValueChanged?.Invoke(intValue, null);
        }

        public void OnLeftButtonClick()
        {
            _value = ((int)_value <= 0)? 0: _value;
            SetValue();
        }

        public void OnRightButtonClick()
        {
            _value = ((int)_value + 1) % _stringArray.Length;
            SetValue();
        }

        private void SetValue()
        {
            if (valueText && (int)_value < _stringArray.Length)
            {
                valueText.text = _stringArray[(int)_value].ToString();
            }
        }

        private void SetValueToggle()
        {
            if (toggle != null)
            {
                toggle.isOn = (bool)_value;
            }
        }

        public UnityEngine.Object InitButton(string labelContent, Action<object, UnityEngine.Object> onValueChanged)
        {
            if (label != null)
            {
                label.SetText(labelContent);
            }
            var b = button;
            if (button)
            {
                button.onClick.AddListener(() =>
                {
                    onValueChanged?.Invoke(0, b);
                });
            }
            return button;
        }

        public UnityEngine.Object InitToggle(string labelContent, Action<object, UnityEngine.Object> onValueChanged, bool defaultValue)
        {
            _onValueChanged += onValueChanged;
            if (label != null)
            {
                label.SetText(labelContent);
            }
            _value = defaultValue;
            if (valueText != null)
            {
                valueText.text = defaultValue.ToString();
            }
            SetValueToggle();
            if (toggle != null)
            {

                toggle.onValueChanged.AddListener((b) =>
                {
                    onValueChanged?.Invoke(b, toggle);
                });
            }
            return toggle;
        }

        public UnityEngine.Object InitDropdown(string labelContent, Action<object, UnityEngine.Object> onValueChanged, List<string> contents, int defaultValue)
        {
            _onValueChanged += onValueChanged;
            if (label != null)
            {
                label.SetText(labelContent);
            }
            _value = defaultValue;
            if (dropdown != null)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(contents);
                dropdown.value = defaultValue;
                dropdown.onValueChanged.AddListener(
                    (i) =>
                    {
                        onValueChanged?.Invoke(i, dropdown);
                    }
                );
            }
            return dropdown;
        }

        public UnityEngine.Object InitSlider(string labelContent, object minValue, object maxValue, object defaultValue, Action<object, UnityEngine.Object> onValueChanged, bool wholeNumbers)
        {
            void setTextValue(Slider slider, float value)
            {
                if (valueText != null)
                {
                    // Format to 0.00 if wholeNumber is false
                    valueText.text = slider.wholeNumbers ? value.ToString("0") : value.ToString("0.00");
                }
            }
            _onValueChanged += onValueChanged;
            if (label != null)
            {
                label.SetText(labelContent);
            }
            _value = defaultValue;
            if (slider != null)
            {
                slider.wholeNumbers = wholeNumbers;
                if (wholeNumbers)
                {
                    slider.minValue = (int)minValue;
                    slider.maxValue = (int)maxValue;
                    slider.value = (int)defaultValue;
                    setTextValue(slider, slider.value);
                }
                else
                {
                    slider.minValue = (float)minValue;
                    slider.maxValue = (float)maxValue;
                    slider.value = (float)defaultValue;
                    setTextValue(slider, slider.value);
                }
                slider.onValueChanged.AddListener(
                    (f) =>
                    {
                        setTextValue(slider, f);
                        onValueChanged?.Invoke(f, slider);
                    }
                );
            }
            return slider;
        }

        public void Init(string labelContent, Action<object, UnityEngine.Object> onValueChanged, string[] stringArray, int defaultIndex)
        {
            _onValueChanged += onValueChanged;
            _stringArray = stringArray;
            _value = defaultIndex;

            SetValue();
        }
    }
}

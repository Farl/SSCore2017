/* (C)2019 Rayark Inc. - All Rights Reserved
 * Rayark Confidential
 *
 * NOTICE: The intellectual and technical concepts contained herein are
 * proprietary to or under control of Rayark Inc. and its affiliates.
 * The information herein may be covered by patents, patents in process,
 * and are protected by trade secret or copyright law.
 * You may not disseminate this information or reproduce this material
 * unless otherwise prior agreed by Rayark Inc. in writing.
 */


namespace JetGen
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class LayoutBuilder : ILayoutBuilder
    {
        private readonly List<IHoVLayoutElement> _elements;
        private readonly Dictionary<int, IHoVLayoutElement> _elementDict;
        private readonly RectTransform _rectTransform;
        private float _paddingTop;
        private float _paddingBottom;
        private float _paddingLeft;
        private float _paddingRight;
        private Vector2 _spacing;
        private int _axis = 1;
        private int _colCount = 1;

        /// <summary>Create a LayoutBuilder along with a new RectTransform to attached to.</summary>
        public LayoutBuilder(string objectName) : this(_CreateRectTransform(objectName))
        {
        }

        /// <summary>Create a LayoutBuilder given a RectTransform to attach to.</summary>
        public LayoutBuilder(RectTransform contentRect)
        {
            _elements = new List<IHoVLayoutElement>();
            _elementDict = new Dictionary<int, IHoVLayoutElement>();
            _rectTransform = contentRect;
            _paddingLeft = _paddingRight = _paddingTop = _paddingBottom = 0f;
            _spacing = Vector2.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutGroup"></param>
        public LayoutBuilder(RectTransform contentRect, HorizontalOrVerticalLayoutGroup layoutGroup)
        {
            _elements = new List<IHoVLayoutElement>();
            _elementDict = new Dictionary<int, IHoVLayoutElement>();
            _rectTransform = contentRect;
            if (layoutGroup != null)
            {
                _paddingLeft = layoutGroup.padding.left;
                _paddingRight = layoutGroup.padding.right;
                _paddingTop = layoutGroup.padding.top;
                _paddingBottom = layoutGroup.padding.bottom;
                _spacing = new Vector2(layoutGroup.spacing, layoutGroup.spacing);
                layoutGroup.enabled = false;
            }
            else
            {
                _paddingLeft = _paddingRight = _paddingTop = _paddingBottom = 0f;
                _spacing = Vector2.zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutGroup"></param>
        public LayoutBuilder(RectTransform contentRect, GridLayoutGroup layoutGroup)
        {
            _elements = new List<IHoVLayoutElement>();
            _elementDict = new Dictionary<int, IHoVLayoutElement>();
            _rectTransform = contentRect;
            if (layoutGroup != null)
            {
                _paddingLeft = layoutGroup.padding.left;
                _paddingRight = layoutGroup.padding.right;
                _paddingTop = layoutGroup.padding.top;
                _paddingBottom = layoutGroup.padding.bottom;
                _spacing = layoutGroup.spacing;
                layoutGroup.enabled = false;

                foreach (RectTransform trans in contentRect)
                {
                    trans.gameObject.SetActive(false);
                }
            }
            else
            {
                _paddingLeft = _paddingRight = _paddingTop = _paddingBottom = 0f;
                _spacing = Vector2.zero;
            }
        }

        public void DestroyGameObject()
        {
            if (_rectTransform)
            {
                if (Application.isPlaying)
                    Object.Destroy(_rectTransform.gameObject);
                else
                    Object.DestroyImmediate(_rectTransform.gameObject);
            }
        }

        IHoVLayout ILayoutBuilder.GetLayout()
        {
            var position = _rectTransform.anchoredPosition;
            _DoLayout();
            _rectTransform.anchoredPosition = position;
            return new HoVLayout(_axis, _rectTransform, _elements, _elementDict);
        }

        private void _DoLayout()
        {
            var topLeftPadding = new Vector2(_paddingLeft, _paddingTop);
            var mainOffset = 0f;
            var subOffset = 0f;
            var maxMainOffset = 0f;
            var maxSubOffset = 0f;
            var curColCount = 0;
            var subAxis = 1 ^ _axis;
            foreach (var element in _elements)
            {
                var mainPos = topLeftPadding[_axis] + mainOffset;
                var subPos = topLeftPadding[subAxis] + subOffset;
                element.Position = _GetElementPos(_axis, mainPos, subPos);
                subOffset += element.Size[subAxis] + _spacing[subAxis];
                maxSubOffset = Mathf.Max(maxSubOffset, subOffset);
                maxMainOffset = Mathf.Max(maxMainOffset, mainOffset + element.Size[_axis]);
                curColCount++;

                if (curColCount >= _colCount)
                {
                    mainOffset = maxMainOffset + _spacing[_axis];
                    subOffset = 0f;
                    curColCount = 0;
                }
            }
            if (_rectTransform.anchorMin.x != 0)
            {
                Debug.LogWarning("Layout should have anchorMin set to the left.", _rectTransform);
            }
            if (_rectTransform.anchorMax.y != 1)
            {
                Debug.LogWarning("Layout should have anchorMax set to the top.", _rectTransform);
            }
            if (_axis == (int)RectTransform.Axis.Vertical)
            {
                var height = maxMainOffset + _paddingBottom;
                var width = maxSubOffset + _paddingRight;
                _rectTransform.offsetMin = new Vector2(
                    0f,
                    -height
                );
                _rectTransform.offsetMax = new Vector2(
                    width,
                    0f
                );
            }
            else
            {
                var height = maxSubOffset + _paddingBottom;
                var width = maxMainOffset + _paddingRight;
                _rectTransform.offsetMin = new Vector2(
                    0f,
                    -height
                );
                _rectTransform.offsetMax = new Vector2(
                    width,
                    0f
                );

            }
        }

        private Vector2 _GetElementPos(int axis, float mainPos, float subPos)
        {
            if (axis == (int)RectTransform.Axis.Vertical)
            {
                return new Vector2(subPos, mainPos);
            }
            else
            {
                return new Vector2(mainPos, subPos);
            }
        }

        ILayoutBuilder ILayoutBuilder.Add(IHoVLayoutElement element)
        {
            _elements.Add(element);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.Add(IHoVLayoutElement element, int id)
        {
            _elementDict.Add(id, element);
            _elements.Add(element);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetAxis(int axis)
        {
            _axis = axis;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetColumnCount(int count)
        {
            _colCount = count;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetSpacing(float spacing)
        {
            _spacing = new Vector2(spacing, spacing);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetSpacing(float spacingX, float spacingY)
        {
            _spacing = new Vector2(spacingX, spacingY);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float padding)
        {
            _paddingLeft = _paddingRight = _paddingTop = _paddingBottom = padding;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float topBottom, float leftRight)
        {
            _paddingTop = _paddingBottom = topBottom;
            _paddingLeft = _paddingRight = leftRight;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float top, float leftRight, float bottom)
        {
            _paddingTop = top;
            _paddingLeft = _paddingRight = leftRight;
            _paddingBottom = bottom;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float top, float right, float bottom, float left)
        {
            _paddingTop = top;
            _paddingLeft = left;
            _paddingRight = right;
            _paddingBottom = bottom;
            return this;
        }

        private static RectTransform _CreateRectTransform(string objectName)
        {
            var obj = new GameObject(objectName, typeof(RectTransform));
            if (!Application.isPlaying)
            {
                obj.name = $"(DontSave){obj.name}";
                obj.hideFlags = HideFlags.DontSave;
            }
            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.position = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.rotation = Quaternion.identity;
            return rectTransform;
        }
    }
}

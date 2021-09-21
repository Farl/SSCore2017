
namespace JetGen
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GridLayoutGroupBuilder : ILayoutBuilder
    {
        private readonly List<IHoVLayoutElement> _elements;
        private readonly Dictionary<int, IHoVLayoutElement> _elementDict;
        private readonly RectTransform _rectTransform;

        private CustomGridLayoutGroup _gridLayoutGroup;

        private int _axis
        {
            get
            {
                if (_gridLayoutGroup)
                    return ((int)_gridLayoutGroup.startAxis + 1) % 2;
                return 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutGroup"></param>
        public GridLayoutGroupBuilder(RectTransform contentRect, CustomGridLayoutGroup layoutGroup, bool keepChildren = true)
        {
            _elements = new List<IHoVLayoutElement>();
            _elementDict = new Dictionary<int, IHoVLayoutElement>();
            _rectTransform = contentRect;
            if (layoutGroup != null)
            {
                _gridLayoutGroup = layoutGroup;
                var contentSizeFitter = _gridLayoutGroup.GetComponent<ContentSizeFitter>();
                if (contentSizeFitter)
                {
                    contentSizeFitter.enabled = false;
                }
                _gridLayoutGroup.enabled = false;

                // Fix anchor
                _rectTransform.anchorMin = new Vector2(0, 1);
                _rectTransform.anchorMax = new Vector2(0, 1);

                // Deactive all children in group layout
                foreach (RectTransform trans in contentRect)
                {
                    trans.anchorMin = new Vector2(0, 1);
                    trans.anchorMax = new Vector2(0, 1);

                    //var layoutEle = trans.GetOrAddComponent<LayoutElement>();
                    //layoutEle.ignoreLayout = true;

                    if (!keepChildren)
                        trans.gameObject.SetActive(false);
                }
            }
        }

        IHoVLayout ILayoutBuilder.GetLayout()
        {
            if (_gridLayoutGroup == null)
                return null;

            var position = _rectTransform.anchoredPosition;
            _DoLayout();
            _rectTransform.anchoredPosition = position;
            return new HoVLayout(_axis, _rectTransform, _elements, _elementDict);
        }

        private void _DoLayout()
        {
            if (_gridLayoutGroup == null)
                return;

            var topLeftPadding = new Vector2(_gridLayoutGroup.padding.left, _gridLayoutGroup.padding.top);
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
                subOffset += element.Size[subAxis] + _gridLayoutGroup.spacing[subAxis];
                maxSubOffset = Mathf.Max(maxSubOffset, subOffset);
                maxMainOffset = Mathf.Max(maxMainOffset, mainOffset + element.Size[_axis]);
                curColCount++;

                if (curColCount >= _gridLayoutGroup.constraintCount)
                {
                    mainOffset = maxMainOffset + _gridLayoutGroup.spacing[_axis];
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
                var height = maxMainOffset + _gridLayoutGroup.padding.bottom;
                var width = maxSubOffset + _gridLayoutGroup.padding.right;
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
                var height = maxSubOffset + _gridLayoutGroup.padding.bottom;
                var width = maxMainOffset + _gridLayoutGroup.padding.right;
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
            if (_gridLayoutGroup)
                _gridLayoutGroup.startAxis = (CustomGridLayoutGroup.Axis)axis;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetColumnCount(int count)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.constraintCount = count;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetSpacing(float spacing)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.spacing = new Vector2(spacing, spacing);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetSpacing(float spacingX, float spacingY)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.spacing = new Vector2(spacingX, spacingY);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float padding)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float topBottom, float leftRight)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.padding = new RectOffset((int)leftRight, (int)leftRight, (int)topBottom, (int)topBottom);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float top, float leftRight, float bottom)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.padding = new RectOffset((int)leftRight, (int)leftRight, (int)top, (int)bottom);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float top, float right, float bottom, float left)
        {
            if (_gridLayoutGroup)
                _gridLayoutGroup.padding = new RectOffset((int)left, (int)right, (int)top, (int)bottom);
            return this;
        }
    }
}

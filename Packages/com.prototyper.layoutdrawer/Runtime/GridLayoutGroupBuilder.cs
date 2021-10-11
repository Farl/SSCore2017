
namespace JetGen
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class GridLayoutGroupBuilder : ILayoutBuilder
    {
        private readonly List<IHoVLayoutElement> _elements;
        private readonly Dictionary<int, IHoVLayoutElement> _elementDict;
        private readonly RectTransform _rectTransform;

        private CustomGridLayoutGroup _gridLayoutGroup;

        private int _startAxis
        {
            get
            {
                if (_gridLayoutGroup)
                    return (int)_gridLayoutGroup.startAxis;
                return 0;
            }
        }

        private int _extendAxis
        {
            get
            {
                return 1 ^ _startAxis;
            }
        }

        public static ILayoutBuilder CreateLayoutBuilder(LayoutGroup layoutGroup, bool keepChildren = true)
        {
            if (layoutGroup == null)
                return null;

            var t = layoutGroup.GetType();
            var rt = layoutGroup.GetComponent<RectTransform>();

            if (t == typeof(CustomGridLayoutGroup))
            {
                return new GridLayoutGroupBuilder(rt, layoutGroup as CustomGridLayoutGroup, keepChildren);
            }
            else if (t.IsSubclassOf(typeof(HorizontalOrVerticalLayoutGroup)))
            {
                return new HoVLayoutGroupBuilder(rt, layoutGroup as HorizontalOrVerticalLayoutGroup, keepChildren);
            }
            return null;
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
                var uiBehaviours = _rectTransform.GetComponents<UIBehaviour>();
                foreach (var uiB in uiBehaviours)
                {
                    var layoutSelfController = uiB as ILayoutSelfController;
                    if (layoutSelfController != null)
                    {
                        uiB.enabled = false;
                    }
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
            return new HoVLayout(_extendAxis, _rectTransform, _elements, _elementDict);
        }

        private void _DoLayout()
        {
            if (_gridLayoutGroup == null)
                return;

            var padding = _gridLayoutGroup.padding;
            var spacing = _gridLayoutGroup.spacing;

            int startAxis = _startAxis;
            Vector2 offset = Vector2.zero;
            Vector2 maxOffset = Vector2.zero;
            int curColCount = 0;
            int curRowCount = 0;
            List<float> rowSizeList = new List<float>();
            List<Rect> rectList = new List<Rect>();

            foreach (var element in _elements)
            {
                Vector2 size = element.Size;
                Vector2 position = _gridLayoutGroup.Calculate(size, ref curColCount, ref curRowCount, ref offset, ref maxOffset, ref rowSizeList);
                rectList.Add(new Rect(position, size));
            }

            curColCount = curRowCount = 0;
            int i = 0;
            foreach (var element in _elements)
            {
                float _startOffset = 0;
                var position = rectList[i].position;
                _gridLayoutGroup.CountColumnRow(rowSizeList, ref curColCount, ref curRowCount, ref _startOffset);
                position[startAxis] += _startOffset;

                // Set child
                element.Position = position;
                i++;
            }

            if (_rectTransform.anchorMin.x != 0)
            {
                Debug.LogWarning("Layout should have anchorMin set to the left.", _rectTransform);
            }
            if (_rectTransform.anchorMax.y != 1)
            {
                Debug.LogWarning("Layout should have anchorMax set to the top.", _rectTransform);
            }

            _rectTransform.offsetMin = new Vector2(
                0f,
                -(maxOffset.y + padding.vertical)
            );
            _rectTransform.offsetMax = new Vector2(
                (maxOffset.x + padding.horizontal),
                0f
            );
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
                _gridLayoutGroup.startAxis = (CustomGridLayoutGroup.Axis)(1 ^ axis);
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

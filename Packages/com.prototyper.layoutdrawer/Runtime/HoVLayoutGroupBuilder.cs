
namespace SS
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class HoVLayoutGroupBuilder : ILayoutBuilder
    {
        private readonly List<IHoVLayoutElement> _elements;
        private readonly Dictionary<int, IHoVLayoutElement> _elementDict;
        private readonly RectTransform _rectTransform;

        private HorizontalOrVerticalLayoutGroup _layoutGroup;

        private int _axis
        {
            get
            {
                if (_layoutGroup)
                    return ((_layoutGroup as VerticalLayoutGroup) != null)? 1: 0;
                return 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutGroup"></param>
        public HoVLayoutGroupBuilder(RectTransform contentRect, HorizontalOrVerticalLayoutGroup layoutGroup, bool keepChildren = true)
        {
            _elements = new List<IHoVLayoutElement>();
            _elementDict = new Dictionary<int, IHoVLayoutElement>();
            _rectTransform = contentRect;
            if (layoutGroup != null)
            {
                _layoutGroup = layoutGroup;
                var uiBehaviours = _rectTransform.GetComponents<UIBehaviour>();
                foreach (var uiB in uiBehaviours)
                {
                    var layoutSelfController = uiB as ILayoutSelfController;
                    if (layoutSelfController != null)
                    {
                        uiB.enabled = false;
                    }
                }
                _layoutGroup.enabled = false;

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
            if (_layoutGroup == null)
                return null;

            var position = _rectTransform.anchoredPosition;
            _DoLayout();
            _rectTransform.anchoredPosition = position;
            return new HoVLayout(_axis, _rectTransform, _elements, _elementDict);
        }

        private void _DoLayout()
        {
            if (_layoutGroup == null)
                return;

            var topLeftPadding = new Vector2(_layoutGroup.padding.left, _layoutGroup.padding.top);
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
                subOffset += element.Size[subAxis] + _layoutGroup.spacing;
                maxSubOffset = Mathf.Max(maxSubOffset, subOffset);
                maxMainOffset = Mathf.Max(maxMainOffset, mainOffset + element.Size[_axis]);
                curColCount++;

                if (curColCount >= 1)
                {
                    mainOffset = maxMainOffset + _layoutGroup.spacing;
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
                var height = maxMainOffset + _layoutGroup.padding.bottom;
                var width = maxSubOffset + _layoutGroup.padding.right;
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
                var height = maxSubOffset + _layoutGroup.padding.bottom;
                var width = maxMainOffset + _layoutGroup.padding.right;
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
            Debug.LogWarning("Useless setting");
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetColumnCount(int count)
        {
            Debug.LogWarning("Useless setting");
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetSpacing(float spacing)
        {
            if (_layoutGroup)
                _layoutGroup.spacing = spacing;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetSpacing(float spacingX, float spacingY)
        {
            if (_layoutGroup)
                _layoutGroup.spacing = (_axis == 0)? spacingX: spacingY;
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float padding)
        {
            if (_layoutGroup)
                _layoutGroup.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float topBottom, float leftRight)
        {
            if (_layoutGroup)
                _layoutGroup.padding = new RectOffset((int)leftRight, (int)leftRight, (int)topBottom, (int)topBottom);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float top, float leftRight, float bottom)
        {
            if (_layoutGroup)
                _layoutGroup.padding = new RectOffset((int)leftRight, (int)leftRight, (int)top, (int)bottom);
            return this;
        }

        ILayoutBuilder ILayoutBuilder.SetPadding(float top, float right, float bottom, float left)
        {
            if (_layoutGroup)
                _layoutGroup.padding = new RectOffset((int)left, (int)right, (int)top, (int)bottom);
            return this;
        }
    }
}

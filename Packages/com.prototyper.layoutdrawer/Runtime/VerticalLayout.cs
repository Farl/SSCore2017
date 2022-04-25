
namespace SS
{
    using System.Collections.Generic;
    using UnityEngine;
    public class VerticalLayout : IHoVLayoutV1
    {
        private class DoLayoutVisitor : IHoVLayoutElementVisitor
        {
            void IHoVLayoutElementVisitor.Visit(HorizontalLayout element)
            {
            }

            void IHoVLayoutElementVisitor.Visit(VerticalLayout element)
            {
                element._DoLayout();
            }

            void IHoVLayoutElementVisitor.Visit(HoVLayout element)
            {
            }

            void IHoVLayoutElementVisitor.Visit<TPrefab, TState>(PooledLayoutElement<TPrefab, TState> element)
            {
            }

            void IHoVLayoutElementVisitor.Visit(StaticLayoutElement element)
            {
            }

            void IHoVLayoutElementVisitor.Visit(RowLayoutElement element)
            {
            }
        }

        private readonly float _width;
        private float _height;

        private readonly List<IHoVLayoutElement> _elements;
        private readonly Dictionary<int, IHoVLayoutElement> _elementDict;
        private readonly RectTransform _rectTransform;
        private readonly DoLayoutVisitor _doLayoutVisitor;
        private float _positionX;
        private float _positionY;
        private float _paddingLeft;
        private float _paddingRight;
        private float _paddingTop;
        private float _paddingBottom;
        private float _spacing;

        public VerticalLayout(RectTransform contentRect)
        {
            _elements = new List<IHoVLayoutElement>();
            _elementDict = new Dictionary<int, IHoVLayoutElement>();
            _rectTransform = contentRect;
            _width = contentRect.rect.width;
            _doLayoutVisitor = new DoLayoutVisitor();
            _paddingLeft = _paddingRight = _paddingTop = _paddingBottom = 0f;
            _spacing = 0f;
        }

        int IHoVLayout.Axis => 1;
        bool IHoVLayout.RedrawBothAxis => false;

        RectTransform IHoVLayout.RectTransform => _rectTransform;

        float IHoVLayoutV1.PaddingLeft
        {
            get => _paddingLeft;
            set => _paddingLeft = value;
        }

        float IHoVLayoutV1.PaddingRight
        {
            get => _paddingRight;
            set => _paddingRight = value;
        }

        float IHoVLayoutV1.PaddingTop
        {
            get => _paddingTop;
            set => _paddingTop = value;
        }

        float IHoVLayoutV1.PaddingBottom
        {
            get => _paddingBottom;
            set => _paddingBottom = value;
        }

        float IHoVLayoutV1.Spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        Vector2 IHoVLayoutElement.Position
        {
            get => new Vector2(_positionX, _positionY);
            set
            {
                _positionX = value.x;
                _positionY = value.y;
            }
        }

        float IHoVLayoutElement.PositionX
        {
            get => _positionX;
            set => _positionX = value;
        }

        float IHoVLayoutElement.PositionY
        {
            get => _positionY;
            set => _positionY = value;
        }

        Vector2 IHoVLayoutElement.Size => new Vector2(_width, _height);
        float IHoVLayoutElement.Width => _width;
        float IHoVLayoutElement.Height => _height;

        IHoVLayoutElement IHoVLayoutElement.Parent { get; set; }

        void IHoVLayoutElement.Accept(IHoVLayoutElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        void IHoVLayoutV1.DoLayout()
        {
            foreach (var element in _elements)
                element.Accept(_doLayoutVisitor);

            var position = _rectTransform.anchoredPosition;

            _DoLayout();

            _rectTransform.anchoredPosition = position;
        }

        private void _DoLayout()
        {
            var positionY = _paddingTop;
            foreach (var element in _elements)
            {
                element.PositionY = positionY;
                element.PositionX = _paddingLeft;
                positionY += element.Height + _spacing;
            }

            _height = positionY + _paddingBottom;

            _rectTransform.offsetMax =
                new Vector2(_rectTransform.offsetMin.x, _positionY);

            _rectTransform.offsetMin =
                new Vector2(_rectTransform.offsetMin.x, _positionY - _height);
        }

        void IHoVLayoutV1.Add(IHoVLayoutElement element)
        {
            _elements.Add(element);
            element.Parent = this;
        }

        void IHoVLayoutV1.Add(IHoVLayoutElement element, int id)
        {
            _elementDict.Add(id, element);
            (this as IHoVLayoutV1).Add(element);
        }

        void IHoVLayoutV1.SetPadding(float padding)
        {
            _paddingLeft = _paddingRight = _paddingTop = _paddingBottom = padding;
        }

        void IHoVLayoutV1.SetPadding(float topBottom, float leftRight)
        {
            _paddingTop = _paddingBottom = topBottom;
            _paddingLeft = _paddingRight = leftRight;
        }

        void IHoVLayoutV1.SetPadding(float top, float leftRight, float bottom)
        {
            _paddingTop = top;
            _paddingLeft = _paddingRight = leftRight;
            _paddingBottom = bottom;
        }

        void IHoVLayoutV1.SetPadding(float top, float right, float bottom, float left)
        {
            _paddingTop = top;
            _paddingLeft = left;
            _paddingRight = right;
            _paddingBottom = bottom;
        }

        IHoVLayoutElement IHoVLayout.GetElementById(int id)
        {
            if (_elementDict.TryGetValue(id, out var value))
            {
                return value;
            }
            return null;
        }

        IEnumerable<IHoVLayoutElement> IHoVLayout.GetElements(Vector2 min, Vector2 max)
        {
            foreach (var element in _elements)
            {
                var startPos = element.Position;
                if (startPos.x > max.x || startPos.y > max.y)
                    yield break;

                var endPos = element.Position + element.Size;
                if (endPos.x >= min.x || endPos.y >= min.y)
                {
                    yield return element;
                }
            }
        }

        IEnumerable<IHoVLayoutElement> IHoVLayout.GetElements(float begin, float end)
        {
            var posY = 0f;
            foreach (var element in _elements)
            {
                if (posY > end)
                    yield break;

                posY += element.Height + _spacing;

                if (posY >= begin)
                {
                    yield return element;
                }
            }
        }
    }
}

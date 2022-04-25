
namespace SS
{
    using UnityEngine;
    using System.Collections.Generic;

    public class RowLayoutElement : IHoVLayoutElement
    {
        private Vector2 _position;
        private readonly float _height;
        private float _width;
        private readonly float _cellWidth;
        private readonly List<IHoVLayoutElement> _elements;
        private IHoVLayoutElement _parent;

        public RowLayoutElement(float height, float cellWidth)
        {
            _height = height;
            _width = 0;
            _cellWidth = cellWidth;
            _elements = new List<IHoVLayoutElement>();
        }

        public List<IHoVLayoutElement> Elements => _elements;

        public float CellWidth => _cellWidth;

        Vector2 IHoVLayoutElement.Position
        {
            get => _position;

            set
            {
                _position = value;
                foreach (var e in _elements)
                    e.Position = value;
            }
        }

        float IHoVLayoutElement.PositionX
        {
            get => _position.x;

            set
            {
                _position = new Vector2(value, _position.y);
                foreach (var e in _elements)
                    e.PositionX = value;
            }
        }

        float IHoVLayoutElement.PositionY
        {
            get => _position.y;

            set
            {
                _position = new Vector2(_position.x, value);
                foreach (var e in _elements)
                    e.PositionY = value;
            }
        }

        IHoVLayoutElement IHoVLayoutElement.Parent
        {
            get => _parent;

            set
            {
                _parent = value;

                foreach (var e in _elements)
                    e.Parent = value;
            }
        }

        Vector2 IHoVLayoutElement.Size => new Vector2(_width, _height);
        float IHoVLayoutElement.Height => _height;
        float IHoVLayoutElement.Width => _width;

        public void Add(IHoVLayoutElement element)
        {
            _width += element.Width;
            _elements.Add(element);
        }

        void IHoVLayoutElement.Accept(IHoVLayoutElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
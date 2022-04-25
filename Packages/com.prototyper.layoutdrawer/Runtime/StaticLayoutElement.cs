
namespace SS
{
    using UnityEngine;
    public class StaticLayoutElement : IHoVLayoutElement
    {
        private readonly RectTransform _rectTransform;
        private readonly Vector2 _size;
        private Vector2 _position;

        public StaticLayoutElement(RectTransform rectTransform)
        {
            _rectTransform = rectTransform;
            _size = _rectTransform.rect.size;
            _rectTransform.gameObject.SetActive(false);
        }

        public RectTransform RectTransform => _rectTransform;

        Vector2 IHoVLayoutElement.Position
        {
            get => _position;
            set => _position = value;
        }

        float IHoVLayoutElement.PositionX
        {
            get => _position.x;
            set => _position = new Vector2(value, _position.y);
        }

        float IHoVLayoutElement.PositionY
        {
            get => _position.y;
            set => _position = new Vector2(_position.x, value);
        }

        Vector2 IHoVLayoutElement.Size => _size;
        float IHoVLayoutElement.Width => _size.x;
        float IHoVLayoutElement.Height => _size.y;

        IHoVLayoutElement IHoVLayoutElement.Parent { get; set; }

        void IHoVLayoutElement.Accept(IHoVLayoutElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

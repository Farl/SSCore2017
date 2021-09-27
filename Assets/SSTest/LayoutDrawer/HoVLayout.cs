/* (C)2021 Rayark Inc. - All Rights Reserved
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
    public class HoVLayout : IHoVLayout
    {
        private readonly int _axis;
        private readonly RectTransform _contentRect;
        private readonly List<IHoVLayoutElement> _elementList;
        private readonly Dictionary<int, IHoVLayoutElement> _elementDict;
        private IHoVLayoutElement _parent;
        private Vector2 _position;
        private Vector2 _size;
        public HoVLayout(
            int axis,
            RectTransform contentRect,
            List<IHoVLayoutElement> elementList,
            Dictionary<int, IHoVLayoutElement> elementDict
            )
        {
            _axis = axis;
            _contentRect = contentRect;
            _elementList = elementList;
            _elementDict = elementDict;
            _size = contentRect.rect.size;
            foreach (var element in _elementList)
            {
                element.Parent = this;
            }
        }

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
        IHoVLayoutElement IHoVLayoutElement.Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                if (value is IHoVLayout)
                {
                    _contentRect.SetParent((_parent as IHoVLayout).RectTransform);
                }
            }
        }
        int IHoVLayout.Axis => _axis;
        bool IHoVLayout.RedrawBothAxis => true;

        RectTransform IHoVLayout.RectTransform => _contentRect;
        void IHoVLayoutElement.Accept(IHoVLayoutElementVisitor visitor)
        {
            visitor.Visit(this);
        }
        IHoVLayoutElement IHoVLayout.GetElementById(int id)
        {
            if (_elementDict.TryGetValue(id, out var value))
            {
                return value;
            }
            foreach (var elementLayout in _elementList)
            {
                if (elementLayout is IHoVLayout)
                {
                    var element = (elementLayout as IHoVLayout).GetElementById(id);
                    if (element != null)
                        return element;
                }
            }
            return null;
        }
        IEnumerable<IHoVLayoutElement> IHoVLayout.GetElements(Vector2 begin, Vector2 end)
        {
            foreach (var element in _elementList)
            {
                var startPos = element.Position;
                if (startPos.x > end.x || startPos.y > end.y)
                    yield break;

                var endPos = element.Position + element.Size;
                if (endPos.x >= begin.x && endPos.y >= begin.y)
                {
                    yield return element;
                }
            }
        }

        IEnumerable<IHoVLayoutElement> IHoVLayout.GetElements(float begin, float end)
        {
            foreach (var element in _elementList)
            {
                var startPos = element.Position[_axis];
                if (startPos > end)
                    yield break;

                var endPos = element.Position[_axis] + element.Size[_axis];
                if (endPos >= begin)
                {
                    yield return element;
                }
            }
        }
    }
}

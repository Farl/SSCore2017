
namespace SS
{
    using System;
    using UnityEngine;

    public static class PooledLayoutElement
    {
        public static PooledLayoutElement<TPrefab, TState> Create<TPrefab, TState>(
            TPrefab prefab, int key, TState state)
            where TPrefab : MonoBehaviour
        {
            return new PooledLayoutElement<TPrefab, TState>(prefab, key, state);
        }

        public static PooledLayoutElement<TPrefab, TState> Create<TPrefab, TState>(
            TPrefab prefab, int key, TState state, Func<TState, float> getHeight)
            where TPrefab : MonoBehaviour
        {
            return new PooledLayoutElement<TPrefab, TState>(prefab, key, state, getHeight);
        }
    }

    public class PooledLayoutElement<TPrefab, TState> : IHoVLayoutElement where TPrefab : MonoBehaviour
    {
        private Vector2 _position;
        private readonly float _height;
        private readonly float _width;

        public PooledLayoutElement(
            TPrefab prefab,
            int key,
            TState state,
            Func<TState, float> getHeightFunc = null,
            Func<TState, float> getWidthFunc = null)
        {
            Key = key;
            State = state;
            Prefab = prefab;

            if (getHeightFunc == null)
            {
                _height = Prefab.GetComponent<RectTransform>().rect.height;
            }
            else
            {
                _height = getHeightFunc(State);
            }

            if (getWidthFunc == null)
            {
                _width = Prefab.GetComponent<RectTransform>().rect.width;
            }
            else
            {
                _width = getWidthFunc(State);
            }
        }

        public TPrefab Prefab { get; }

        public int Key { get; }

        public TState State { get; }

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

        Vector2 IHoVLayoutElement.Size => new Vector2(_width, _height);

        float IHoVLayoutElement.Height => _height;

        float IHoVLayoutElement.Width => _width;

        IHoVLayoutElement IHoVLayoutElement.Parent { get; set; }

        void IHoVLayoutElement.Accept(IHoVLayoutElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

namespace SS
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    public class LayoutDrawer : ILayoutDrawer
    {
        private class StaticLayoutElementHandler
        {
            private HashSet<RectTransform> _isVisible;
            private HashSet<RectTransform> _wasVisible;

            public StaticLayoutElementHandler()
            {
                _isVisible = new HashSet<RectTransform>();
            }

            public void Begin()
            {
                _wasVisible = _isVisible;
                _isVisible = new HashSet<RectTransform>();
            }

            public void Prepare(RectTransform transform)
            {
                _isVisible.Add(transform);
            }

            public void Draw(RectTransform transform)
            {
                if (!_wasVisible.Contains(transform))
                    transform.gameObject.SetActive(true);
            }

            public void Finish()
            {
                foreach (var e in _wasVisible)
                {
                    if (_isVisible.Contains(e))
                        continue;

                    e.gameObject.SetActive(false);
                }
            }
        }

        private interface IPooledLayoutElementHandler
        {
            void Begin();
            void Finish();
        }

        private class PooledLayoutElementHandler<TPrefab, TState>
                : IPooledLayoutElementHandler
                where TPrefab : MonoBehaviour
        {
            private readonly Func<RectTransform, TPrefab> _createEntry;
            private readonly Action<TPrefab, TState> _updateState;

            private readonly Dictionary<int, TPrefab> _entryMap;
            private readonly Queue<TPrefab> _unusedList;

            private HashSet<int> _isVisible;
            private HashSet<int> _wasVisible;
            private readonly Action<TPrefab> _reset;

            public PooledLayoutElementHandler(
                Func<RectTransform, TPrefab> createEntry,
                Action<TPrefab> reset,
                Action<TPrefab, TState> updateState)
            {
                _createEntry = createEntry;
                _updateState = updateState;
                _reset = reset;

                _entryMap = new Dictionary<int, TPrefab>();
                _isVisible = new HashSet<int>();
                _unusedList = new Queue<TPrefab>();
            }

            void IPooledLayoutElementHandler.Begin()
            {
                _wasVisible = _isVisible;
                _isVisible = new HashSet<int>();
            }

            public void Prepare(int key)
            {
                _isVisible.Add(key);
            }

            public TPrefab Create(int key, TState state, RectTransform parentTransform)
            {
                _entryMap.TryGetValue(key, out var entry);

                if (entry == null)
                {
                    foreach (var kvp in _entryMap)
                    {
                        if (_isVisible.Contains(kvp.Key))
                            continue;

                        entry = kvp.Value;
                        entry.transform.SetParent(parentTransform);
                        _entryMap.Remove(kvp.Key);
                        break;
                    }

                    if (entry == null)
                    {
                        if (_unusedList.Count > 0)
                        {
                            entry = _unusedList.Dequeue();
                            entry.gameObject.SetActive(true);
                            entry.transform.SetParent(parentTransform);
                        }
                        else
                        {
                            entry = _createEntry(parentTransform);
                        }
                    }

                    _reset(entry);

                    _entryMap[key] = entry;
                }

                _updateState(entry, state);

                return entry;
            }

            void IPooledLayoutElementHandler.Finish()
            {
                foreach (var e in _wasVisible)
                {
                    if (_isVisible.Contains(e))
                        continue;

                    if (!_entryMap.TryGetValue(e, out var entry))
                        continue;

                    entry.gameObject.SetActive(false);
                    _unusedList.Enqueue(entry);
                    _entryMap.Remove(e);
                }
            }
        }

        private IHoVLayout _layout;

        private readonly Dictionary<MonoBehaviour, IPooledLayoutElementHandler> _poolHandlers;

        private readonly PrepareToRedrawVisitor _prepareToRedraw;
        private readonly RedrawVisitor _redraw;
        private float _begin;
        private float _end;
        private readonly StaticLayoutElementHandler _staticHandler;

        public LayoutDrawer()
        {
            _staticHandler = new StaticLayoutElementHandler();
            _poolHandlers = new Dictionary<MonoBehaviour, IPooledLayoutElementHandler>();
            _prepareToRedraw = new PrepareToRedrawVisitor(this);
            _redraw = new RedrawVisitor(this);
        }

        void ILayoutDrawer.SetPoolHandler<TPrefab, TState>(
            TPrefab prefab,
            Func<RectTransform, TPrefab> createEntry,
            Action<TPrefab> resetEntry,
            Action<TPrefab, TState> updateState)
        {
            _poolHandlers[prefab] =
                new PooledLayoutElementHandler<TPrefab, TState>(createEntry, resetEntry, updateState);
        }

        void ILayoutDrawer.UpdateLayout(IHoVLayout layout)
        {
            _layout = layout;
            _redraw.Axis = layout.Axis;
        }

        void ILayoutDrawer.Redraw(float begin, float end)
        {
            if (_layout == null)
                return;

            _staticHandler.Begin();

            foreach (var e in _poolHandlers.Values)
                e.Begin();

            _begin = begin;
            _end = end;

            foreach (var element in _layout.GetElements(begin, end))
                element.Accept(_prepareToRedraw);

            foreach (var element in _layout.GetElements(begin, end))
                element.Accept(_redraw);

            foreach (var e in _poolHandlers.Values)
                e.Finish();

            _staticHandler.Finish();
        }

        void ILayoutDrawer.RedrawBothAxis(RectTransform viewport, RectTransform content)
        {
            if (_layout == null)
                return;

            _staticHandler.Begin();

            foreach (var e in _poolHandlers.Values)
                e.Begin();

            var begin = new Vector2(-content.offsetMin.x, content.offsetMax.y);
            var end = new Vector2(-content.offsetMin.x + viewport.rect.width, content.offsetMax.y + viewport.rect.height);

            _begin = (_layout.Axis == 0) ? begin.x : begin.y;
            _end = (_layout.Axis == 0) ? end.x : end.y;

            foreach (var element in _layout.GetElements(begin, end))
                element.Accept(_prepareToRedraw);

            foreach (var element in _layout.GetElements(begin, end))
                element.Accept(_redraw);

            foreach (var e in _poolHandlers.Values)
                e.Finish();

            _staticHandler.Finish();
        }

        void ILayoutDrawer.RedrawVertically(RectTransform viewport, RectTransform content)
        {
            (this as ILayoutDrawer).Redraw(
                content.offsetMax.y,
                content.offsetMax.y + viewport.rect.height);
        }

        void ILayoutDrawer.RedrawHorizontally(RectTransform viewport, RectTransform content)
        {
            (this as ILayoutDrawer).Redraw(
                -content.offsetMin.x,
                -content.offsetMin.x + viewport.rect.width);
        }

        void ILayoutDrawer.RedrawAlongAxis(RectTransform viewport, RectTransform content)
        {
            if (_layout == null)
                return;
            if (_layout.RedrawBothAxis)
            {
                (this as ILayoutDrawer).RedrawBothAxis(viewport, content);
            }
            else
            {
                if (_layout.Axis == (int)RectTransform.Axis.Vertical)
                {
                    (this as ILayoutDrawer).RedrawVertically(viewport, content);
                }
                else
                {
                    (this as ILayoutDrawer).RedrawHorizontally(viewport, content);
                }
            }
        }

        void ILayoutDrawer.RedrawAlongAxis(ScrollRect scrollRect)
        {
            (this as ILayoutDrawer).RedrawAlongAxis(scrollRect.viewport, scrollRect.content);
        }

        IHoVLayoutElement ILayoutDrawer.GetElementById(int id)
        {
            return _layout.GetElementById(id);
        }

        private class RedrawVisitor : IHoVLayoutElementVisitor
        {
            private readonly LayoutDrawer _drawer;

            public RedrawVisitor(LayoutDrawer drawer)
            {
                _drawer = drawer;
            }
            public int Axis { get; set; }

            void IHoVLayoutElementVisitor.Visit(HorizontalLayout layout)
            {
                IHoVLayout horizontalLayout = layout;
                foreach (var element in
                    horizontalLayout.GetElements(
                        _drawer._begin - horizontalLayout.Position[Axis],
                        _drawer._end - horizontalLayout.Position[Axis]))
                {
                    element.Accept(this);
                }

                _UpdatePosition(horizontalLayout, horizontalLayout.RectTransform);
            }

            void IHoVLayoutElementVisitor.Visit(VerticalLayout layout)
            {
                IHoVLayout verticalLayout = layout;
                foreach (var element in
                    verticalLayout.GetElements(
                        _drawer._begin - verticalLayout.Position[Axis],
                        _drawer._end - verticalLayout.Position[Axis]))
                {
                    element.Accept(this);
                }

                _UpdatePosition(verticalLayout, verticalLayout.RectTransform);
            }

            void IHoVLayoutElementVisitor.Visit<TPrefab, TState>(PooledLayoutElement<TPrefab, TState> element)
            {
                IHoVLayoutElement layoutElement = element;

                var poolHandler = _drawer._poolHandlers[element.Prefab] as PooledLayoutElementHandler<TPrefab, TState>;
                var entry =
                    poolHandler.Create(
                        element.Key,
                        element.State,
                        (layoutElement.Parent as IHoVLayout).RectTransform);

                _UpdatePosition(element, entry.GetComponent<RectTransform>());
            }

            void IHoVLayoutElementVisitor.Visit(StaticLayoutElement element)
            {
                var rectTransform = element.RectTransform;
                _drawer._staticHandler.Draw(rectTransform);
                _UpdatePosition(element, rectTransform);
            }

            void IHoVLayoutElementVisitor.Visit(RowLayoutElement element)
            {
                for (var i = 0; i < element.Elements.Count; ++i)
                {
                    element.Elements[i].Accept(this);
                }
            }
            void IHoVLayoutElementVisitor.Visit(HoVLayout layout)
            {
                IHoVLayout verticalLayout = layout;
                foreach (var element in
                    verticalLayout.GetElements(
                        _drawer._begin - verticalLayout.Position[Axis],
                        _drawer._end - verticalLayout.Position[Axis]))
                {
                    element.Accept(this);
                }

                _UpdatePosition(verticalLayout, verticalLayout.RectTransform);
            }

            private void _UpdatePosition(
                IHoVLayoutElement layoutElement,
                RectTransform rectTransform)
            {
                if (rectTransform.anchorMin.x != 0)
                {
                    Debug.LogWarning("layoutElement should have anchorMin set to the left.", rectTransform);
                }
                if (rectTransform.anchorMax.y != 1)
                {
                    Debug.LogWarning("layoutElement should have anchorMax set to the top.", rectTransform);
                }
                var subAxis = 1 ^ Axis;
                var maxSub = layoutElement.Position[subAxis] + layoutElement.Size[subAxis];
                var minSub = layoutElement.Position[subAxis];
                var maxMain = layoutElement.Position[Axis];
                var minMain = layoutElement.Position[Axis] + layoutElement.Size[Axis];

                if (Axis == (int)RectTransform.Axis.Vertical)
                {
                    rectTransform.offsetMin = new Vector2(minSub, -minMain);
                    rectTransform.offsetMax = new Vector2(maxSub, -maxMain);
                }
                else
                {
                    rectTransform.offsetMin = new Vector2(maxMain, -maxSub);
                    rectTransform.offsetMax = new Vector2(minMain, -minSub);
                }
            }
        }

        private class PrepareToRedrawVisitor : IHoVLayoutElementVisitor
        {
            private readonly LayoutDrawer _drawer;

            public PrepareToRedrawVisitor(LayoutDrawer drawer)
            {
                _drawer = drawer;
            }

            void IHoVLayoutElementVisitor.Visit(HorizontalLayout layout)
            {
                IHoVLayout horizontalLayout = layout;
                foreach (var element in
                    horizontalLayout.GetElements(
                        _drawer._begin - horizontalLayout.PositionX,
                        _drawer._end - horizontalLayout.PositionX))
                {
                    element.Accept(this);
                }
            }

            void IHoVLayoutElementVisitor.Visit(VerticalLayout layout)
            {
                IHoVLayout verticalLayout = layout;
                foreach (
                    var element
                    in verticalLayout
                        .GetElements(
                            _drawer._begin - verticalLayout.PositionY,
                            _drawer._end - verticalLayout.PositionY))
                {
                    element.Accept(this);
                }
            }

            void IHoVLayoutElementVisitor.Visit(HoVLayout layout)
            {
                IHoVLayout staticLayout = layout;
                var elements = staticLayout
                    .GetElements(
                        _drawer._begin - staticLayout.Position[staticLayout.Axis],
                        _drawer._end - staticLayout.Position[staticLayout.Axis]
                    );
                foreach (var element in elements)
                {
                    element.Accept(this);
                }
            }

            void IHoVLayoutElementVisitor.Visit<TPrefab, TState>(PooledLayoutElement<TPrefab, TState> element)
            {
                var poolHandler = _drawer._poolHandlers[element.Prefab] as PooledLayoutElementHandler<TPrefab, TState>;

                poolHandler.Prepare(element.Key);
            }

            void IHoVLayoutElementVisitor.Visit(StaticLayoutElement element)
            {
                _drawer._staticHandler.Prepare(element.RectTransform);
            }

            void IHoVLayoutElementVisitor.Visit(RowLayoutElement element)
            {
                foreach (var e in element.Elements)
                    e.Accept(this);
            }
        }
    }
}

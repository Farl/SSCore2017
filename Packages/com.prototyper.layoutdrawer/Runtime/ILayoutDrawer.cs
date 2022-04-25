namespace SS
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    public interface ILayoutDrawer
    {
        void SetPoolHandler<TPrefab, TState>(
            TPrefab prefab,
            Func<RectTransform, TPrefab> spawner,
            Action<TPrefab> resetter,
            Action<TPrefab, TState> updater) where TPrefab : MonoBehaviour;

        void UpdateLayout(IHoVLayout layout);

        void Redraw(float begin, float end);

        void RedrawVertically(RectTransform viewport, RectTransform content);

        void RedrawHorizontally(RectTransform viewport, RectTransform content);

        void RedrawBothAxis(RectTransform viewport, RectTransform content);

        void RedrawAlongAxis(RectTransform viewport, RectTransform content);

        void RedrawAlongAxis(ScrollRect scrollRect);

        IHoVLayoutElement GetElementById(int id);

    }
}

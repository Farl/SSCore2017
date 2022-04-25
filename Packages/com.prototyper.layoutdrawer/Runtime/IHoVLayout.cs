
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public interface IHoVLayout : IHoVLayoutElement
    {
        bool RedrawBothAxis { get; }
        int Axis { get; }
        RectTransform RectTransform { get; }
        IHoVLayoutElement GetElementById(int id);
        IEnumerable<IHoVLayoutElement> GetElements(float begin, float end);
        IEnumerable<IHoVLayoutElement> GetElements(Vector2 min, Vector2 max);
    }

    [System.Obsolete("Use LayoutBuilder to create layout instead")]
    public interface IHoVLayoutV1 : IHoVLayout
    {
        float PaddingLeft { get; set; }
        float PaddingRight { get; set; }
        float PaddingTop { get; set; }
        float PaddingBottom { get; set; }
        float Spacing { get; set; }

        void Add(IHoVLayoutElement element);
        void Add(IHoVLayoutElement element, int id);
        void SetPadding(float padding);
        void SetPadding(float topBottom, float leftRight);
        void SetPadding(float top, float leftRight, float bottom);
        void SetPadding(float top, float right, float bottom, float left);
        void DoLayout();
    }
}

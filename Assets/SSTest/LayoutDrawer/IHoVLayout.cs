/* (C)2019 Rayark Inc. - All Rights Reserved
 * Rayark Confidential
 *
 * NOTICE: The intellectual and technical concepts contained herein are
 * proprietary to or under control of Rayark Inc. and its affiliates.
 * The information herein may be covered by patents, patents in process,
 * and are protected by trade secret or copyright law.
 * You may not disseminate this information or reproduce this material
 * unless otherwise prior agreed by Rayark Inc. in writing.
 */

using System.Collections.Generic;
using UnityEngine;

namespace JetGen
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

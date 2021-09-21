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

namespace JetGen
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

        void RedrawAlongAxis(RectTransform viewport, RectTransform content);

        void RedrawAlongAxis(ScrollRect scrollRect);

        IHoVLayoutElement GetElementById(int id);

    }
}

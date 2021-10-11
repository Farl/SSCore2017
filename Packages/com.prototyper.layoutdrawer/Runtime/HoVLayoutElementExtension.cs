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
    using UnityEngine;
    public static class HoVLayoutElementExtension
    {
        public static float GetTopNormalizedPosition(this IHoVLayoutElement element, RectTransform viewport, RectTransform content)
        {
            var positionY = _TransformPositionYFromBottom(element);
            var normalizedPos = _CalculateVerticalNormalizedPosition(positionY, viewport, content, 1f);
            return normalizedPos;
        }

        public static float GetLeftNormalizedPosition(this IHoVLayoutElement element, RectTransform viewport, RectTransform content)
        {
            var positionX = _TransformPositionXFromLeft(element);
            var normalizedPos = _CalculateHorizontalNormalizedPosition(positionX, viewport, content, 0f);
            return normalizedPos;
        }

        public static float GetHorizontallyCenteredNormalizedPosition(this IHoVLayoutElement element, RectTransform viewport, RectTransform content)
        {
            var positionX = _GetCenterPositionXFromLeft(element);
            var normalizedPos = _CalculateHorizontalNormalizedPosition(positionX, viewport, content);
            return normalizedPos;
        }

        public static float GetVerticallyCenteredNormalizedPosition(this IHoVLayoutElement element, RectTransform viewport, RectTransform content)
        {
            var positionY = _GetCenterPositionYFromBottom(element);
            var normalizedPos = _CalculateVerticalNormalizedPosition(positionY, viewport, content);
            return normalizedPos;
        }

        private static float _TransformPositionXFromLeft(IHoVLayoutElement element)
        {
            var posX = 0f;
            var ptr = element;
            while (ptr.Parent != null)
            {
                posX += ptr.PositionX;
                ptr = ptr.Parent;
            }
            return posX;
        }

        private static float _TransformPositionYFromBottom(IHoVLayoutElement element)
        {
            var posY = 0f;
            var ptr = element;
            while (ptr.Parent != null)
            {
                posY += ptr.PositionY;
                ptr = ptr.Parent;
            }
            return ptr.Height - posY;
        }

        private static float _GetCenterPositionXFromLeft(IHoVLayoutElement element)
        {
            return _TransformPositionXFromLeft(element) + (element.Width / 2);
        }

        private static float _GetCenterPositionYFromBottom(IHoVLayoutElement element)
        {
            return _TransformPositionYFromBottom(element) - (element.Height / 2);
        }

        private static float _CalculateHorizontalNormalizedPosition(float positionX, RectTransform viewport, RectTransform content, float viewportRefRatio = 0.5f)
        {
            var viewportWidth = viewport.rect.width;
            var refPoint = viewportWidth * viewportRefRatio;

            return (positionX - refPoint) / (content.rect.width - viewportWidth);
        }

        private static float _CalculateVerticalNormalizedPosition(float positionY, RectTransform viewport, RectTransform content, float viewportRefRatio = 0.5f)
        {
            var viewportHeight = viewport.rect.height;
            var bottomRefPoint = viewportHeight * viewportRefRatio;

            return (positionY - bottomRefPoint) / (content.rect.height - viewportHeight);
        }
    }
}

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
    using UnityEngine;
    public interface IHoVLayoutElement
    {
        Vector2 Position { get; set; }
        float PositionX { get; set; }
        float PositionY { get; set; }

        Vector2 Size { get; }
        float Width { get; }
        float Height { get; }

        IHoVLayoutElement Parent { get; set; }

        void Accept(IHoVLayoutElementVisitor visitor);
    }
}
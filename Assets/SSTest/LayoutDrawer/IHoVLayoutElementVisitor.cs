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

using UnityEngine;

namespace JetGen
{
    public interface IHoVLayoutElementVisitor
    {
        void Visit(HorizontalLayout element);
        void Visit(VerticalLayout element);
        void Visit(HoVLayout element);
        void Visit<TPrefab, TState>(PooledLayoutElement<TPrefab, TState> element) where TPrefab : MonoBehaviour;
        void Visit(StaticLayoutElement element);
        void Visit(RowLayoutElement element);
    }
}

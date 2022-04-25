
using UnityEngine;

namespace SS
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

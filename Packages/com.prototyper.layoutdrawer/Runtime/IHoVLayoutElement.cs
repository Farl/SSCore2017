
namespace SS
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
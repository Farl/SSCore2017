namespace JetGen
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>ILayoutBuilder is an builder interface that builds an IStaticLayout</summary>
    public interface ILayoutBuilder
    {
        ILayoutBuilder Add(IHoVLayoutElement element);
        ILayoutBuilder Add(IHoVLayoutElement element, int id);
        IHoVLayout GetLayout();

        /// <summary>SetColumnCount sets how many elements are displayed along the sub axis.
        /// You could also set to 0 to auto-fill.</summary>
        /// <param name="count">count determines how many elements are displayed along the sub axis.
        /// Default value is 1, you could also set this value to 0 to auto-fill.</param>
        ILayoutBuilder SetColumnCount(int count);

        /// <summary>SetAxis sets the direction this layout is supposed to be scrolled.</summary>
        /// <param name="axis">1 means vertical, 0 means horizontal</param>
        ILayoutBuilder SetAxis(int axis);

        ILayoutBuilder SetSpacing(float spacing);
        ILayoutBuilder SetSpacing(float spacingX, float spacingY);
        ILayoutBuilder SetPadding(float padding);
        ILayoutBuilder SetPadding(float topBottom, float leftRight);
        ILayoutBuilder SetPadding(float top, float leftRight, float bottom);
        ILayoutBuilder SetPadding(float top, float right, float bottom, float left);
    }
}
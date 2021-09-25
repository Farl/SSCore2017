namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/SS/Horizontal or Vertical Layout Group", 150)]
    /// <summary>
    ///   Layout child layout elements side by side.
    /// </summary>
    public class CustomHorizontalOrVerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        public bool isVertical;
        protected CustomHorizontalOrVerticalLayoutGroup()
        { }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, isVertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, isVertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, isVertical);
        }

        /// <summary>
        /// Called by the layout system. Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, isVertical);
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Custom Grid Layout Group", 152)]
    /// <summary>
    ///   Layout class to arrange children elements in a grid format.
    /// </summary>
    /// <remarks>
    /// The GridLayoutGroup component is used to layout child layout elements in a uniform grid where all cells have the same size. The size and the spacing between cells is controlled by the GridLayoutGroup itself. The children have no influence on their sizes.
    /// </remarks>
    public class CustomGridLayoutGroup : LayoutGroup, ILayoutSelfController
    {
        /// <summary>
        /// Which corner is the starting corner for the grid.
        /// </summary>
        public enum Corner
        {
            /// <summary>
            /// Upper Left corner.
            /// </summary>
            UpperLeft = 0,
            /// <summary>
            /// Upper Right corner.
            /// </summary>
            UpperRight = 1,
            /// <summary>
            /// Lower Left corner.
            /// </summary>
            LowerLeft = 2,
            /// <summary>
            /// Lower Right corner.
            /// </summary>
            LowerRight = 3
        }

        /// <summary>
        /// The grid axis we are looking at.
        /// </summary>
        /// <remarks>
        /// As the storage is a [][] we make access easier by passing a axis.
        /// </remarks>
        public enum Axis
        {
            /// <summary>
            /// Horizontal axis
            /// </summary>
            Horizontal = 0,
            /// <summary>
            /// Vertical axis.
            /// </summary>
            Vertical = 1
        }

        /// <summary>
        /// Constraint type on either the number of columns or rows.
        /// </summary>
        public enum Constraint
        {
            /// <summary>
            /// Don't constrain the number of rows or columns.
            /// </summary>
            Flexible = 0,
            /// <summary>
            /// Constrain the number of columns to a specified number.
            /// </summary>
            FixedColumnCount = 1,
            /// <summary>
            /// Constraint the number of rows to a specified number.
            /// </summary>
            FixedRowCount = 2
        }

        [SerializeField] protected Corner m_StartCorner = Corner.UpperLeft;

        /// <summary>
        /// Which corner should the first cell be placed in?
        /// </summary>
        public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

        [SerializeField] protected Axis m_StartAxis = Axis.Horizontal;

        /// <summary>
        /// Which axis should cells be placed along first
        /// </summary>
        /// <remarks>
        /// When startAxis is set to horizontal, an entire row will be filled out before proceeding to the next row. When set to vertical, an entire column will be filled out before proceeding to the next column.
        /// </remarks>
        public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

        [SerializeField] protected bool fixedCellSize = false;

        [SerializeField] protected Vector2 m_CellSize = new Vector2(100, 100);

        /// <summary>
        /// The size to use for each cell in the grid.
        /// </summary>
        public Vector2 cellSize { get { return m_CellSize; } set { SetProperty(ref m_CellSize, value); } }

        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

        /// <summary>
        /// The spacing to use between layout elements in the grid on both axises.
        /// </summary>
        public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

        [SerializeField] protected Constraint m_Constraint = Constraint.FixedColumnCount;

        /// <summary>
        /// Which constraint to use for the GridLayoutGroup.
        /// </summary>
        /// <remarks>
        /// Specifying a constraint can make the GridLayoutGroup work better in conjunction with a [[ContentSizeFitter]] component. When GridLayoutGroup is used on a RectTransform with a manually specified size, there's no need to specify a constraint.
        /// </remarks>
        public Constraint constraint { get { return m_Constraint; } set { SetProperty(ref m_Constraint, value); } }

        [SerializeField] protected int m_ConstraintCount = 2;

        /// <summary>
        /// How many cells there should be along the constrained axis.
        /// </summary>
        public int constraintCount { get { return m_ConstraintCount; } set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); } }

        public bool isLayoutGroup = true;
        public bool isContentSizeFitter = false;
        public bool debugTest = true;

        private DrivenRectTransformTracker m_SelfTracker;

        protected CustomGridLayoutGroup()
        { }

        // Reference from content size fitter
        protected override void OnEnable()
        {
            base.OnEnable();
            if (isContentSizeFitter)
                SetLayoutDirty();
        }

        // Reference from content size fitter
        protected override void OnDisable()
        {
            if (isContentSizeFitter)
                LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        // Reference from content size fitter
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (isContentSizeFitter)
                SetLayoutDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            constraintCount = constraintCount;
            if (isContentSizeFitter)
                SetLayoutDirty();
        }

#endif

        // Reference from content size fitter
        protected void SetLayoutDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        /// <summary>
        /// Called by the layout system to calculate the horizontal layout size.
        /// Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (!isLayoutGroup)
                return;

            if (fixedCellSize)
            {
                int minColumns = 0;
                int preferredColumns = 0;
                if (m_Constraint == Constraint.FixedColumnCount)
                {
                    minColumns = preferredColumns = m_ConstraintCount;
                }
                else if (m_Constraint == Constraint.FixedRowCount)
                {
                    minColumns = preferredColumns = Mathf.CeilToInt(rectChildren.Count / (float)m_ConstraintCount - 0.001f);
                }
                else
                {
                    minColumns = 1;
                    preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));
                }

                SetLayoutInputForAxis(
                    padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
                    padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
                    -1, 0);
            }
            else
            {
                Vector2 max = TraverseChildren(false);

                SetLayoutInputForAxis(
                    max.x,
                    max.x,
                    -1, 0);
            }
        }

        /// <summary>
        /// Called by the layout system to calculate the vertical layout size.
        /// Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            if (!isLayoutGroup)
                return;

            if (fixedCellSize)
            {
                int minRows = 0;
                if (m_Constraint == Constraint.FixedColumnCount)
                {
                    minRows = Mathf.CeilToInt(rectChildren.Count / (float)m_ConstraintCount - 0.001f);
                }
                else if (m_Constraint == Constraint.FixedRowCount)
                {
                    minRows = m_ConstraintCount;
                }
                else
                {
                    float width = rectTransform.rect.width;
                    int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
                    minRows = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX);
                }

                float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
                SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
            }
            else
            {
                Vector2 max = TraverseChildren(false);

                SetLayoutInputForAxis(
                    max.y,
                    max.y,
                    -1, 1);
            }
        }

        // Reference from content size fitter
        private void HandleSelfFittingAlongAxis(int axis)
        {
            if (!isContentSizeFitter)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
                return;
            }

            m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(rectTransform, axis));
        }

        /// <summary>
        /// Called by the layout system
        /// Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            m_SelfTracker.Clear();
            SetCellsAlongAxis(0);
            HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        /// Called by the layout system
        /// Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
            HandleSelfFittingAlongAxis(1);
        }

        private void CalcCellCount(out int cellCountX, out int cellCountY)
        {
            cellCountX = 1;
            cellCountY = 1;
            if (m_Constraint == Constraint.FixedColumnCount)
            {
                cellCountX = m_ConstraintCount;

                if (rectChildren.Count > cellCountX)
                    cellCountY = rectChildren.Count / cellCountX + (rectChildren.Count % cellCountX > 0 ? 1 : 0);
            }
            else if (m_Constraint == Constraint.FixedRowCount)
            {
                cellCountY = m_ConstraintCount;

                if (rectChildren.Count > cellCountY)
                    cellCountX = rectChildren.Count / cellCountY + (rectChildren.Count % cellCountY > 0 ? 1 : 0);
            }
            else // Flexible
            {
                float width = rectTransform.rect.size.x;
                float height = rectTransform.rect.size.y;

                if (cellSize.x + spacing.x <= 0)
                    cellCountX = int.MaxValue;
                else
                    cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

                if (cellSize.y + spacing.y <= 0)
                    cellCountY = int.MaxValue;
                else
                    cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
            }
        }

        private Vector2 TraverseChildren(bool setChild, bool controlSize = false)
        {
            int cellCountX;
            int cellCountY;
            CalcCellCount(out cellCountX, out cellCountY);

            int cornerX = (int)startCorner % 2;
            int cornerY = (int)startCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            if (startAxis == Axis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
                actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
                actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
            }

            Vector2 requiredSpace = new Vector2(
                actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
                actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
            );
            Vector2 startOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(1, requiredSpace.y)
            );

            Vector2 current = startOffset;
            Vector2 prev = startOffset;
            Vector2 max = (fixedCellSize)? requiredSpace: current;

            Vector2 offset = Vector2.zero;
            Vector2 maxOffset = Vector2.zero;
            int currColCount = 0;

            int axis = (int)startAxis;
            int subAxis = 1 ^ axis;

            // For each children
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var rect = rectChildren[i];
                Vector2 size = rect.sizeDelta;

                if (controlSize)
                {
                    size = new Vector2(LayoutUtility.GetPreferredWidth(rect), LayoutUtility.GetPreferredHeight(rect));
                }

                Vector2 position = new Vector2(padding.left, padding.top) + offset;

                offset[axis] += size[axis] + spacing[axis];
                maxOffset[axis] = Mathf.Max(maxOffset[axis], offset[axis]);
                maxOffset[subAxis] = Mathf.Max(maxOffset[subAxis], offset[subAxis] + size[subAxis]);

                currColCount++;

                if (currColCount >= m_ConstraintCount)
                {
                    offset[axis] = 0;
                    offset[subAxis] = maxOffset[subAxis] + spacing[subAxis];
                    currColCount = 0;
                }

                if (fixedCellSize)
                {
                    size = cellSize;
                }

                if (setChild)
                {
                    if (controlSize)
                    {
                        SetChildAlongAxis(rect, 0, position.x, size[0]);
                        SetChildAlongAxis(rect, 1, position.y, size[1]);
                    }
                    else
                    {
                        SetChildAlongAxis(rect, 0, position.x);
                        SetChildAlongAxis(rect, 1, position.y);
                    }
                }
            }
            if (fixedCellSize)
            {
                return requiredSpace + new Vector2(padding.horizontal, padding.vertical);
            }
            maxOffset[axis] -= spacing[axis];
            return maxOffset + new Vector2(padding.horizontal, padding.vertical);
        }

        private void SetCellsAlongAxis(int axis)
        {
            if (!isLayoutGroup)
                return;

            // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
            // and only vertical values when invoked for the vertical axis.
            // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
            // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
            // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.


            if (axis == 0)
            {
                // Only set the sizes when invoked for horizontal axis, not the positions.
                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform rect = rectChildren[i];

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;

                    if (fixedCellSize)
                    {
                        m_Tracker.Add(this, rect,
                            DrivenTransformProperties.Anchors |
                            DrivenTransformProperties.AnchoredPosition |
                            DrivenTransformProperties.SizeDelta);
                        rect.sizeDelta = cellSize;
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                TraverseChildren(true);
            }
        }

        // Reference form HorizontalOrVerticalLayoutGroup
#if UNITY_EDITOR

        private int m_Capacity = 10;
        private Vector2[] m_Sizes = new Vector2[10];

        protected virtual void Update()
        {
            if (Application.isPlaying)
                return;

            int count = transform.childCount;

            if (count > m_Capacity)
            {
                if (count > m_Capacity * 2)
                    m_Capacity = count;
                else
                    m_Capacity *= 2;

                m_Sizes = new Vector2[m_Capacity];
            }

            // If children size change in editor, update layout (case 945680 - Child GameObjects in a Horizontal/Vertical Layout Group don't display their correct position in the Editor)
            bool dirty = false;
            for (int i = 0; i < count; i++)
            {
                RectTransform t = transform.GetChild(i) as RectTransform;
                if (t != null && t.sizeDelta != m_Sizes[i])
                {
                    dirty = true;
                    m_Sizes[i] = t.sizeDelta;
                }
            }

            if (dirty)
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#endif
    }
}
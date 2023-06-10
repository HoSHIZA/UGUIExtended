using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Layout
{
    [DisallowMultipleComponent]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Flow Layout Group")]
    public class FlowLayoutGroup : LayoutGroup
    {
        public bool ChildForceExpandHeight = false;
        public bool ChildForceExpandWidth = false;
        public float Spacing = 0f;
        
        private float _layoutHeight;
        
        private readonly IList<RectTransform> _rowList = new List<RectTransform>();

        protected bool IsCenterAlign =>
            childAlignment == TextAnchor.LowerCenter || 
            childAlignment == TextAnchor.MiddleCenter || 
            childAlignment == TextAnchor.UpperCenter;

        protected bool IsRightAlign =>
            childAlignment == TextAnchor.LowerRight || 
            childAlignment == TextAnchor.MiddleRight ||
            childAlignment == TextAnchor.UpperRight;

        protected bool IsMiddleAlign =>
            childAlignment == TextAnchor.MiddleLeft ||
            childAlignment == TextAnchor.MiddleRight ||
            childAlignment == TextAnchor.MiddleCenter;

        protected bool IsLowerAlign =>
            childAlignment == TextAnchor.LowerLeft ||
            childAlignment == TextAnchor.LowerRight ||
            childAlignment == TextAnchor.LowerCenter;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            var min = GetGreatestMinimumChildWidth() + padding.left + padding.right;
            
            SetLayoutInputForAxis(min, -1, -1, 0);
        }

        public override void SetLayoutHorizontal()
        {
            SetLayout(0, false);
        }

        public override void SetLayoutVertical()
        {
            SetLayout(1, false);
        }

        public override void CalculateLayoutInputVertical()
        {
            _layoutHeight = SetLayout(1, true);
        }
        
        public float SetLayout(int axis, bool layoutInput)
        {
            var groupHeight = rectTransform.rect.height;
            var workingWidth = rectTransform.rect.width - padding.left - padding.right;
            var yOffset = IsLowerAlign ? padding.bottom : (float) padding.top;

            var currentRowWidth = 0f;
            var currentRowHeight = 0f;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var index = IsLowerAlign ? rectChildren.Count - 1 - i : i;
                var child = rectChildren[index];
                var childWidth = LayoutUtility.GetPreferredSize(child, 0);
                var childHeight = LayoutUtility.GetPreferredSize(child, 1);

                childWidth = Mathf.Min(childWidth, workingWidth);

                if (_rowList.Count > 0)
                {
                    currentRowWidth += Spacing;
                }

                if (currentRowWidth + childWidth > workingWidth)
                {
                    currentRowWidth -= Spacing;

                    if (!layoutInput)
                    {
                        var h = CalculateRowVerticalOffset(groupHeight, yOffset, currentRowHeight);
                        LayoutRow(currentRowWidth, currentRowHeight, workingWidth, padding.left, h, axis);
                    }

                    _rowList.Clear();

                    yOffset += currentRowHeight;
                    yOffset += Spacing;

                    currentRowHeight = 0;
                    currentRowWidth = 0;
                }

                currentRowWidth += childWidth;
                _rowList.Add(child);

                if (childHeight > currentRowHeight)
                {
                    currentRowHeight = childHeight;
                }
            }

            if (!layoutInput)
            {
                var h = CalculateRowVerticalOffset(groupHeight, yOffset, currentRowHeight);
                
                LayoutRow(currentRowWidth, currentRowHeight, workingWidth, padding.left, h, axis);
            }
            
            _rowList.Clear();
            
            yOffset += currentRowHeight;
            yOffset += IsLowerAlign ? padding.top : padding.bottom;
            
            if (layoutInput)
            {
                if (axis == 1)
                {
                    SetLayoutInputForAxis(yOffset, yOffset, -1, axis);
                }
            }

            return yOffset;
        }

        private float CalculateRowVerticalOffset(float groupHeight, float yOffset, float currentRowHeight)
        {
            float h;

            if (IsLowerAlign)
            {
                h = groupHeight - yOffset - currentRowHeight;
            }
            else if (IsMiddleAlign)
            {
                h = groupHeight * 0.5f - _layoutHeight * 0.5f + yOffset;
            }
            else
            {
                h = yOffset;
            }

            return h;
        }

        protected void LayoutRow(float rowWidth, float rowHeight, float maxWidth, float xOffset, float yOffset, int axis)
        {
            var xPos = xOffset;

            if (!ChildForceExpandWidth && IsCenterAlign)
            {
                xPos += (maxWidth - rowWidth) * 0.5f;
            }
            else if (!ChildForceExpandWidth && IsRightAlign)
            {
                xPos += (maxWidth - rowWidth);
            }

            var extraWidth = 0f;

            if (ChildForceExpandWidth)
            {
                var flexibleChildCount = _rowList.Count(row => LayoutUtility.GetFlexibleWidth(row) > 0f);

                if (flexibleChildCount > 0)
                {
                    extraWidth = (maxWidth - rowWidth) / flexibleChildCount;
                }
            }

            for (var j = 0; j < _rowList.Count; j++)
            {
                var index = IsLowerAlign ? _rowList.Count - 1 - j : j;

                var rowChild = _rowList[index];

                var rowChildWidth = LayoutUtility.GetPreferredSize(rowChild, 0);

                if (LayoutUtility.GetFlexibleWidth(rowChild) > 0f)
                {
                    rowChildWidth += extraWidth;
                }

                var rowChildHeight = LayoutUtility.GetPreferredSize(rowChild, 1);

                if (ChildForceExpandHeight)
                {
                    rowChildHeight = rowHeight;
                }

                rowChildWidth = Mathf.Min(rowChildWidth, maxWidth);

                var yPos = yOffset;

                if (IsMiddleAlign)
                {
                    yPos += (rowHeight - rowChildHeight) * 0.5f;
                }
                else if (IsLowerAlign)
                {
                    yPos += (rowHeight - rowChildHeight);
                }

                if (axis == 0)
                {
                    SetChildAlongAxis(rowChild, 0, xPos, rowChildWidth);
                }
                else
                {
                    SetChildAlongAxis(rowChild, 1, yPos, rowChildHeight);
                }

                xPos += rowChildWidth + Spacing;
            }
        }

        public float GetGreatestMinimumChildWidth()
        {
            return rectChildren
                .Select(LayoutUtility.GetMinWidth)
                .Aggregate(0f, (current, min) => Mathf.Max(min, current));
        }
    }
}
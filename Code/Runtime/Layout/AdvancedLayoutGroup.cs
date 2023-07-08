using UnityEngine;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Layout
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Advanced Size Fitter")]
    public class AdvancedLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        [SerializeField] private LayoutGroupType _type;

        private bool IsVertical => _type == LayoutGroupType.Vertical;
        private bool IsHorizontal => _type == LayoutGroupType.Horizontal;
        
        protected AdvancedLayoutGroup()
        {}
        
        public override void CalculateLayoutInputHorizontal()
        {
            if (_type == LayoutGroupType.Disabled) return;
            
            base.CalculateLayoutInputHorizontal();
            
            CalcAlongAxis(0, true);
        }
        
        public override void CalculateLayoutInputVertical()
        {
            if (_type == LayoutGroupType.Disabled) return;
            
            CalcAlongAxis(1, true);
        }
        
        public override void SetLayoutHorizontal()
        {
            if (_type == LayoutGroupType.Disabled) return;
            
            SetChildrenAlongAxis(0, true);
        }
        
        public override void SetLayoutVertical()
        {
            if (_type == LayoutGroupType.Disabled) return;
            
            SetChildrenAlongAxis(1, true);
        }
        
        private enum LayoutGroupType
        {
            Disabled,
            Vertical,
            Horizontal,
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Layout
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GridLayoutGroup))]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Fit Grid Layout Cell")]
    public sealed class FitGridLayoutCellSize : UIBehaviour
    {
        [SerializeField] private Axis _axis;
        [SerializeField] private RatioMode _ratioMode;
        [SerializeField] private float _preferredCellSize;
        
        private GridLayoutGroup _grid;
        
        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform = _rectTransform 
            ? _rectTransform 
            : transform as RectTransform;

        protected override void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
        }

        protected override void Start()
        {
            UpdateCell();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateCell();
        }

        private void UpdateCell()
        {
            if (_axis == Axis.X)
            {
                if (_grid.constraint == GridLayoutGroup.Constraint.Flexible)
                {
                    _grid.constraintCount = (int) (RectTransform.rect.width / _preferredCellSize);
                }
                
                var spacing = (_grid.constraintCount - 1) * _grid.spacing.x;
                var contentSize = RectTransform.rect.width - _grid.padding.left - _grid.padding.right - spacing;
                var sizePerCell = contentSize / _grid.constraintCount;

                _grid.cellSize = new Vector2(sizePerCell, _ratioMode == RatioMode.Free ? _grid.cellSize.y : sizePerCell);
            }
            else
            {
                if (_grid.constraint == GridLayoutGroup.Constraint.Flexible)
                {
                    _grid.constraintCount = (int) (RectTransform.rect.height / _preferredCellSize);
                }
                
                var spacing = (_grid.constraintCount - 1) * _grid.spacing.y;
                var contentSize = RectTransform.rect.height - _grid.padding.top - _grid.padding.bottom - spacing;
                var sizePerCell = contentSize / _grid.constraintCount;
                
                _grid.cellSize = new Vector2(_ratioMode == RatioMode.Free ? _grid.cellSize.x : sizePerCell, sizePerCell);
            }
        }

#if UNITY_EDITOR
        [ExecuteAlways]
        private void Update()
        {
            _grid = GetComponent<GridLayoutGroup>();
            
            UpdateCell();
        }
#endif

        private enum Axis
        {
            X,
            Y,
        }

        private enum RatioMode
        {
            Free,
            Fixed,
        }
    }
}
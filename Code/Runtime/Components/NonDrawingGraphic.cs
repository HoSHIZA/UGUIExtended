using UnityEngine;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Non Drawing Graphic")]
    public class NonDrawingGraphic : MaskableGraphic
    {
        public override void SetMaterialDirty()
        {
        }

        public override void SetVerticesDirty()
        {
        }

        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryPanel : ConsoleNodeHolderInstance {

        public Panel panel;
        protected float spacing = 2f;

        public void Set(Panel panel) {
            this.panel = panel;
            spacing = childsContainer.GetComponent<VerticalLayoutGroup>().spacing;
            // Ensure the panel is the same size as the parent
            rT.offsetMin = new Vector2(0, 0);
            rT.offsetMax = new Vector2(0, 0);
        }

        public void AddMinorPanel(GameObject gameObject) {
            rT.offsetMax = new Vector2(0, -panel.minors.Count * (spacing + Console.RowHeight));
        }

        /// <summary>
        /// Changes the size of the RectTransform based on state and number of child Nodes.
        /// Force to update the rect layout to display the VerticalLayoutGroups correctly.
        /// </summary>
        protected override void UpdateSize() {
            float totalHeight = 0f;
            for (int n = 0; n < panel.nodes.Count; ++n)
                if(panel.nodes[n].instance.gameObject.activeSelf)
                    totalHeight += panel.nodes[n].instance.GetHeight() + spacing;
            childsContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
            LayoutRebuilder.MarkLayoutForRebuild(childsContainer);
        }
    }
}
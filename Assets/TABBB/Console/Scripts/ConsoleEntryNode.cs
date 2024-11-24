using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNode : ConsoleNodeHolderInstance {

        public Image expandButton;
        public TMP_Text tmpName;
        public delegate void DelegateNode(ConsoleEntryNode node);
        public event DelegateNode OnUpdateSize;
        public event DelegateNode OnDestroy;

        public Node node;
        protected bool collapsed;
        protected float baseHeight = 30f;
        protected float spacing = 2f;

        private void Start() {
            // Starting without the Display GameObject active in the hierarchy may cause the 
            // Nodes to have (0,0,0) scale, we force it to (1,1,1) to fix it.
            rT.localScale = Vector3.one;
            SetUp();
        }

        /// <summary>
        /// Used to apply the customization on every Node prefab.
        /// </summary>
        protected virtual void SetUp() {
            expandButton.color = Console.Foreground;
            tmpName.color = Console.Foreground;
            tmpName.font = Console.Font;
            rT.GetChild(0).GetComponent<Image>().color = Console.Dark;
        }

        protected virtual void SetNode(Node node) {
            spacing = childsContainer.GetComponent<VerticalLayoutGroup>().spacing;
            this.node = node;
        }

        /// <summary>
        /// Set base Node value and default collapse status.
        /// Must only be called after instantiating. 
        /// </summary>
        public void SetNode(Node node, bool collapsed, float baseHeight) {
            this.baseHeight = baseHeight;
            SetNode(node);
            this.collapsed = collapsed;
            tmpName.text = node.name;
            UpdateValue();
            //no need to evaluate the state of expand and container status
            childsContainer.gameObject.SetActive(false);
            expandButton.enabled = false;
        }
        public virtual void UpdateValue() { }

        public override void AddNode(Node node) {
            expandButton.enabled = true;
            node.instance.rT.SetParent(childsContainer);
            node.instance.OnUpdateSize += ChildNode_OnUpdateSize;
            node.instance.OnDestroy += ChildNode_OnDestroy;
            if (!collapsed) {
                childsContainer.gameObject.SetActive(true);
                UpdateSize();
            }
        }

        /// <summary>
        /// Assigned to the Expand Button in the prefab.
        /// </summary>
        public void Callback() {
            ToggleCollapse();
        }
        /// <summary>
        /// Expands or collapses the Node based on its current state.
        /// </summary>
        protected void ToggleCollapse() {
            if (node.nodes.Count == 0) return;
            collapsed = !collapsed;
            childsContainer.gameObject.SetActive(!collapsed);
            UpdateSize();
        }

        /// <summary>
        /// Changes the size of the RectTransform based on state and number of child Nodes.
        /// </summary>
        protected override void UpdateSize() {
            float totalHeight = GetHeight();
            float childContainerHeight = totalHeight - baseHeight;
            rT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
            childsContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childContainerHeight);
            UpdateSize(this);
        }

        // We use OnUpdateSize and OnDestroy events so the parents up to the Panel 
        // can resize accordingly
        protected void UpdateSize(ConsoleEntryNode node) {
            OnUpdateSize?.Invoke(node);
        }
        public void Destroy() {
            OnDestroy?.Invoke(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Returns the height of the Node considering its child Nodes.
        /// </summary>
        public virtual float GetHeight() {
            float height = baseHeight;
            if (!collapsed && node.nodes.Count > 0)
                height += spacing + GetChildsHeight();
            return height;
        }
        /// <summary>
        /// Returns the total accumulative height of the child Nodes.
        /// </summary>
        protected virtual float GetChildsHeight() {
            float height = spacing * (node.nodes.Count - 1);
                for (int n = 0; n < node.nodes.Count; ++n)
                    height += node.nodes[n].instance.GetHeight();
            return height;
        }
    }
}
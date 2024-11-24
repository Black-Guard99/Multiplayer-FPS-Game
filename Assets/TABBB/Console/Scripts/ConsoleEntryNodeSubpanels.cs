using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {

    public class ConsoleEntryNodeSubpanels : ConsoleEntryNode {

        public GameObject buttonPrefab;
        public RectTransform buttonsParent;

        public new ValueNode<SubpanelValue> node;
        ConsolePanelButton[] buttons;
        List<Node>[] tabNodes;
        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<SubpanelValue>;
            tabNodes = new List<Node>[this.node.value.names.Length];
            buttons = new ConsolePanelButton[this.node.value.names.Length];
            selectedIndex = 0;
            addNodeIndex = 0;
            for (int n = 0; n < this.node.value.names.Length; ++n) {
                ConsolePanelButton button = Instantiate(buttonPrefab, buttonsParent).GetComponent<ConsolePanelButton>();
                int index = n;
                button.Set(this.node.value.names[n], delegate { Callback_Tab(index); });
                buttons[n] = button;
                tabNodes[n] = new List<Node>();
                buttons[n].Highlight(n == 0);
            }
            buttonsParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        }

        int addNodeIndex;
        bool ignoreExt;
        /// <summary>
        /// Selects the tab index used to add the new panels through the Console generic method.
        /// </summary>
        public void PreSelectIndex(int tabIndex) {
            addNodeIndex = tabIndex;
        }
        public override void AddNode(Node node) {
            // with this we avoid calling twice AddNode(int, Node) since AddNode<T> calls the current 
            // method, redirected from Console
            if (ignoreExt) {
                ignoreExt = false;
                return;
            }
            AddNode(addNodeIndex, node);
        }
        /// <summary>
        /// Works in the same way as Console.AddNode but selecting the tab the Node goes into.
        /// </summary>
        public ValueNode<T> AddNode<T>(int tabIndex, string name, T value, ValueNode<T>.DelegateObject callback = null) {
            ignoreExt = true;
            ValueNode<T> node = Console.I.AddNode(this.node, name, value, callback);
            AddNode(tabIndex, node);
            return node;
        }

        /// <summary>
        /// Adds the node to the specified tab.
        /// </summary>
        void AddNode(int tabIndex, Node node) {
            tabNodes[tabIndex].Add(node);
            expandButton.enabled = true;
            node.instance.rT.SetParent(childsContainer);
            node.instance.OnUpdateSize += ChildNode_OnUpdateSize;
            node.instance.OnDestroy += ChildNode_OnDestroy;
            node.instance.gameObject.SetActive(tabIndex == selectedIndex);
            if (!collapsed && tabIndex == selectedIndex) {
                childsContainer.gameObject.SetActive(true);
                UpdateSize();
            }
        }

        /// <summary>
        /// Need to remove the deleted node from the node lists of each tab.
        /// </summary>
        protected override void ChildNode_OnDestroy(ConsoleEntryNode node) {
            for (int n = 0; n < tabNodes.Length; ++n) {
                if (tabNodes[n].Remove(node.node))
                    break;
            }
            base.ChildNode_OnDestroy(node);
        }

        int selectedIndex;
        /// <summary>
        /// Assigned to the Buttons to select what tab to display.
        /// </summary>
        /// <param name="index"></param>
        public void Callback_Tab(int index) {
            if (selectedIndex == index) return;
            selectedIndex = index;
            ShowPanel(index);
        }

        /// <summary>
        /// Sets to active only the Nodes of the specified tab.
        /// </summary>
        /// <param name="index">Index of the tab.</param>
        void ShowPanel(int index) {
            for (int n = 0; n < tabNodes.Length; ++n) {
                bool b = n == index;
                buttons[n].Highlight(b);
                for (int i = 0; i < tabNodes[n].Count; ++i) {
                    tabNodes[n][i].instance.gameObject.SetActive(b);
                }
            }
            UpdateSize();
        }
        protected override void UpdateSize() {
            float height = baseHeight; // Name + buttons row
            if (!collapsed) {
                height += baseHeight + spacing * 2;
                for (int i = 0; i < tabNodes[selectedIndex].Count; ++i) {
                    height += tabNodes[selectedIndex][i].instance.GetHeight();
                }
            }
            UpdateSize(height);
        }
        void UpdateSize(float totalHeight) {
            rT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
            childsContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight - (baseHeight * 2f + spacing));
            UpdateSize(this);
        }
    }
}
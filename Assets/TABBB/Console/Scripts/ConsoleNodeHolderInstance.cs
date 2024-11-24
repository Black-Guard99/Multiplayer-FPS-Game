using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {

    /// <summary>
    /// Template to inherit from any class having Node childs.
    /// </summary>
    public class ConsoleNodeHolderInstance : MonoBehaviour {
        public RectTransform rT; // Self reference
        public RectTransform childsContainer;
        public virtual void AddNode(Node node) {
            node.instance.transform.SetParent(childsContainer);
            // Resize in case any child node changes in size or is removed.
            node.instance.OnUpdateSize += ChildNode_OnUpdateSize;
            node.instance.OnDestroy += ChildNode_OnDestroy;
            UpdateSize();
        }

        public void ForceUpdate() {
            UpdateSize();
        }

        /// <summary>
        /// Changes the size of the RectTrasnform based on state and number of child Nodes.
        /// </summary>
        protected virtual void UpdateSize() { }

        protected virtual void ChildNode_OnUpdateSize(ConsoleEntryNode node) {
            UpdateSize();
        }
        protected virtual void ChildNode_OnDestroy(ConsoleEntryNode node) {
            node.OnUpdateSize -= ChildNode_OnUpdateSize;
            node.OnDestroy -= ChildNode_OnDestroy;
            UpdateSize();
        }
    }
}
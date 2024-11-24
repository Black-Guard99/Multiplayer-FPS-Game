using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeDisplay : ConsoleEntryNode {
        public TMP_Text tmpValue;
        public new ValueNode<string> node;
        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<string>;
        }
        protected override void SetUp() {
            base.SetUp();
            tmpValue.color = Console.Foreground;
            tmpValue.font = Console.Font;
        }
        public override void UpdateValue() {
            tmpValue.text = node.value;
        }
    }
}
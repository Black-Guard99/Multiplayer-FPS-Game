using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeButton : ConsoleEntryNode {

        public new ValueNode<ButtonValue> node;
        public TMP_Text tmpValue;
        public Button button;

        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<ButtonValue>;
        }
        protected override void SetUp() {
            base.SetUp();
            tmpValue.color = Console.ButtonForeground;
            tmpValue.font = Console.Font;
            button.colors = Console.ButtonColors;
        }

        // Change button label
        public override void UpdateValue() {
            tmpValue.text = node.value.label;
        }

        // Assigned to the prefab Button
        public void Callback_Button() {
            node.SetValueFromConsole(node.value);
        }

    }
}
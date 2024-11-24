using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeInputField : ConsoleEntryNode {

        public TMPro.TMP_InputField inputField;
        public new ValueNode<InputValue> node;

        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<InputValue>;
        }
        protected override void SetUp() {
            base.SetUp();
            inputField.fontAsset = Console.Font;
            inputField.textComponent.color = Console.Foreground;
            inputField.textComponent.font = Console.Font;
        }
        public override void UpdateValue() {
            inputField.text = node.value.value;
        }
        public void Callback_UpdateInputField() {
            node.value.value = inputField.text;
            node.SetValueFromConsole(node.value);
        }

    }
}
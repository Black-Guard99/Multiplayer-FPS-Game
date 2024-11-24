using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeVector3 : ConsoleEntryNode {

        public TMPro.TMP_InputField inputFieldX, inputFieldY, inputFieldZ;
        public Image frameX, frameY, frameZ;
        public new ValueNode<Vector3Value> node;

        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<Vector3Value>;
        }
        protected override void SetUp() {
            base.SetUp();
            inputFieldX.fontAsset = Console.Font;
            inputFieldX.textComponent.color = Console.Foreground;
            inputFieldX.textComponent.font = Console.Font;
            inputFieldY.fontAsset = Console.Font;
            inputFieldY.textComponent.color = Console.Foreground;
            inputFieldY.textComponent.font = Console.Font;
            inputFieldZ.fontAsset = Console.Font;
            inputFieldZ.textComponent.color = Console.Foreground;
            inputFieldZ.textComponent.font = Console.Font;
            frameX.color = Console.Foreground;
            frameY.color = Console.Foreground;
            frameZ.color = Console.Foreground;
        }

        public override void UpdateValue() {
            if (held) return;
            inputFieldX.text = node.value.value.x.ToString(node.value.format);
            inputFieldY.text = node.value.value.y.ToString(node.value.format);
            inputFieldZ.text = node.value.value.z.ToString(node.value.format);
        }

        // What we do is block the update of the value display of all values of the vector whenever 
        // it is selected, that way it can be edited easily even if the original value is changing.
        // When it is deselected or the editing is finished, we stop blocking the update of values.
        bool held;
        public void Callback_Select() {
            held = true;
        }
        public void Callback_Deselect() {
            held = false;
        }
        public void Callback_End() {
            held = false;
            node.value.value = new Vector3(float.Parse(inputFieldX.text), float.Parse(inputFieldY.text), float.Parse(inputFieldZ.text));
            node.SetValueFromConsole(node.value);
        }
    }
}
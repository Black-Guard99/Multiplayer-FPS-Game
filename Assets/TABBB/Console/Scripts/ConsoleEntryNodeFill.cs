using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeFill : ConsoleEntryNode {

        public TMP_Text tmpValue;
        public Image fill, frame;
        public new ValueNode<FillValue> node;

        protected override void SetUp() {
            base.SetUp();
            tmpValue.color = Console.Foreground;
            tmpValue.font = Console.Font;
            frame.color = Console.Foreground;
        }

        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<FillValue>;
            tmpValue.gameObject.SetActive(this.node.value.displayValue);
            UpdateValue();
        }

        public override void UpdateValue() {
            float f = (node.value.value - node.value.min) / (node.value.max - node.value.min);
            fill.fillAmount = f;
            fill.color = Console.FillBarGradient.Evaluate(f);
            tmpValue.text = "[" + node.value.value.ToString(node.value.format) + "/" + node.value.max.ToString(node.value.format) + "]";
        }
    }
}
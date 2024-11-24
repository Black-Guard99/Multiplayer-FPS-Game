using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeToggle : ConsoleEntryNode {
        public new ValueNode<bool> node;
        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<bool>;
            UpdateValue();
        }

        public RectTransform slider;
        public RectTransform offPosition, onPosition;
        public Image background;

        public void Callback_Toggle() {
            node.SetValueFromConsole(!node.value);
            UpdateValue();
        }

        public override void UpdateValue() {
            RectTransform target = offPosition;
            Color color = Console.Medium;
            bool on = node.value;
            if (on) {
                target = onPosition;
                color = Console.Light;
            }
            slider.position = target.position;
            background.color = color;

            //expande/collapse when alternating states
            if ((on && collapsed) || (!on && !collapsed))
                ToggleCollapse();
        }

    }
}
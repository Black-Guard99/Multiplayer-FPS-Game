using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryNodeSlider : ConsoleEntryNode {

        public TMP_Text tmpValue;
        public Image handle, fill, background;
        public Slider slider;
        public new ValueNode<SliderValue> node;

        protected override void SetNode(Node node) {
            base.SetNode(node);
            this.node = node as ValueNode<SliderValue>;
            UpdateValue();
        }
        protected override void SetUp() {
            base.SetUp();
            tmpValue.color = Console.Foreground;
            tmpValue.font = Console.Font;
            handle.color = Console.Foreground;
            fill.color = Console.Light;
            background.color = Console.Medium;
        }

        public override void UpdateValue() {
            tmpValue.text = 
                "[" + node.value.min.ToString(node.value.format) + " \t" + 
                node.value.value.ToString(node.value.format) + " \t" + 
                node.value.max.ToString(node.value.format) + "]";
            slider.minValue = node.value.min;
            slider.maxValue = node.value.max;
            slider.value = node.value.value;
            slider.wholeNumbers = node.value.onlyInts;
        }
        public void Callback_SliderChange(float f) {
            node.value.value = f;
            UpdateValue();
            node.SetValueFromConsole(node.value);
        }

    }
}
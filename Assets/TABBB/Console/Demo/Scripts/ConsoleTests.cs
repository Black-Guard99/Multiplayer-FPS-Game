using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {
    public class ConsoleTests : MonoBehaviour {

        ValueNode<string> nodeTime;
        ValueNode<string> nodeFrameRate, nodeFrameRateAvg;

        void Start() {
            // ### CREATE & DESTROY PANEL EXAMPLE ###
            // Create first panel to test delete
            // Remove a panel searching by name
            Console.I.AddPanel("First Panel");
            Console.I.RemovePanel("First Panel");

            // ### TYPES OF NODE EXAMPLE ###
            // Create second panel with all the elements
            Panel panel = Console.I.AddPanel("Second Panel");
            // Basic text display
            panel.AddInfo("Fixed Value", "Hello World");
            nodeTime = panel.AddInfo("Time value", "0");
            nodeFrameRate = panel.AddInfo("Frame rate", "0");
            nodeFrameRateAvg = panel.AddInfo("Frame rate avg", "0");
            panel.AddInputField("Input Field", new InputValue("Initial"), InputFieldCallback);
            // Toggles expand and collapse their children by default, similar to the idea of toggling a feature and all 
            // the variables associated to it becoming irrelevant
            Node toggleNode = panel.AddToggle("Toggle", false, ToggleCallback);
            toggleNode.AddInfo("Toggle group", "#1");
            toggleNode.AddInfo("Toggle group", "#2");
            toggleNode.AddInfo("Toggle group", "#3");
            toggleNode.AddInfo("Toggle group", "#4");
            panel.AddSlider("Slider Example", -2f, 4f, 0f, SliderCallback);
            // Filled bars use a color code red-yellow-green based on fill amount (defined on palette)
            Node nodeHPBars = panel.AddInfo("Enemy Health Bars", "");
            nodeHPBars.AddFillBar("Enemy #1", 0f, 100f, 10);
            nodeHPBars.AddFillBar("Enemy #2", 0f, 100f, 30);
            nodeHPBars.AddFillBar("Enemy #3", 0f, 100f, 50);
            nodeHPBars.AddFillBar("Enemy #4", 0f, 100f, 70);
            nodeHPBars.AddFillBar("Enemy #5", 0f, 100f, 90);
            nodeHPBars.AddFillBar("Enemy #6", 0f, 100f, 100);

            panel.AddButton("Button", "Press", ButtonCallback);
            panel.AddVector2("Vector2", new Vector2(Random.value, Random.value), Vector2Callback);
            panel.AddVector3("Vector3", new Vector3(Random.value, Random.value, Random.value), Vector3Callback);

            // ### DEPTH OF NODE EXAMPLE & DELETING NODES EXAMPLE ###
            // Create third panel not being focused
            // This panel has "Depth 3"
            // The first "Depth 3" group is created expanded by default
            // The second group is set to be collapsed by default
            Console.I.AddPanel("Third Panel", false);
            Console.I.AddNode("Third Panel", "Base Node", "#1");
            Console.I.AddNode("Third Panel", "Base Node", "#2");
            Console.I.AddNode("Third Panel", "Base Node", "#3");
            Console.I.AddNode("Third Panel", "Base Node", "#4");
            Console.I.AddNode("Third Panel/Base Node", "Depth 2_0", "#1_1");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_0", "Depth 3", "#1_1_1");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_0", "Depth 3", "#1_1_2");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_0", "Depth 3_0", "#1_1_3");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_0", "Depth 3", "#1_1_4");
            Node deletedNode = Console.I.AddNode("Third Panel/Base Node/Depth 2_0", "Depth 3", "#1_1_5");
            Console.I.AddNode("Third Panel/Base Node", "Depth 2_1", "#1_2", null, true);
            Console.I.AddNode("Third Panel/Base Node/Depth 2_1", "Depth 3", "#1_2_1");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_1", "Depth 3", "#1_2_2");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_1", "Depth 3_0", "#1_2_3");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_1", "Depth 3", "#1_2_4");
            Console.I.AddNode("Third Panel/Base Node/Depth 2_1", "Depth 3", "#1_2_5");
            // Test to Remove two Node, found by name/path and then by reference
            Console.I.RemoveNode("Third Panel/Base Node/Depth 2_1/Depth 3_0");
            Console.I.RemoveNode(deletedNode);
        }

        void InputFieldCallback(InputValue s) {
            print("New input field value:" + s.value);
        }
        void ToggleCallback(bool value) {
            print("New toggle value:" + value);
        }
        private void SliderCallback(SliderValue t) {
            print("New slider value: " + t.value);
        }
        private void ButtonCallback(ButtonValue t) {
            print("Button Callback");
        }
        private void Vector2Callback(Vector2Value t) {
            print("New Vector2 value: " + t.value);
        }
        private void Vector3Callback(Vector3Value t) {
            print("New Vector3 value: " + t.value);
        }

        bool lastFrame;
        float[] frameTimes = new float[30];
        int frameIndex = 0;
        void Update() {
            float delta = Time.unscaledDeltaTime;
            frameTimes[frameIndex % 30] = delta;
            ++frameIndex;
            float s = 0f;
            for (int n = 0; n < 30; ++n)
                s += frameTimes[n];
            s /= 30;

            nodeTime.SetValue(Time.time.ToString());
            nodeFrameRate.SetValue((1f / delta).ToString("0.00"));
            nodeFrameRateAvg.SetValue((1f / s).ToString("0.00"));
        }
    }
}

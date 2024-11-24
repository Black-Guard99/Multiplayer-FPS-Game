using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {
    public class ConsoleDemo : MonoBehaviour {

        public Transform player;
        float planeLimit = 4.75f;

        public Transform[] pilars;
        ValueNode<string>[] pilarNodes;
        
        void Start() {
            // This messages will output in the UnityEditor Console and the Logs panel of this Console
            Debug.Log("This is a normal message");
            Debug.LogWarning("This is a warning message");
            Debug.LogError("This is an error message");

            // Create a new panel called "Demo Panel" and automatically select it
            Panel panel = Console.I.AddPanel("Demo Panel", true);
            // Create one group displaying the distances from the Player (Capsule) to 
            // all pilars (Cilinders) and create second group of buttons, 
            // when pressed, they move the pilars to a random position inside the plane
            pilarNodes = new ValueNode<string>[pilars.Length];
            Node distancesNode = panel.AddInfo("Distances", "");
            Node actionNode = panel.AddInfo("Actions", "");
            for (int n = 0; n < pilars.Length; ++n) {
                pilarNodes[n] = distancesNode.AddInfo("Pilar #" + n, "");
                int index = n;
                Console.I.AddNode(actionNode, "Pilar #" + n, new ButtonValue("Move"), delegate (ButtonValue b) { MovePilar(index); });
            }

            // Create a third group that allows us to modify the player parameters
            // a slider for its speed and a Vector3 for its position
            Node playerNode = panel.AddInfo("Player", "");
            playerNode.AddSlider("Speed", 0.1f, 7f, playerSpeed, PlayerSpeedSlider);
            playerDestination = player.position;
            playerDestinationNode = playerNode.AddVector3("Destination", playerDestination, PlayerDestinationV3);

            // Lastly a generic example of how to create a subpanel system
            ConsoleEntryNodeSubpanels subpanel = Console.I.AddNode(panel, "SubPanel 1", new SubpanelValue("Panel 1", "Panel 2", "Panel 3")).instance as ConsoleEntryNodeSubpanels;
            subpanel.AddNode(0, "Fill", new FillValue(0, 1, Random.value));
            subpanel.AddNode(0, "Fill", new FillValue(0, 1, Random.value));
            subpanel.AddNode(0, "Fill", new FillValue(0, 1, Random.value));
            subpanel.AddNode(0, "Fill", new FillValue(0, 1, Random.value));
            subpanel.AddNode(0, "Fill", new FillValue(0, 1, Random.value));
            subpanel.AddNode(1, "Button", new ButtonValue("Press me"), Callback_Button);
            subpanel.AddNode(1, "Button", new ButtonValue("Press me"), Callback_Button);
            subpanel.AddNode(2, "Toggle", true, Callback_Toggle);
            subpanel.AddNode(2, "Toggle", false, Callback_Toggle);
            subpanel.AddNode(2, "Toggle", true, Callback_Toggle);
            subpanel.AddNode(2, "Toggle", false, Callback_Toggle);
            // And how to create the exact same subpanel system, but with the generic calls
            // Before adding a new node, we need to preselect the tab where the nodes will be created
            Node subpanelNode = panel.AddSubpanel("SubPanel 2", new SubpanelValue("Panel 1", "Panel 2", "Panel 3"), out subpanel);
            subpanel.PreSelectIndex(0);
            subpanelNode.AddFillBar("Fill", 0, 1, Random.value);
            subpanelNode.AddFillBar("Fill", 0, 1, Random.value);
            subpanelNode.AddFillBar("Fill", 0, 1, Random.value);
            subpanelNode.AddFillBar("Fill", 0, 1, Random.value);
            subpanelNode.AddFillBar("Fill", 0, 1, Random.value);
            subpanel.PreSelectIndex(1);
            subpanelNode.AddButton("Button", "Press me", Callback_Button);
            subpanelNode.AddButton("Button", "Press me", Callback_Button);
            subpanel.PreSelectIndex(2);
            subpanelNode.AddToggle("Toggle", true, Callback_Toggle);
            subpanelNode.AddToggle("Toggle", false, Callback_Toggle);
            subpanelNode.AddToggle("Toggle", true, Callback_Toggle);
            subpanelNode.AddToggle("Toggle", false, Callback_Toggle);
        }

        ValueNode<Vector3Value> playerDestinationNode;
        float playerSpeed = 2f;
        Vector3 playerDestination;
        private void PlayerSpeedSlider(SliderValue t) {
            playerSpeed = t.value;
        }
        private void PlayerDestinationV3(Vector3Value v) {
            v.value.y = 0.594f; //we cap the vertical position
            v.value.x = Mathf.Clamp(v.value.x, -planeLimit, planeLimit); //and limit the XZ inside the plane
            v.value.z = Mathf.Clamp(v.value.z, -planeLimit, planeLimit);
            playerDestination = v.value;
            playerDestinationNode.SetValue(v);
        }
        private void MovePilar(int index) {
            pilars[index].position = new Vector3(Random.Range(-planeLimit, planeLimit), 0.5f, Random.Range(-planeLimit, planeLimit));
        }

        private void Callback_Button(ButtonValue t) {
            print("I'm the Subpanel BUTTON message!");
        }
        private void Callback_Toggle(bool t) {
            print("I'm the Subpanel TOGGLE message!");
        }

        // Update the distance values and move the player
        void Update() {
            Vector3 pos = Vector3.Lerp(player.position, playerDestination, Time.deltaTime * playerSpeed);
            player.position = pos;
            for (int n = 0; n < pilars.Length; ++n) {
                pilarNodes[n].SetValue(Vector3.Distance(pos, pilars[n].position).ToString());
            }
        }

    }
}
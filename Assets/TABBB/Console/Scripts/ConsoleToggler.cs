using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {

    /// <summary>
    /// Toggles the Console state. Uses old Input System by default.
    /// </summary>
    public class ConsoleToggler : MonoBehaviour {

        bool lastFrame;
        void Update() {
            // When Q, W and E are pressed down we alternate the active state of display
            if (Input.GetKey(KeyCode.Tab)) {
                //if (Keyboard.current.qKey.isPressed && Keyboard.current.wKey.isPressed && Keyboard.current.eKey.isPressed) {
                if (lastFrame) return;
                lastFrame = true;
                Console.I.Toggle();
            } else {
                lastFrame = false;
            }
        }
    }
}
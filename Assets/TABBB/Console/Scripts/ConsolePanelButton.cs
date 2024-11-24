using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {

    /// <summary>
    /// Generic button behaviour, changes its width based on its label content and redirects 
    /// the Button callback method to a runtime assigned one.
    /// </summary>
    public class ConsolePanelButton : MonoBehaviour {

        public RectTransform rT;
        public TMP_Text tmp;
        public Image highlight;
        public Button button;

        private void Start() {
            highlight.color = Console.Foreground;
            tmp.color = Console.ButtonForeground;
            tmp.font = Console.Font;
            button.colors = Console.ButtonColors;
        }

        // This callback is assigned through Set and triggered through the Button callback
        public System.Action callback;
        // This callback is called by the Button component in Scene
        public void Callback() {
            callback();
        }
        /// <summary>
        /// Sets the necessary information for the button to function.
        /// </summary>
        /// <param name="name">Content displayed in the TMP Text component.</param>
        /// <param name="callback">Action triggered when pressing the button.</param>
        public void Set(string name, System.Action callback) {
            this.callback = callback;
            tmp.text = name;
            rT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tmp.preferredWidth + 10f);
        }

        /// <summary>
        /// Sets the highlight state of the button.
        /// </summary>
        public void Highlight(bool on) {
            highlight.gameObject.SetActive(on);
        }
    }
}
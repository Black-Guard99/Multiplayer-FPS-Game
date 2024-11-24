using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsoleEntryMessage : ConsoleEntryNode {

        public TMP_Text tmpValue;
        public RectTransform textRT;
        public Image collapseIcon;
        public Sprite normal, warning, error;
        public Message message;

        protected override void SetUp() {
            expandButton.color = Console.Foreground;
            tmpName.color = Console.Foreground;
            tmpName.font = Console.Font;
            tmpValue.color = Console.Foreground;
            tmpValue.font = Console.Font;
        }

        protected override void SetNode(Node node) {
            this.node = node;
        }

        public void SetMessage(Message message) {
            expandButton.enabled = true;
            expanded = false;
            this.message = message;
            switch (message.logType) {
                case LogType.Log:
                    collapseIcon.sprite = normal;
                    break;
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    collapseIcon.sprite = error;
                    break;
                case LogType.Warning:
                    collapseIcon.sprite = warning;
                    break;
            }

            tmpValue.text = "       " + message.condition + "\n" + message.stackTrace;
        }

        bool expanded;
        public void Callback_Expand() {
            expanded = !expanded;
            // If expanded -> Convert = 1, then the preferred height is added
            // Otherwise its value is 0 and we only use baseHeight
            rT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight + System.Convert.ToInt32(expanded) * tmpValue.preferredHeight);
            UpdateSize(this);
        }

        public override float GetHeight() {
            return baseHeight + System.Convert.ToInt32(expanded) * tmpValue.preferredHeight;
        }
    }
}
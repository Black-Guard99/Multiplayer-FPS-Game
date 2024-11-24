using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {
    public class ConsolePanelMessageFilter : MonoBehaviour {

        public RectTransform rT;
        public Image[] buttonsBg;
        public Image inputFrame;
        public TMP_InputField inputField;
        public Image clearImage;
        public TMP_Text clearTmp;

        bool[] typeFilter = new bool[3];
        Panel panel;
        bool textFilter;
        string text;
        
        public void Set(Panel panel, float height) {
            this.panel = panel;
            inputField.text = "";
            rT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            inputFrame.color = Console.Light;
            inputField.fontAsset = Console.Font;
            inputField.textComponent.font = Console.Font;
            inputField.textComponent.color = Console.Foreground;
            buttonsBg[0].color = Console.Light;
            buttonsBg[1].color = Console.Light;
            buttonsBg[2].color = Console.Light;
            clearTmp.color = Console.Background;
            clearImage.color = Console.Foreground;
            panel.OnNodeUpdate += UpdateMessages;
        }
        public void Callback_Select_Type(int code) {
            typeFilter[code] = !typeFilter[code];
            buttonsBg[code].color = typeFilter[code] ? Console.Medium : Console.Light;
            UpdateMessages();
            panel.instance.ForceUpdate();
        }
        public void Callback_Search(string s) {
            textFilter = s != "";
            text = s;
            UpdateMessages();
            panel.instance.ForceUpdate();
        }
        public void Callback_Clear() {
            Console.I.ClearLog();
        }
        void UpdateMessages() {
            for (int n = 0; n < panel.nodes.Count; ++n) {
                Message msg = panel.nodes[n] as Message;
                bool active;
                switch (msg.logType) {
                    case LogType.Log:
                        active = !typeFilter[0];
                        break;
                    case LogType.Warning:
                        active = !typeFilter[1];
                        break;
                    default:
                        active = !typeFilter[2];
                        break;
                }
                if (textFilter)
                    active &= msg.condition.Contains(text);
                msg.instance.gameObject.SetActive(active);
            }
        }

    }
}
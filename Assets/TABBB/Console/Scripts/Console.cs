using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TABBB.Tools.Console {

    #region NodeClasses
    /// <summary>
    /// Generic base class that can have subnodes as childs.
    /// Has a Scene object reference for its UI element.
    /// </summary>
    public class NodeHolder {
        public string name;
        public List<Node> nodes;
        public ConsoleNodeHolderInstance instance;
        public NodeHolder(string name, ConsoleNodeHolderInstance instance) {
            this.name = name;
            this.instance = instance;
            nodes = new List<Node>();
        }
        public void AddNode(Node node) {
            nodes.Add(node);
        }
        public void RemoveNode(Node node) {
            nodes.Remove(node);
        }
    }
    /// <summary>
    /// Class that holds all the information associated to a Panel. The UI element itself, Button used 
    /// to select it and additional fixed option panels.
    /// </summary>
    public class Panel : NodeHolder {
        public new ConsoleEntryPanel instance;
        public ConsolePanelButton button;
        public List<GameObject> minors;
        public Panel(string name, ConsoleEntryPanel instance, ConsolePanelButton button) : base(name, instance) {
            this.instance = instance;
            this.button = button;
        }
        public void AddMinorPanel(GameObject gameObject) {
            if (minors == null)
                minors = new List<GameObject>();
            minors.Add(gameObject);
            instance.AddMinorPanel(gameObject);
        }
        public event Action OnNodeUpdate;
        public void NodeUpdated() {
            OnNodeUpdate?.Invoke();
        }
    }
    /// <summary>
    /// Base class for an information Node. Used only to inherit and hold the references in NodeHolder
    /// </summary>
    public class Node : NodeHolder {
        public NodeHolder parent;
        public new ConsoleEntryNode instance;
        public Node(string name, ConsoleEntryNode instance, NodeHolder parent) : base(name, instance) {
            this.instance = instance;
            this.parent = parent;
        }
    }
    /// <summary>
    /// Keeps all the information given by the generic log output of the application and a reference so the UI element 
    /// displaying it.
    /// </summary>
    public class Message : Node {
        public string condition;
        public string stackTrace;
        public LogType logType;
        public Message(string condition, string stackTrace, LogType logType, ConsoleEntryMessage instance, NodeHolder parent) : base("", instance, parent) {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.logType = logType;
        }
    }
    /// <summary>
    /// Used to hold the callback and setups for each different Node type.
    /// </summary>
    /// <typeparam name="T">struct/class we want to use as base information.</typeparam>
    public class ValueNode<T> : Node {
        public T value;
        public delegate void DelegateObject(T t);
        public DelegateObject callback;
        /// <summary>
        /// Called from Node prefabs to trigger the object callback.
        /// </summary>
        /// <param name="value"></param>
        public void SetValueFromConsole(T value) {
            this.value = value;
            callback?.Invoke(value);
        }

        public void SetValue(T value) {
            this.value = value;
            instance.UpdateValue();
        }

        public ValueNode(string name, T value, DelegateObject callback, ConsoleEntryNode instance, NodeHolder parent) : base(name, instance, parent) {
            this.value = value;
            this.callback = callback;
        }
    }
    #endregion

    #region ValueClasses
    // This classes are used to hold all the necessary information to set up the different types of ConsoleEntryNode classes 
    // in scene.
    public class Vector2Value {
        public Vector2 value;
        public string format;
        public Vector2Value(Vector2 value, string format = "0.00") {
            this.value = value;
            this.format = format;
        }
    }
    public class Vector3Value {
        public Vector3 value;
        public string format;
        public Vector3Value(Vector3 value, string format = "0.00") {
            this.value = value;
            this.format = format;
        }
    }
    public class InputValue {
        public string value;
        public InputValue(string value) {
            this.value = value;
        }
    }
    public class SliderValue {
        public float min, max;
        public float value;
        public string format;
        public bool onlyInts;
        public SliderValue(float min, float max, float value, bool onlyInts = false, string format = "0.00") {
            this.min = min;
            this.max = max;
            this.value = value;
            this.format = format;
            this.onlyInts = onlyInts;
        }
    }
    public class FillValue {
        public float min, max;
        public float value;
        public bool displayValue;
        public string format;
        public FillValue(float min, float max, float value, bool displayValue = true, string format = "0.00") {
            this.min = min;
            this.max = max;
            this.value = value;
            this.displayValue = displayValue;
            this.format = format;
        }
    }
    public class ButtonValue {
        public string label;
        public ButtonValue(string label) {
            this.label = label;
        }
    }
    public class SubpanelValue {
        public string[] names;
        //public List<List<Node>> nodes;
        public SubpanelValue(params string[] names) {
            this.names = names;
            //nodes = new List<List<Node>>();
            //for (int n = 0; n < names.Length; ++n)
            //    nodes.Add(new List<Node>());
        }
        //public void AddNode(Node node, int tabIndex) {
        //    nodes[tabIndex].Add(node);
            
        //}
    }
    #endregion

    #region Customization
    [Serializable]
    public class Palette {
        [Header("General")]
        public float rowHeight = 30f;
        public Color background;
        public Color foreground;
        public Color light, medium, dark;
        public TMPro.TMP_FontAsset font;
        [Header("Buttons")]
        public bool buttonOverrideForeground;
        public Color buttonForeground;
        public ColorBlock buttonColorBlock;
        [Header("Fill bar")]
        public Gradient fillBarGradient;
    }
    #endregion

    public static class NodeHelpers {
        public static ValueNode<string> AddInfo(this NodeHolder nodeHolder, string name, string value) {
            return Console.I.AddNode(nodeHolder, name, value);
        }
        public static ValueNode<InputValue> AddInputField(this NodeHolder nodeHolder, string name, InputValue value, ValueNode<InputValue>.DelegateObject callback) {
            return Console.I.AddNode(nodeHolder, name, value, callback) as ValueNode<InputValue>;
        }
        public static ValueNode<SliderValue> AddSlider(this NodeHolder nodeHolder, string name, float minValue, float maxValue, float value, ValueNode<SliderValue>.DelegateObject callback) {
            return Console.I.AddNode(nodeHolder, name, new SliderValue(minValue, maxValue, value), callback) as ValueNode<SliderValue>;
        }
        public static ValueNode<FillValue> AddFillBar(this NodeHolder nodeHolder, string name, float minValue, float maxValue, float value) {
            return Console.I.AddNode(nodeHolder, name, new FillValue(minValue, maxValue, value)) as ValueNode<FillValue>;
        }
        public static ValueNode<ButtonValue> AddButton(this NodeHolder nodeHolder, string name, string label, ValueNode<ButtonValue>.DelegateObject callback) {
            return Console.I.AddNode(nodeHolder, name, new ButtonValue(label), callback) as ValueNode<ButtonValue>;
        }
        public static ValueNode<bool> AddToggle(this NodeHolder nodeHolder, string name, bool value, ValueNode<bool>.DelegateObject callback) {
            return Console.I.AddNode(nodeHolder, name, value, callback) as ValueNode<bool>;
        }
        public static ValueNode<Vector2Value> AddVector2(this NodeHolder nodeHolder, string name, Vector2 value, ValueNode<Vector2Value>.DelegateObject callback) {
            return Console.I.AddNode(nodeHolder, name, new Vector2Value(value), callback) as ValueNode<Vector2Value>;
        }
        public static ValueNode<Vector3Value> AddVector3(this NodeHolder nodeHolder, string name, Vector3 value, ValueNode<Vector3Value>.DelegateObject callback) {
            return Console.I.AddNode(nodeHolder, name, new Vector3Value(value), callback) as ValueNode<Vector3Value>;
        }
        public static ValueNode<SubpanelValue> AddSubpanel(this NodeHolder nodeHolder, string name, SubpanelValue value, out ConsoleEntryNodeSubpanels subpanel) {
            ValueNode<SubpanelValue> node = Console.I.AddNode(nodeHolder, name, value) as ValueNode<SubpanelValue>;
            subpanel = node.instance as ConsoleEntryNodeSubpanels;
            return node;
        }
    }

    public class Console : MonoBehaviour {

        public static Console I; // Instance for free references

        public bool awakeHidden = true;

        [Header("Log")]
        public bool catchLogs = true; // Do we create the generic log panel?
        public bool addFilters = true; // Add filter options? (log type & message content)
        public int MAX_MESSAGES = 100; //Size of the message pool

        [Header("Customization")]
        public ConsoleConfig consoleConfig;

        [Header("Self references")]
        public GameObject display;
        public Image background;
        public RectTransform parentPanels, parentPanelButtons;
        public GameObject prefabPanelButton, prefabPanel;

        [Header("Type Nodes")]
        public GameObject prefabMessage;
        public GameObject prefabMessageFiltering;
        public GameObject prefabNodeInfoDisplay; // simple info display
        public GameObject prefabNodeInputField; // string input field
        public GameObject prefabNodeVector2; // V2 input field
        public GameObject prefabNodeVector3; // V3 input field
        public GameObject prefabNodeToggle; // bool toggle
        public GameObject prefabNodeSlider; // float slider
        public GameObject prefabNodeFill; // horizontal fill + text in front
        public GameObject prefabNodeButton; // horizontal fill + text in front
        public GameObject prefabNodePanels; // horizontal fill + text in front

        public static Color Background { get { return I.palette.background; } }
        public static Color Foreground { get { return I.palette.foreground; } }
        public static Color Light { get { return I.palette.light; } }
        public static Color Medium { get { return I.palette.medium; } }
        public static Color Dark { get { return I.palette.dark; } }
        public static Gradient FillBarGradient { get { return I.palette.fillBarGradient; } }
        public static TMPro.TMP_FontAsset Font { get { return I.palette.font; } }
        public static Color ButtonForeground {
            get {
                if (I.palette.buttonOverrideForeground)
                    return I.palette.buttonForeground;
                return I.palette.foreground;
            }
        }
        public static ColorBlock ButtonColors { get { return I.palette.buttonColorBlock; } }
        public static float RowHeight { get { return I.palette.rowHeight; } }

        Dictionary<Type, GameObject> prefabDictionary;
        List<Panel> panels;
        Message[] messages;
        int msgCount;
        Panel selectedPanel, logPanel;
        Palette palette;

        /// <summary>
        /// Initialize all the structures. If catchLogs is true, the panel Logs is created and we subscribe to the application log event.
        /// </summary>
        private void Awake() {
            // initialize prefab dictionary
            prefabDictionary = new Dictionary<Type, GameObject> {
                { typeof(Vector2Value), prefabNodeVector2 },
                { typeof(Vector3Value), prefabNodeVector3 },
                { typeof(bool), prefabNodeToggle },
                { typeof(string), prefabNodeInfoDisplay },
                { typeof(Message), prefabMessage },
                { typeof(InputValue), prefabNodeInputField },
                { typeof(SliderValue), prefabNodeSlider },
                { typeof(FillValue), prefabNodeFill },
                { typeof(ButtonValue), prefabNodeButton },
                { typeof(SubpanelValue), prefabNodePanels }
            };

            // initialize structures
            I = this;
            panels = new List<Panel>();
            selectedPanel = null;
            messages = new Message[MAX_MESSAGES];
            msgCount = 0;
            display.SetActive(!awakeHidden);

            // customization
            palette = consoleConfig.configuration;
            background.color = palette.background;

            // create log panel
            if (catchLogs) {
                logPanel = AddPanel("Logs");
                Application.logMessageReceived += Application_logMessageReceived;
                if (addFilters) {
                    ConsolePanelMessageFilter filter = Instantiate(prefabMessageFiltering, parentPanels).GetComponent<ConsolePanelMessageFilter>();
                    filter.Set(logPanel, palette.rowHeight);
                    logPanel.AddMinorPanel(filter.gameObject);
                }
            }
            prefabMessageFiltering = null;
        }
        /// <summary>
        /// Use this method to add your custom Nodes without having modifying the Console Asset scripts in case of an asset Update override.
        /// </summary>
        /// <param name="type">Type of class/struct</param>
        /// <param name="prefab">ConsoleEntryNode inheriting based prefab.</param>
        public void AddPrefabNode(Type type, GameObject prefab) {
            prefabDictionary.Add(type, prefab);
        }

        /// <summary>
        /// Adds a message to the log panel whenever an Application log is sent.
        /// </summary>
        private void Application_logMessageReceived(string condition, string stackTrace, LogType type) {
            ConsoleEntryMessage instance;
            if (msgCount >= MAX_MESSAGES) {
                instance = messages[msgCount % MAX_MESSAGES].instance as ConsoleEntryMessage;
                instance.rT.SetAsLastSibling();
                instance.SetMessage(new Message(condition, stackTrace, type, instance, logPanel));
                logPanel.NodeUpdated();
            } else {
                instance = Instantiate(prefabMessage).GetComponent<ConsoleEntryMessage>();
                Message node = new Message(condition, stackTrace, type, instance, logPanel);
                instance.SetNode(node, true, palette.rowHeight);
                instance.SetMessage(node);
                logPanel.AddNode(node);
                logPanel.NodeUpdated();
                logPanel.instance.AddNode(node);
            }
            messages[msgCount % MAX_MESSAGES] = instance.message;
            ++msgCount;
        }
        public void ClearLog() {
            for (int n = 0; n < MAX_MESSAGES; ++n) {
                if (messages[n] == null) break;
                Destroy(messages[n].instance.gameObject);
            }
            logPanel.nodes = new List<Node>();
            logPanel.instance.ForceUpdate();
        }

        /// <summary>
        /// Adds a new panel.
        /// </summary>
        /// <param name="name">Name of the panel, display on the button associated to the panel.</param>
        /// <param name="active">Automatically display the panel?</param>
        /// <returns>Panel structure that holds all its information.</returns>
        public Panel AddPanel(string name, bool active = false) {
            ConsolePanelButton button = Instantiate(prefabPanelButton, parentPanelButtons).GetComponent<ConsolePanelButton>();
            button.Set(name, delegate { Callback_PanelButton(button); });

            ConsoleEntryPanel instance = Instantiate(prefabPanel, parentPanels).GetComponent<ConsoleEntryPanel>();
            instance.gameObject.SetActive(false);
            Panel panel = new Panel(name, instance, button);
            instance.Set(panel);

            panels.Add(panel);
            if (active)
                SelectPanel(panels.Count - 1);
            UpdatePanelButtons();
            return panel;
        }
        /// <summary>
        /// Destoys the panel instances and references to its structure.
        /// </summary>
        /// <param name="name">Name of the panel.</param>
        public void RemovePanel(string name) {
            Panel panel = FindPanel(name);
            if (panel == null) {
                Debug.LogWarning("Could not find panel by the name: " + panel);
                return;
            }
            RemovePanel(panel);
        }
        /// <summary>
        /// Destoys the panel instances and references to its structure.
        /// </summary>
        /// <param name="panel">Panel to remove</param>
        public void RemovePanel(Panel panel) {
            panels.Remove(panel);
            Destroy(panel.button.gameObject);
            Destroy(panel.instance.gameObject);
            if(panel == logPanel)
                Application.logMessageReceived -= Application_logMessageReceived;
            if (selectedPanel == panel && panels.Count > 0)
                SelectPanel(0);
            UpdatePanelButtons();
        }


        /// <summary>
        /// Adds a new Node to the Console.
        /// </summary>
        /// <param name="path">Path of the parent it goes into.</param>
        /// <param name="name">Name of the Node.</param>
        /// <param name="value">Default value of the Node.</param>
        /// <param name="collapsed">Should the child Nodes of the created Node be hidden by default?</param>
        /// <param name="callback">Callback called when its value is modified through the Console.</param>
        /// <returns>The created Node.</returns>
        public Node AddNode<T>(string path, string name, T value, ValueNode<T>.DelegateObject callback = null, bool collapsed = false) {
            NodeHolder holder = FindHolder(path);
            if (holder == null) {
                Debug.LogWarning("Could not find parent with path: " + path);
                return null;
            }
            return AddNode(holder, name, value, callback, collapsed);
        }
        /// <summary>
        /// Adds a new Node to an existing Node Holder element (Panel or Node).
        /// </summary>
        /// <param name="holder">Reference of the Holder it goes into.</param>
        /// <param name="name">Name of the Node.</param>
        /// <param name="value">Default value of the Node.</param>
        /// <param name="collapsed">Should the child Nodes of the created Node be hidden by default?</param>
        /// <param name="callback">Callback called when its value is modified through the Console.</param>
        /// <returns>The created Node.</returns>
        public ValueNode<T> AddNode<T>(NodeHolder holder, string name, T value, ValueNode<T>.DelegateObject callback = null, bool collapsed = false) {
            ConsoleEntryNode entryNode = Instantiate(prefabDictionary[value.GetType()]).GetComponent<ConsoleEntryNode>();
            ValueNode<T> node = new ValueNode<T>(name, value, callback, entryNode, holder);
            holder.AddNode(node);
            entryNode.SetNode(node, collapsed, palette.rowHeight);
            holder.instance.AddNode(node);
            return node;
        }
        /// <summary>
        /// Find a node and removes it from the console. Deletes all its associated instances.
        /// </summary>
        /// <param name="path">Path to the node.</param>
        public void RemoveNode(string path) {
            if (!(FindHolder(path) is Node n)) return;
            RemoveNode(n);
        }
        /// <summary>
        /// Removes the Node from the console. Deletes all its associated instances.
        /// </summary>
        /// <param name="nodeName">Node reference.</param>
        public void RemoveNode(Node node) {
            node.parent.RemoveNode(node);
            node.instance.Destroy();
        }


        /// <summary>
        /// Searches and returns a Panel by its name.
        /// </summary>
        public Panel FindPanel(string name) {
            Panel panel = null;
            for (int n = 0; n < panels.Count; ++n) {
                if (panels[n].name == name) {
                    panel = panels[n];
                    break;
                }
            }
            return panel;
        }
        /// <summary>
        /// Searches and returns a Node by its name. Naming should follow: PanelName/NodeName, add consecutive node names separated by / to add depth to the search.
        /// </summary>
        public Node FindNode(string path) {
            return FindHolder(path) as Node;
        }
        /// <summary>
        /// Searches and returns a NodeHolder by its name. Naming should follow: PanelName/NodeName, add consecutive node names separated by / to add depth to the search.
        /// </summary>
        public NodeHolder FindHolder(string path) {
            string[] split = path.Split('/');
            Panel panel = FindPanel(split[0]);
            NodeHolder nodeHolder = panel;
            //For every node depth, we do a search of the current node holder, starting with the panel
            for (int n = 1; n < split.Length; ++n) {
                // Find the child node of the current iteration
                Node iterationNode = null;
                for (int i = 0; i < nodeHolder.nodes.Count; ++i) {
                    if (nodeHolder.nodes[i].name == split[n]) {
                        iterationNode = nodeHolder.nodes[i];
                        break;
                    }
                }
                // Abort if we can not find a node
                if (iterationNode == null) {
                    Debug.LogWarning("Could not find Node by the name: " + split[n]);
                    return null;
                }
                // Update the iteration references with the found node
                nodeHolder = iterationNode;
            }
            return nodeHolder;
        }
        
        /// <summary>
        /// Updates the size of the panel buttons parent to make the horizontal scroll work properly.
        /// </summary>
        void UpdatePanelButtons() {
            float totalWidth = 0f;
            for (int n = 0; n < panels.Count; ++n)
                totalWidth += panels[n].button.rT.sizeDelta.x + 5f;
            parentPanelButtons.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        }
        /// <summary>
        /// Referenced by the Button component in the PrefabPanelButton through Callback().
        /// </summary>
        public void Callback_PanelButton(ConsolePanelButton sender) {
            SelectPanel(sender.rT.GetSiblingIndex());
        }
        /// <summary>
        /// Selects the panel to display by position.
        /// </summary>
        void SelectPanel(int index) {
            if (index >= panels.Count) return;
            if (selectedPanel == panels[index]) return;
            for (int n = 0; n < panels.Count; ++n) {
                panels[n].instance.gameObject.SetActive(n == index);
                panels[n].button.Highlight(n == index);
                if (panels[n].minors != null)
                    for (int m = 0; m < panels[n].minors.Count; ++m)
                        panels[n].minors[m].SetActive(n == index);
            }
            selectedPanel = panels[index];
        }

        /// <summary>
        /// Toggles the display state of the Console.
        /// </summary>
        public void Toggle() {
            display.SetActive(!display.activeSelf);
        }
    }
}
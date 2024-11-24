using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TABBB.Tools.Console {
    [CreateAssetMenu(fileName = "Config", menuName = "TABBB/ConsoleConfig")]
    public class ConsoleConfig : ScriptableObject {
        public Palette configuration;
    }
}
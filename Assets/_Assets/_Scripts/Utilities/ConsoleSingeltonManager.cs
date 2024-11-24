using UnityEngine;
using TABBB.Tools.Console;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using GamerWolf.Utils;

public class ConsoleSingeltonManager : GenericSingleton<ConsoleSingeltonManager> {
    private Panel panel;

    private void Start(){
        panel = Console.I.AddPanel("My Logs ",true);
    }

    public void ShowLogs(string logs){
        panel.AddInfo(logs,"");
    }
}
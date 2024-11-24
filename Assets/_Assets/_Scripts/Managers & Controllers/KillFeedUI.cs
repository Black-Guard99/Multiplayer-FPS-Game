using TMPro;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class KillFeedUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI killedby,killedWidth,killedTo;
    public void SetKills(string killedBy, string killedWith, string killedTO) {
        killedby.SetText(killedBy);
        killedWidth.SetText(string.Concat("[",killedWith,"]"));
        killedTo.SetText(killedTO);
    }
    
    
}
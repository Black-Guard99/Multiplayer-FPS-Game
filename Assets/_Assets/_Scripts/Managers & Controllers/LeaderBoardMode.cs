using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class LeaderBoardMode : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI mode,map;
    [SerializeField] private TextMeshProUGUI homeScore,awayScore;
    [SerializeField] private GameObject homeToggle,awayToggle;
    
    public void InitLeaderBoard(string mode,string map){
        this.mode.text = mode;
        this.map.text = map;
    }
    public void SetHomeandAwayScore(string homescore,string awayscore){
        if(homeScore != null) homeScore.text = homescore;
        if(awayScore != null) awayScore.text = awayscore;
    }
}
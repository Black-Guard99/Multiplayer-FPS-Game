using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class UIControllers : MonoBehaviour {
    public static UIControllers Current{get;private set;}
    [SerializeField] private GameObject endGameWindow;
    [SerializeField] private GameObject waitingTimeWindow;
    [SerializeField] private TextMeshProUGUI waitingTimeText;
    [SerializeField] private TextMeshProUGUI timeDiplayTime;
    [SerializeField] private TextMeshProUGUI localPlayerDeath,localPlayerKills;
    [SerializeField] private TextMeshProUGUI pingText;
    private void Awake(){
        if(Current == null){
            Current = this;
        }else{
            Destroy(Current.gameObject);
        }
    }
    public void ShowHideWaitingWindow(bool waiting){
        waitingTimeWindow.SetActive(waiting);
    }
    public void ShowWaitingTime(float time){
        waitingTimeText.SetText(time.ToString());
    }
    public void ShowHideEndGameWindow(bool show){
        endGameWindow.SetActive(show);
    }
    public void SetKills(int killcount){
        localPlayerKills.SetText(string.Concat(killcount));
    }
    public void SetDeath(int deathCount){
        localPlayerKills.SetText(string.Concat(deathCount));
    }
    
    public void DisplayTime(float minit,float seconds){
        timeDiplayTime.SetText(string.Format("{00:00}:{1:00}",minit,seconds));
    }
    /* public void SetPingAmount(int ping){
        Color pingColor = ping > 100 ? Color.red : Color.green;
        pingText.color = pingColor;
        pingText.SetText(string.Concat("Ping : ",ping));

    } */
    
    
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LeaderboardPlayerCard : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI levelText,usernameText,scoreValueText,killsValueText,DeathValueText;
    [SerializeField] private Image cardImage;
    [SerializeField] private GameObject homeTeamIndicator,awayTeamIndicator;


    public void SetIsAwayTeam(bool awayTeam){
        awayTeamIndicator.SetActive(awayTeam);
    }
    public void SetIsHomeTeam(bool homeTeam){
        homeTeamIndicator.SetActive(homeTeam);
    }
    public void SetCardsData(string level,string username,string scores,string kills,string death){
        levelText.SetText(level);
        usernameText.SetText(username);
        scoreValueText.SetText(scores);
        killsValueText.SetText(kills);
        DeathValueText.SetText(death);
    }
    public void SetCardsColor(Color cardColor){
        cardImage.color = cardColor;
    }
}
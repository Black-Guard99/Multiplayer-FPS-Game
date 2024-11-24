using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LocalPlayerUiManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI respawningTimer;
    public void ShowRespawnTimer(float timeLeft) {
        respawningTimer.gameObject.SetActive(true);
        respawningTimer.SetText(timeLeft.ToString());
    }
}
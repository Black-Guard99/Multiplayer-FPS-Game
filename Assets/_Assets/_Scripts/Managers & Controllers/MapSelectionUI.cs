using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using TMPro;

public class MapSelectionUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI mapName;
    [SerializeField] private GameObject selectionIcon;
    [SerializeField] private MapData mapData;
    [SerializeField] private MatchManagrUI mathManagrUi;
    private void Start(){
        mapName.SetText(mapData.name.ToUpper());
    }
    public void SetMap(){
        mathManagrUi.SetCurrenetMap(mapData);
    }
    public void ShowHideSelectionIcon(GameSettingsSO gameSettings){
        selectionIcon.SetActive(gameSettings.mapData.name == mapData.name);
    }

    
}
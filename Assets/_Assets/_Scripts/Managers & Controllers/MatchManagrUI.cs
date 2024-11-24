using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MatchManagrUI : MonoBehaviour {
    [SerializeField] private GameObject modeSelectionWindow,mapSelectionWindow;
    [SerializeField] private MapSelectionUI[] mapSelectionButton;
    [SerializeField] private ModeSelectionUi[] modeSelectionButton;
    [SerializeField] private GameSettingsSO gameSettings;
    [SerializeField] private Launcher launcher;
    [SerializeField] private Toggle uIButtonToggles;
    private void Start(){
        OpenMode();
        SetActiveModesAndMaps();
    }

    public void OpenMode(){
        modeSelectionWindow.SetActive(true);
        mapSelectionWindow.SetActive(false);
        SetActiveModesAndMaps();
    }
    public void OpenMap(){
        modeSelectionWindow.SetActive(false);
        mapSelectionWindow.SetActive(true);
        SetActiveModesAndMaps();
    }
    public void SetCurrentMode(GameMode gameMode) {
        gameSettings.gameMode = gameMode;
        SetActiveModesAndMaps();
    }

    public void SetCurrenetMap(MapData mapData) {
        gameSettings.mapData = mapData;
        SetActiveModesAndMaps();
    }
    public void SetActiveModesAndMaps(){
        foreach(MapSelectionUI mapSelectionUI in mapSelectionButton){
            mapSelectionUI.ShowHideSelectionIcon(gameSettings);
        }
        foreach(ModeSelectionUi modeSelectionUi in modeSelectionButton){
            modeSelectionUi.ShowHideSelectionIcon(gameSettings);
        }
    }
    public void ToggleHasBots(){
        gameSettings.hasBots = uIButtonToggles.isOn;
    }
}
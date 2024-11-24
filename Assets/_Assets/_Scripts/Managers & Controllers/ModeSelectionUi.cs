using TMPro;
using UnityEngine;

public class ModeSelectionUi : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI modeName;
    [SerializeField] private GameObject selectionIcon;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private MatchManagrUI matchManagerUi;
    private void Start(){
        modeName.SetText(gameMode.ToString().ToUpper());
    }

    public void SetCurrentMode(){
        matchManagerUi.SetCurrentMode(gameMode);
    }
    public void ShowHideSelectionIcon(GameSettingsSO gameSettings){
        selectionIcon.SetActive(gameSettings.gameMode == gameMode);
        
    }
}
using UnityEngine;
public class WeponUIManager : MonoBehaviour {
    [SerializeField] private NightVisionSystem nightVisionSystem;
    [SerializeField] private ThrowableSelectionUI[] throwableSelectionUisList;
    
    private bool lastSelectedMele;
    public void RefreshThrowableUi(){
        foreach(ThrowableSelectionUI throwableSelectionUI in throwableSelectionUisList){
            throwableSelectionUI.HideWindow();
        }
    }
    public void OnMeleSelected(){
        lastSelectedMele = true;
    }
    public void PrimaryOrSecondarySelected(){
        lastSelectedMele = false;
    }
    public bool GetLastSelectedMele(){
        return lastSelectedMele;
    }
    public void ToggleNightVision(){
        nightVisionSystem.ToggleNightVision();
    }
}
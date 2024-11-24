using UnityEngine;
public class LoadoutManager : MonoBehaviour {
    [SerializeField] private LoadoutSO[] allLoadouts;
    [SerializeField] private LoadoutSO currentSelectingLoadout;
    [SerializeField] private LoadoutDisplayBtnUI[] loadoutDisplayBtnUiArray;
    private void Start(){
        foreach(LoadoutSO loadout in allLoadouts){
            if(loadout.isActiveLoadout){
                currentSelectingLoadout = loadout;
                break;
            }
        }
    }
    public void SetCurrentLoadout(LoadoutSO loadout) {
        currentSelectingLoadout = loadout;
        RefershLoadoutDisplayBtn();
    }
    private void RefershLoadoutDisplayBtn(){
        foreach(LoadoutDisplayBtnUI loadoutDisplayBtnUI in loadoutDisplayBtnUiArray){
            if(loadoutDisplayBtnUI.GetLoadoutSO() == currentSelectingLoadout){
                loadoutDisplayBtnUI.RefereshLoadoutBtn(false);
            }else{
                loadoutDisplayBtnUI.RefereshLoadoutBtn(true);
            }
        }
    }
    public void SelectLoadoutForTheMatch(){
        //^ Calling From Select Loadout Button..............
        foreach(LoadoutSO loadout in allLoadouts){
            loadout.isActiveLoadout = false;
        }
        currentSelectingLoadout.isActiveLoadout = true;
        foreach(LoadoutDisplayBtnUI loadoutDisplayBtnUI in loadoutDisplayBtnUiArray){
            loadoutDisplayBtnUI.RefreshUI();
        }
    }
}
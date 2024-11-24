using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LodoutCustomizeUIBtn : MonoBehaviour {
    [SerializeField] private LoadoutManagerUI loadoutManagerUi;
    [SerializeField] private LoadoutSO loadout;
    [SerializeField] private GameObject loadSelection,WeponView;
    public void ShowCurrentLoadout(LoadoutSO loadout){
        this.loadout = loadout;
    }
    // Call from Btn.......
    public void SetCustomizedLoad(){
        if(loadout == null){
            // ConsoleSingeltonManager.Current?.ShowLogs("No Loadout to Added To Show....");
            return;
        }
        loadoutManagerUi.SetCurrentLoadouts(loadout);
        loadSelection.SetActive(false);
        WeponView.SetActive(true);
    }
}
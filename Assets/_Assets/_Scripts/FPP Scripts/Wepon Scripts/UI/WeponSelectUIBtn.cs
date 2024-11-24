using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WeponSelectUIBtn : MonoBehaviour {
    [SerializeField] private Loadout loadout;
    [SerializeField] private WeponUIManager weponSelectUiBtn;
    [SerializeField] private Gun.WeponPositions weponPositions;
    
    public void SwitchPrimaryAndSecondary(){
        if(weponSelectUiBtn.GetLastSelectedMele()){
            weponSelectUiBtn.PrimaryOrSecondarySelected();
            switch(weponPositions){
                case Gun.WeponPositions.Primary:
                    loadout.SwitchtoPrimaryOnly();
                break;
                case Gun.WeponPositions.Secondary:
                    loadout.SwitchtoSecondaryOnly();
                break;
            }
            return;
        }
        loadout.SwitchPrimaryAndSecondaryWepons();
    }
    public void SwitchMele(){
        weponSelectUiBtn.OnMeleSelected();
        loadout.SwitchMele();
    }
}
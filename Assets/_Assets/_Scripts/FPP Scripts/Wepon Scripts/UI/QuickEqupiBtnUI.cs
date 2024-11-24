using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class QuickEqupiBtnUI : MonoBehaviour {
    [SerializeField] private LoadoutManagerUI loadoutManagerUi;
    private GunDisplaySO gundisplay;

    public void QuickEquipCurrentGun(){
        loadoutManagerUi.SetCurrentLoadoutGuns(gundisplay.gunSo);
    }
    public void SetCurrentGun(GunDisplaySO gunDisplay){
        this.gundisplay = gunDisplay;
    }
}
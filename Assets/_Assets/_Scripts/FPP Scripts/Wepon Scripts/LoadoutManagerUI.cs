using System;
using UnityEngine;
public class LoadoutManagerUI : MonoBehaviour {
    [SerializeField] private LoadoutSO loadoutSo;
    [SerializeField] private Transform weponViewTransform;
    [SerializeField] private GameObject weponCustomizationWindow,loadoutWindow;
    [SerializeField] private LoadoutDisplayBtnUI[] loadoutDisplayBtnUisArray;
    [SerializeField] private WeponView[] weponViews;
    public Action OnWeponChange;
    public void SetCurrentLoadouts(LoadoutSO loadout){
        loadoutSo = loadout;
        Debug.Log("Loadout " + loadout.name);
        foreach(WeponView weponView in weponViews){
            weponView.ShowHideIndicator();
        }
        
    }
    public void SetCurrentLoadoutGuns(GunSO gun){
        if(loadoutSo == null) return;
        switch(gun.weponPositions){
            case Gun.WeponPositions.Primary:
                loadoutSo.primaryGunAttachments.gunCurrent = gun;
            break;
            case Gun.WeponPositions.Secondary:
                loadoutSo.secondaryGunAttachments.gunCurrent = gun;
            break;
            case Gun.WeponPositions.Mele:
                loadoutSo.meleWeaponAttachments.gunCurrent = gun;
            break;
            case Gun.WeponPositions.LethalThrowable:
                loadoutSo.lethalThrowabl.gunCurrent = gun;
            break;
            case Gun.WeponPositions.NonLethalThrowable:
                loadoutSo.nonLethalThrowabl.gunCurrent = gun;
            break;
        }
        OnWeponChange?.Invoke();
        foreach(WeponView weponView in weponViews){
            weponView.ShowHideIndicator();
        }
    }
    public void RemoveCurrentGun(){
        if(weponViewTransform.childCount > 0){
            foreach(Transform gunSpawnd in weponViewTransform){
                Destroy(gunSpawnd.gameObject);
            }
        }
        foreach(LoadoutDisplayBtnUI loadoutDisplayBtn in loadoutDisplayBtnUisArray){
            loadoutDisplayBtn.RefreshUI();
        }
        weponCustomizationWindow.SetActive(false);
        loadoutWindow.SetActive(true);
    }
    public bool HasGunInLoadout(GunSO gun){
        return loadoutSo.primaryGunAttachments.gunCurrent == gun || loadoutSo.secondaryGunAttachments.gunCurrent == gun 
            || loadoutSo.meleWeaponAttachments.gunCurrent == gun || loadoutSo.lethalThrowabl.gunCurrent == gun  
                || loadoutSo.nonLethalThrowabl.gunCurrent == gun;
    }
    public LoadoutSO CurrentLoadOutDisplay(){
        return loadoutSo;
    }

}
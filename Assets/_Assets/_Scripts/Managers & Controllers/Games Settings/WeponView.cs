using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class WeponView : MonoBehaviour {
    private enum WeponDisplayType{
        Assult,
        Sniper,
        LMG,
        SMG,
        Shotgun,
        HandGun,
        Mele,
        LethalThrowable,
        NonLethalThrowable,

    }
    [SerializeField] private Transform gunSpawnPoint;
    [SerializeField] private GameObject quickEquipBtn;
    [SerializeField] private GameObject weaponShowIndicator;
    [SerializeField] private WeponDisplayType weponDisplayType;
    [SerializeField] private GameObject assultRifelView,sniperView,lmgView,smgView,shotgunView,handgunView,meleGunView,lethalView,nonLethalView;
    [SerializeField] private List<WeponDisplayButton> assultsWeaponBtnList,sniperWeaponBtnList,lmgWeaponBtnList,smgWeaponBtnList,shotgunWeapoBtnList,handGunWeaponBtnList,meleWeaponBtnList,lethalWeaponBtnList,nonLeathlWeaponBtnList;
    private GameObject CurrentSpawnedGun;
    private LoadoutSO currentLoadout;
    private void Start(){
        ShowHideIndicator();
        assultRifelView.SetActive(false);
        sniperView.SetActive(false);
        lmgView.SetActive(false);
        smgView.SetActive(false);
        shotgunView.SetActive(false);
        handgunView.SetActive(false);
        meleGunView.SetActive(false);
        lethalView.SetActive(false);
        nonLethalView.SetActive(false);
    }
    public void SpawnGun(GunDisplaySO gunDisplaySo,LoadoutSO loadoutSO){
        if(CurrentSpawnedGun != null){
            Destroy(CurrentSpawnedGun);
        }
        currentLoadout = loadoutSO;
        CurrentSpawnedGun = Instantiate(gunDisplaySo.displayPrefabs,gunSpawnPoint);
        if(CurrentSpawnedGun.TryGetComponent(out AttachmentDisplayGun attachmentDisplayGun)){
            attachmentDisplayGun.SetLoadout(currentLoadout);
        }
    }
    public void ShowDisplay(){
        if(gunSpawnPoint.childCount > 0){
            foreach(Transform gunSpawnd in gunSpawnPoint){
                Destroy(gunSpawnd.gameObject);
            }
        }
        assultRifelView.SetActive(false);
        sniperView.SetActive(false);
        lmgView.SetActive(false);
        smgView.SetActive(false);
        shotgunView.SetActive(false);
        handgunView.SetActive(false);
        meleGunView.SetActive(false);
        lethalView.SetActive(false);
        nonLethalView.SetActive(false);
        quickEquipBtn.SetActive(false);
        switch(weponDisplayType){
            case WeponDisplayType.Assult:
                assultRifelView.SetActive(true);
            break;
            case WeponDisplayType.Sniper:
                sniperView.SetActive(true);
            break;
            case WeponDisplayType.LMG:
                lmgView.SetActive(true);
            break;
            case WeponDisplayType.SMG:
                smgView.SetActive(true);
            break;
            case WeponDisplayType.Shotgun:
                shotgunView.SetActive(true);
            break;
            case WeponDisplayType.HandGun:
                handgunView.SetActive(true);
            break;
            case WeponDisplayType.Mele:
                meleGunView.SetActive(true);
            break;
            case WeponDisplayType.LethalThrowable:
                lethalView.SetActive(true);
            break;
            case WeponDisplayType.NonLethalThrowable:
                nonLethalView.SetActive(true);
            break;

        }
        ShowHideIndicator();
    }
    public void ShowHideIndicator(){
        switch(weponDisplayType){
            case WeponDisplayType.Assult:
                for(int i = 0 ; i < assultsWeaponBtnList.Count; i++){
                    if(assultsWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.Sniper:
                for(int i = 0 ; i < sniperWeaponBtnList.Count; i++){
                    if(sniperWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.LMG:
                for(int i = 0 ; i < lmgWeaponBtnList.Count; i++){
                    if(lmgWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.SMG:
                for(int i = 0 ; i < smgWeaponBtnList.Count; i++){
                    if(smgWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.Shotgun:
                for(int i = 0 ; i < shotgunWeapoBtnList.Count; i++){
                    if(shotgunWeapoBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.HandGun:
                for(int i = 0 ; i < handGunWeaponBtnList.Count; i++){
                    if(handGunWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.Mele:
                for(int i = 0 ; i < meleWeaponBtnList.Count; i++){
                    if(meleWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.LethalThrowable:
                for(int i = 0 ; i < lethalWeaponBtnList.Count; i++){
                    if(lethalWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;
            case WeponDisplayType.NonLethalThrowable:
                for(int i = 0 ; i < nonLeathlWeaponBtnList.Count; i++){
                    if(nonLeathlWeaponBtnList[i].IsActiveWeapon()){
                        weaponShowIndicator.SetActive(true);
                        break;
                    }else{
                        weaponShowIndicator.SetActive(false);
                    }
                }
            break;

        }
    }
}
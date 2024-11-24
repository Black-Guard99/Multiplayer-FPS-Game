using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoadoutUI : MonoBehaviour {
    [SerializeField] private LoadoutSO currentLoadOut;
    [SerializeField] private Image primaryWeaponIconImage,secondaryWeaponIconImage,meleWeaponIconImage,lethalWeaponIconImage,nonLethalWeaponIconImage;
    [SerializeField] private TextMeshProUGUI primaryWeponNameText,secondaryWeponNameText,meleWeponNameText,lethalWeponNameText,nonLethalWeponNameText;
    public void SetCurrentLoadout(LoadoutSO loadout) {
        currentLoadOut = loadout;
        SetUI();
    }

    public void SetUI(){
        if(currentLoadOut == null) return;
        // Set Text..........
        primaryWeponNameText.SetText(string.Concat(currentLoadOut.primaryGunAttachments.gunCurrent.name.ToUpper()));
        secondaryWeponNameText.SetText(string.Concat(currentLoadOut.secondaryGunAttachments.gunCurrent.name.ToUpper()));
        meleWeponNameText.SetText(string.Concat(currentLoadOut.meleWeaponAttachments.gunCurrent.name.ToUpper()));
        lethalWeponNameText.SetText(string.Concat(currentLoadOut.lethalThrowabl.gunCurrent.name.ToUpper()));
        nonLethalWeponNameText.SetText(string.Concat(currentLoadOut.nonLethalThrowabl.gunCurrent.name.ToUpper()));
        // Set Ui Image.............
        primaryWeaponIconImage.sprite = currentLoadOut.primaryGunAttachments.gunCurrent.weponUiIcon;
        secondaryWeaponIconImage.sprite = currentLoadOut.secondaryGunAttachments.gunCurrent.weponUiIcon;
        meleWeaponIconImage.sprite = currentLoadOut.meleWeaponAttachments.gunCurrent.weponUiIcon;
        lethalWeaponIconImage.sprite = currentLoadOut.lethalThrowabl.gunCurrent.weponUiIcon;
        nonLethalWeaponIconImage.sprite = currentLoadOut.nonLethalThrowabl.gunCurrent.weponUiIcon;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class ThrowableSelectionBtn : MonoBehaviour {
    [SerializeField] private Loadout loadout;
    [SerializeField] private Image throwableIconImage;
    [SerializeField] private ThrowableSO.ThrowableType throwableType;
    [SerializeField] private ThrowableSelectionUI throwableSelectionUi;
    [SerializeField] private TextMeshProUGUI totalAmountAvailable,weponName;
    private Button selectionBtn;
    private void Awake(){
        selectionBtn = GetComponent<Button>();
    }
    public void RefershThrowableAmounts(){
        if(loadout.GetCurrentThrowable(throwableType) != null) {
            totalAmountAvailable.SetText(string.Concat(loadout.GetCurrentThrowable(throwableType).GetMaxBulletCount));
            throwableIconImage.sprite = loadout.GetCurrentThrowable(throwableType).GetGunSprite;
        }
    }
    public void SelectThrowable(){
        if(loadout.GetCurrentThrowable(throwableType) != null) {
            if(loadout.GetCurrentThrowable(throwableType).GetCurrentBulletCount > 0){
                loadout.SelectSingleThrowable(throwableType);
                totalAmountAvailable.SetText(string.Concat(loadout.GetCurrentGun.GetMaxBulletCount));
                throwableSelectionUi.SetCurrentThrowable(weponName.text,totalAmountAvailable.text,this);
            }else{
                Debug.Log("No Ammo");
                selectionBtn.interactable = false;
            }
            RefershThrowableAmounts();
            throwableSelectionUi.CloseSelectionWindow();
        }
    }
    public string GetWeponName(){
        return weponName.text;
    }
    public string GetAmountText(){
        return totalAmountAvailable.text;
    }
    public Sprite GetIconSprite{
        get{
            return throwableIconImage.sprite;
        }
    }
    public void SelectUnSelectThrowable(){
        weponName.text = throwableType.ToString();
        RefershThrowableAmounts();
        if(loadout.GetCurrentThrowable(GetThrowableType()) != null){
            loadout.GetCurrentThrowable(throwableType).OnShoot += ()=>{
                totalAmountAvailable.SetText(string.Concat(loadout.GetCurrentThrowable(throwableType).GetMaxBulletCount));
                throwableSelectionUi.DisplayThrowableAmount();
            };
        }
    }
    public ThrowableSO.ThrowableType GetThrowableType(){
        return throwableType;
    }
}
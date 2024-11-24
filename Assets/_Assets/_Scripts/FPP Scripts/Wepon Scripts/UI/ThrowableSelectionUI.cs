using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ThrowableSelectionUI : MonoBehaviour {
    [SerializeField] private Loadout loadout;
    [SerializeField] private GameObject selectionWindow;
    [SerializeField] private WeponUIManager weponSelectionUi;
    [SerializeField] private List<ThrowableSelectionBtn> throwableSelectionBtnList;
    [SerializeField] private TextMeshProUGUI weponName,amountName;
    [SerializeField] private Image throwableIconImage;
    [SerializeField] private ThrowableSelectionBtn currentActiveThrowableBtn;
    private bool show;
    private Button selectionBtn;
    private void Awake(){
        selectionBtn = GetComponent<Button>();
    }
    public void HideWindow(){
        for (int i = 0; i < throwableSelectionBtnList.Count; i++) {
            if(loadout.GetCurrentThrowable(throwableSelectionBtnList[i].GetThrowableType()) == null){
                ThrowableSelectionBtn currentBtn = throwableSelectionBtnList[i];
                throwableSelectionBtnList.Remove(currentBtn);
                currentBtn.gameObject.SetActive(false);
                i--;
            }else{
                throwableSelectionBtnList[i].SelectUnSelectThrowable();
            }
        }
        currentActiveThrowableBtn = throwableSelectionBtnList[0];
        weponName.SetText(currentActiveThrowableBtn.GetWeponName());
        amountName.SetText(currentActiveThrowableBtn.GetAmountText());
        CloseSelectionWindow();
    }
    public void OpenSelectionWindow(){
        if(loadout.IsAiming) return;
        selectionWindow.SetActive(true);
        RefershThrowables();
    }
    public void CloseSelectionWindow(){
        show = false;
        RefershThrowables();
        selectionWindow.SetActive(false);
    }
    public void ToggleSelectionWindow(){
        show = !show;
        if(show){
            OpenSelectionWindow();
        }else{
            CloseSelectionWindow();
        }
    }
    public void RemoveFromList(ThrowableSelectionBtn throwableSelectionBtn){
        if(throwableSelectionBtnList.Contains(throwableSelectionBtn)){
            throwableSelectionBtnList.Remove(throwableSelectionBtn);
        }
    }
    public void RefershThrowables(){
        foreach(ThrowableSelectionBtn throwableSelection in throwableSelectionBtnList){
            throwableSelection.RefershThrowableAmounts();
        }
    }
    public void DisplayThrowableAmount(){
        weponName.SetText(currentActiveThrowableBtn.GetWeponName());
        amountName.SetText(currentActiveThrowableBtn.GetAmountText());
    }
    
    public void SetCurrentThrowable(string name,string amount,ThrowableSelectionBtn throwableSelectionBtn){
        currentActiveThrowableBtn = throwableSelectionBtn;
        weponName.SetText(currentActiveThrowableBtn.GetWeponName());
        amountName.SetText(currentActiveThrowableBtn.GetAmountText());
        throwableIconImage.sprite = currentActiveThrowableBtn.GetIconSprite;
        weponSelectionUi.OnMeleSelected();
    }
}
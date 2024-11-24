using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
public class ShootingUITrigger : MonoBehaviour,IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private Loadout loadout;
    [SerializeField] private NewPlayerInputController inputController;
    [SerializeField] private bool canChangeDirectionInShooting = true;
    [SerializeField] private GameObject thowingTimerBtn;
    [SerializeField] private Image throwingTimerFillImage;
    private bool isPressing;

    public void OnPointerDown(PointerEventData eventData) {
        isPressing = true;
        Debug.Log("Hodling FireButton");
        loadout.TapOnFireButton();
        if(canChangeDirectionInShooting){
            inputController.SetOnPointerOnShootBtn(true,this.gameObject);
        }else{
            inputController.SetOnPointerOnShootBtn(false,null);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        isPressing = false;
        Debug.Log("Fire Button Up");
        loadout.ReleaseFireButton();
        if(canChangeDirectionInShooting){
            inputController.SetOnPointerOnShootBtn(false,null);
        }
    }
    public void CancleHold(){
        isPressing = false;
        loadout.CancleHold();
    }
    private void Update(){
        if(isPressing){
            loadout.HoldingDownFireButton();
            if(loadout.GetCurrentGun.GetWeponType == Gun.WeponPositions.LethalThrowable || loadout.GetCurrentGun.GetWeponType == Gun.WeponPositions.NonLethalThrowable){
                thowingTimerBtn.SetActive(true);
                throwingTimerFillImage.fillAmount = loadout.GetThrowableTimer();
            }else{
                thowingTimerBtn.SetActive(false);
            }
        }else{
            thowingTimerBtn.SetActive(false);
        }
    }

    
}
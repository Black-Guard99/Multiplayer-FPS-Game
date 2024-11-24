using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ReloadBtnUI : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private Loadout playerLoadout;
    [SerializeField] private Image reloadingTimer;
    private float currentReloadTime;
    private float maxReloadTime;
    public void OnPointerDown(PointerEventData eventData) {
        if(!playerLoadout.GetCurrentGun.IsReloading){
            playerLoadout.StartReload();
            maxReloadTime = playerLoadout.GetCurrentGun.GetReloadingTime;
            currentReloadTime = 0f;
            reloadingTimer.gameObject.SetActive(true);
        }else{
            playerLoadout.CancleReload();
            currentReloadTime = 0f;
            reloadingTimer.fillAmount = 1f;
            reloadingTimer.gameObject.SetActive(false);
        }
    }
    private void Update(){
        if(playerLoadout.GetCurrentGun == null){
            reloadingTimer.fillAmount = 1f;
            reloadingTimer.gameObject.SetActive(false);
            return;
        }
        if(playerLoadout.GetCurrentGun.IsReloading){
            reloadingTimer.gameObject.SetActive(true);
            currentReloadTime += Time.deltaTime;
            reloadingTimer.fillAmount = currentReloadTime / maxReloadTime;
            if(currentReloadTime >= maxReloadTime){
                currentReloadTime = 0f;
                reloadingTimer.fillAmount = 1f;
            }

        }else{
            reloadingTimer.fillAmount = 1f;
            reloadingTimer.gameObject.SetActive(false);
        }
    }
}
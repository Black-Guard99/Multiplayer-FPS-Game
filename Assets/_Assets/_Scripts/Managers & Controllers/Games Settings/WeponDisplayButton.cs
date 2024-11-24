using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeponDisplayButton : MonoBehaviour {
    [SerializeField] private LoadoutManagerUI loadoutManagerUi;
    [SerializeField] private GunDisplaySO gunDisplaySo;
    [SerializeField] private WeponView weponView;
    [SerializeField] private Image gunIcon;
    [SerializeField] private TextMeshProUGUI gunNameText;
    [SerializeField] private StatManager statManager;
    [SerializeField] private QuickEqupiBtnUI quickEqupiBtnUI;
    [SerializeField] private GameObject currentActiveWeaponIndicator;
    private void Start(){
        gunIcon.sprite = gunDisplaySo.gunSprite;
        gunNameText.SetText(gunDisplaySo.gunName);
        quickEqupiBtnUI.gameObject.SetActive(false);
        loadoutManagerUi.OnWeponChange += ()=>{
            ShowHideIndicator(loadoutManagerUi.HasGunInLoadout(gunDisplaySo.gunSo));    
        };
        ShowHideIndicator(loadoutManagerUi.HasGunInLoadout(gunDisplaySo.gunSo));
    }
    public void ShowGun(){
        quickEqupiBtnUI.gameObject.SetActive(true);
        weponView.SpawnGun(gunDisplaySo,loadoutManagerUi.CurrentLoadOutDisplay());
        statManager.SetGun(gunDisplaySo.gunSo);
        quickEqupiBtnUI.SetCurrentGun(gunDisplaySo);
        ShowHideIndicator(loadoutManagerUi.HasGunInLoadout(gunDisplaySo.gunSo));
    }
    public void ShowHideIndicator(bool show){
        currentActiveWeaponIndicator.SetActive(show);
    }
    public bool IsActiveWeapon(){
        return loadoutManagerUi.HasGunInLoadout(gunDisplaySo.gunSo);
    }
}
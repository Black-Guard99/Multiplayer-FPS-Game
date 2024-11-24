using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoadoutDisplayBtnUI : MonoBehaviour {
    [SerializeField] private LoadoutSO loadout;
    [SerializeField] private LoadoutUI loadoutUi;
    [SerializeField] private Image loadoutVisual;
    [SerializeField] private TextMeshProUGUI loadoutNumberText;
    [SerializeField] private LodoutCustomizeUIBtn lodoutCustomizeUiBtn;
    [SerializeField] private LoadoutManager loadoutManager;
    [SerializeField] private GameObject currentActiveLoadoutIndicator;
    private Button clickBtn;
    private void Awake(){
        clickBtn = GetComponent<Button>();
        clickBtn.interactable = !loadout.isActiveLoadout;
        loadoutNumberText.SetText(loadout.name.ToUpper());
        loadoutVisual.sprite = loadout.primaryGunAttachments.gunCurrent.weponUiIcon;
        currentActiveLoadoutIndicator.SetActive(loadout.isActiveLoadout);
        if(loadout.isActiveLoadout){
            loadoutUi.SetCurrentLoadout(loadout);
            lodoutCustomizeUiBtn.ShowCurrentLoadout(loadout);
        }
    }
    // Call from Btn.......
    public void ShowCurrentLoadout(){
        loadoutNumberText.SetText(loadout.name.ToUpper());
        loadoutVisual.sprite = loadout.primaryGunAttachments.gunCurrent.weponUiIcon;
        loadoutUi.SetCurrentLoadout(loadout);
        lodoutCustomizeUiBtn.ShowCurrentLoadout(loadout);
        loadoutManager.SetCurrentLoadout(loadout);
    }

    public void RefreshUI() {
        loadoutNumberText.SetText(loadout.name.ToUpper());
        loadoutVisual.sprite = loadout.primaryGunAttachments.gunCurrent.weponUiIcon;
        loadoutUi.SetUI();
        currentActiveLoadoutIndicator.SetActive(loadout.isActiveLoadout);
        
    }
    public void RefereshLoadoutBtn(bool isActive){
        clickBtn.interactable = isActive;
    }
    public LoadoutSO GetLoadoutSO(){
        return loadout;
    }
}
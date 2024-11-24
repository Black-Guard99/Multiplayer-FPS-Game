using UnityEngine;
public class AttachmentDisplayWindow : MonoBehaviour {
    [SerializeField] private AttachmentCustomizationUIBtn.AttachmentType attachmentType;
    [SerializeField] private GameObject scopeAttachmentWindow,muzzelAttachmentWindow,magzineAttachmentWindow,stockAttachmentWindow,skinAttachmentWindow,barrelAttachmentWindow;
    private bool isWindowsOpen;
    private void Start(){
        ClosedWindow();
    }
    public void OpenWindow(){
        isWindowsOpen = !isWindowsOpen;
        if(isWindowsOpen){
            scopeAttachmentWindow.SetActive(false);
            muzzelAttachmentWindow.SetActive(false);
            magzineAttachmentWindow.SetActive(false);
            stockAttachmentWindow.SetActive(false);
            skinAttachmentWindow.SetActive(false);
            barrelAttachmentWindow.SetActive(false);
            switch(attachmentType){
                case AttachmentCustomizationUIBtn.AttachmentType.Scope:
                    scopeAttachmentWindow.SetActive(true);
                    if(scopeAttachmentWindow.transform.TryGetComponent(out SpreadManager scopeSpread)){
                        scopeSpread.Spread();
                    }
                break;
                case AttachmentCustomizationUIBtn.AttachmentType.Muzzels:
                    muzzelAttachmentWindow.SetActive(true);
                    if(muzzelAttachmentWindow.transform.TryGetComponent(out SpreadManager MuzzelsSpread)){
                        MuzzelsSpread.Spread();
                    }
                break;
                case AttachmentCustomizationUIBtn.AttachmentType.Magzine:
                    magzineAttachmentWindow.SetActive(true);
                    if(magzineAttachmentWindow.transform.TryGetComponent(out SpreadManager magzineSpread)){
                        magzineSpread.Spread();
                    }
                break;
                case AttachmentCustomizationUIBtn.AttachmentType.Stock:
                    stockAttachmentWindow.SetActive(true);
                    if(stockAttachmentWindow.transform.TryGetComponent(out SpreadManager stockspread)){
                        stockspread.Spread();
                    }
                break;
                case AttachmentCustomizationUIBtn.AttachmentType.Barrel:
                    barrelAttachmentWindow.SetActive(true);
                    if(barrelAttachmentWindow.transform.TryGetComponent(out SpreadManager barrelSpread)){
                        barrelSpread.Spread();
                    }
                break;
                case AttachmentCustomizationUIBtn.AttachmentType.Skin:
                    skinAttachmentWindow.SetActive(true);
                    if(skinAttachmentWindow.transform.TryGetComponent(out SpreadManager skinSpread)){
                        skinSpread.Spread();
                    }
                break;

            }
        }else {
            ClosedWindow();
        }
    }
    public void ClosedWindow(){
        isWindowsOpen = false;
        scopeAttachmentWindow.SetActive(false);
        muzzelAttachmentWindow.SetActive(false);
        magzineAttachmentWindow.SetActive(false);
        stockAttachmentWindow.SetActive(false);
        barrelAttachmentWindow.SetActive(false);
        skinAttachmentWindow.SetActive(false);
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AttachmentCustomizationUIBtn : MonoBehaviour {
    public enum AttachmentType{
        Scope,
        Muzzels,
        Magzine,
        Stock,
        Barrel,
        Skin,
    }
    [SerializeField] private AttachmentType attachmentType;
    [SerializeField] private AttachmentDisplayWindow attachmentDisplayWindow;


    [Header("Scopes Customizations")]
    [SerializeField] private ScopeSO scope;
    [SerializeField] private ScopeSystemMenuUI scopeSystemMenuUi;
    [Space(10)]
    [Header("Muzzels Customizations")]
    [SerializeField] private MuzzelSO muzzelSo;
    [SerializeField] private MuzzelSystemMenuUI muzzelSystemMenuUi;
    [Space(10)]
    [Header("Magzines Customizations")]
    [SerializeField] private MagzineSO magzineSo;
    [SerializeField] private MagzineSystemMenuUi magzineSystemMenuUi;
    [Space(10)]
    [Header("Stocks Customizations")]
    [SerializeField] private StockSO stockSo;
    [SerializeField] private StockSystemMenuUi stockSytemMenuUi;

    [Space(10)]
    [Header("Barrels Customization")]
    [SerializeField] private BarrelSO barrelSo;
    [SerializeField] private BarrelSystemMenuUI barrelSystemMenuUi;
    [Space(10)]
    [Header("Skins Customizations")]
    [SerializeField] private SkinSO skinSo;
    [SerializeField] private SkinAttachmentDisplay skinDiplayWindowUi;


    [Space(10)]
    [Header("Ui")]
    [SerializeField] private TextMeshProUGUI attachmentName;
    [SerializeField,Tooltip("Add Later")] private Image attachmentIcon;

    [ContextMenu("Spawn New Icon")]
    public void FindIconImage(){
        
        if(transform.Find("Icon").TryGetComponent(out Image image)){
            attachmentIcon = image;
        }
    }
    private void Start(){
        SetAttachmentName();
    }
    private void SetAttachmentName(){
        switch(attachmentType){
            case AttachmentType.Scope:
                attachmentName.SetText(string.Concat(scope.name.ToUpper()));
                if(scope.iconSprite != null){
                    attachmentIcon.sprite = scope.iconSprite;
                }else{
                    attachmentIcon.gameObject.SetActive(false);
                }
            break;
            case AttachmentType.Muzzels:
                attachmentName.SetText(string.Concat(muzzelSo.name.ToUpper()));
                if(muzzelSo.iconSprite != null){
                    attachmentIcon.sprite = muzzelSo.iconSprite;
                }else{
                    attachmentIcon.gameObject.SetActive(false);
                }
            break;
            case AttachmentType.Magzine:
                attachmentName.SetText(string.Concat(magzineSo.name.ToUpper()));
                if(magzineSo.iconSprite != null){
                    attachmentIcon.sprite = magzineSo.iconSprite;
                }else{
                    attachmentIcon.gameObject.SetActive(false);
                }
            break;
            case AttachmentType.Stock:
                attachmentName.SetText(string.Concat(stockSo.name.ToUpper()));
                if(stockSo.iconSprite != null){
                    attachmentIcon.sprite = stockSo.iconSprite;
                }else{
                    attachmentIcon.gameObject.SetActive(false);
                }
            break;
            case AttachmentType.Barrel:
                attachmentName.SetText(string.Concat(barrelSo.name.ToUpper()));
                if(barrelSo.iconSprite != null){
                    attachmentIcon.sprite = barrelSo.iconSprite;
                }else{
                    attachmentIcon.gameObject.SetActive(false);
                }
            break;
            case AttachmentType.Skin:
                attachmentName.SetText(string.Concat(skinSo.name.ToUpper()));
                // attachmentIcon.sprite = skinSo.iconSprite;
            break;
        }
    }
    public void AddAttachmentToGun(){
        switch(attachmentType){
            case AttachmentType.Scope:
                scopeSystemMenuUi.SetCurrentScope(scope);
            break;
            case AttachmentType.Muzzels:
                muzzelSystemMenuUi.SetCurrentMuzzel(muzzelSo);
            break;
            case AttachmentType.Magzine:
                magzineSystemMenuUi.SetCurrentMagzine(magzineSo);
            break;
            case AttachmentType.Stock:
                stockSytemMenuUi.SetCurrentStock(stockSo);
            break;
            case AttachmentType.Barrel:
                barrelSystemMenuUi.SetCurrentBarrels(barrelSo);
            break;
            case AttachmentType.Skin:
            break;
        }
        attachmentDisplayWindow.ClosedWindow();
    }

}
using UnityEngine;
public class WeponFullBody : MonoBehaviour {
    [SerializeField] private AttachmentSO attachmentCurrent;
    [SerializeField] private ScopeSystem scopeSystem;
    [SerializeField] private MuzzelSystem muzzelSystem;
    [SerializeField] private MagzineSystem magzineSystem;
    [SerializeField] private StockSystem stockSystem;
    [SerializeField] private BarrelSystem barrelSystem;
    private void Awake(){
        if(magzineSystem != null){
            if(attachmentCurrent.currentMagzine != null){
                SetMagCapcity(attachmentCurrent.currentMagzine.magzineType);
            }
        }
        if(scopeSystem != null){
            if(attachmentCurrent.currentScope != null){
                SetCurrentScope(attachmentCurrent.currentScope.scopeType);
            }
        }
        if(muzzelSystem != null){
            if(attachmentCurrent.currentMuzzel != null){
                SetMuzzel(attachmentCurrent.currentMuzzel.muzzelType);
            }
        }
        if(stockSystem != null){
            if(attachmentCurrent.currentStock != null){
                stockSystem.SetCurrentStock(attachmentCurrent.currentStock.stockType);
            }
        }
        if(barrelSystem != null){
            if(attachmentCurrent.currnetBarrel != null){
                barrelSystem.SetCurrentBarrels(attachmentCurrent.currnetBarrel.barrelAttachmentType);
            }
        }
    }
    public void SetCurrentScope(ScopeSO.ScopeType scopeType){
        scopeSystem.SetCurrentScope(scopeType);
    }
    public void SetMuzzel(MuzzelSO.MuzzelType muzzelType){
        muzzelSystem.SetCurrentMuzzels(muzzelType);
    }
    public void SetMagCapcity(MagzineSO.MagzineType magzineType){
        magzineSystem.SetCurrentMagzine(magzineType);
    }
}
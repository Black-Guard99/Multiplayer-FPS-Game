using UnityEngine;
[CreateAssetMenu(menuName = "Configs/LoadoutSO", fileName = "LoadoutSO")]
public class LoadoutSO : ScriptableObject {
    public bool isActiveLoadout;
    // public GunSO primary,secondary,mele,lethalThrowable,nonLethalThowable;
    public LoadGunAttachments primaryGunAttachments,secondaryGunAttachments,meleWeaponAttachments,lethalThrowabl,nonLethalThrowabl;
}

[System.Serializable]
public class LoadGunAttachments{
    public GunSO gunCurrent;
    public AttachmentSO currentGunAttachmentForLoadout;
    public void SetAttachmetnForTheWeapons(){
        currentGunAttachmentForLoadout.currentMagzine.magzineType = currentAttachmetMagzineType;
        currentGunAttachmentForLoadout.currentScope.scopeType = currentAttachmetScopeType;
        currentGunAttachmentForLoadout.currentStock.stockType = currentAttachmetStockType;
        currentGunAttachmentForLoadout.currentMuzzel.muzzelType = currentAttachmetMuzzelType;
    }
    public ScopeSO.ScopeType currentAttachmetScopeType;
    public MuzzelSO.MuzzelType currentAttachmetMuzzelType;
    public MagzineSO.MagzineType currentAttachmetMagzineType;
    public StockSO.StockType currentAttachmetStockType;
    public BarrelSO.BarrelAttachmentType currentAttachmetBarrelsType;
}
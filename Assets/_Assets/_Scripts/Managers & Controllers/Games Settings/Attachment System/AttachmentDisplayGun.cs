using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class AttachmentDisplayGun : MonoBehaviour {
    [SerializeField] private List<ScopeSO> scopesList;
    [SerializeField] private List<MuzzelSO> muzzelList;
    [SerializeField] private List<MagzineSO> magzineSoList;

    [SerializeField] private List<StockSO> stocksList;
    [SerializeField] private List<BarrelSO> barrelsList;
    [SerializeField] private AttachmentSO gunAttachment;
    [SerializeField] private LoadoutSO currentLoadout;
    public void SetScope(ScopeSO scopeSO){
        if(scopesList.Contains(scopeSO)){
            gunAttachment.currentScope = scopeSO;
        }
    }
    public void SetMuzzel(MuzzelSO muzzelsSo){
        if(muzzelList.Contains(muzzelsSo)){
            gunAttachment.currentMuzzel = muzzelsSo;
        }
    }
    public void SetMagzine(MagzineSO magzineSo){
        if(magzineSoList.Contains(magzineSo)){
            gunAttachment.currentMagzine = magzineSo;
        }
    }

    public void SetStock(StockSO stockSO) {
        if(stocksList.Contains(stockSO)){
            gunAttachment.currentStock = stockSO;
        }
    }

    public void SetBarrel(BarrelSO barrelSO) {
        if(barrelsList.Contains(barrelSO)){
            gunAttachment.currnetBarrel = barrelSO;
        }
    }
    public AttachmentSO GetAttachment(){
        return gunAttachment;
    }

    public void SetLoadout(LoadoutSO currentLoadout) {
        this.currentLoadout = currentLoadout;
    }
}
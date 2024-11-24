using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class ScopeSystem : MonoBehaviour {
    [SerializeField] private List<ScopeModel> scopeModelsList;
    [SerializeField] private ScopeModel activeScope;
    [SerializeField] private Gun wepon;
    public void Wepon_OnAim(bool aiming) {
        activeScope.Wepon_OnAim(aiming);
    }
    public void OnZoomChange(float zoomValuePercentage){
        activeScope.OnZoomChange(zoomValuePercentage);
    }
    public void SetCurrentScope(ScopeSO.ScopeType scopeType){
        for (int i = 0; i < scopeModelsList.Count; i++) {
            if(scopeModelsList[i].GetScopeType() == scopeType){
                activeScope = scopeModelsList[i];
                break;
            }
        }
        activeScope.SetScopeActive();
        wepon.onAim += Wepon_OnAim;
        RefreshScope();
    }
    private void RefreshScope(){
        for (int i = 0; i < scopeModelsList.Count; i++) {
            if(activeScope != scopeModelsList[i]){
                scopeModelsList[i].gameObject.SetActive(false);
            }else{
                scopeModelsList[i].gameObject.SetActive(true);
            }
        }
    }
    
}
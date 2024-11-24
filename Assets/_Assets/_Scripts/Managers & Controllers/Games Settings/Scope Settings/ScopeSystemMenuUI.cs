using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ScopeSystemMenuUI : MonoBehaviour {
    [SerializeField] private List<ScopeAttachmentDisplay> scopeModelsList;
    [SerializeField] private ScopeAttachmentDisplay currentScopeAttachment;
    [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    private void Awake(){
        SetCurrentScope(attachmentDisplayGun.GetAttachment().currentScope);
    }
    public void SetCurrentScope(ScopeSO scopeType){
        for (int i = 0; i < scopeModelsList.Count; i++) {
            if(scopeModelsList[i].GetScopeType() == scopeType){
                currentScopeAttachment = scopeModelsList[i];
                break;
            }
        }
        RefreshScope();
    }
    private void RefreshScope(){
        for (int i = 0; i < scopeModelsList.Count; i++) {
            if(currentScopeAttachment != scopeModelsList[i]){
                scopeModelsList[i].gameObject.SetActive(false);
            }else{
                scopeModelsList[i].gameObject.SetActive(true);
            }
        }
        if(currentScopeAttachment != null){
            attachmentDisplayGun.SetScope(currentScopeAttachment.GetScopeType());
        }
    }
}
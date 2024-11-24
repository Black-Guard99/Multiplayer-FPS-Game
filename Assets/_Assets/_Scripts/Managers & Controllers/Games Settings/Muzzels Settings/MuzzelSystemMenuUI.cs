using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class MuzzelSystemMenuUI : MonoBehaviour {
    [SerializeField] private List<MuzzelAttachmentDisplay> muzzelAttachmentList;
    [SerializeField] private MuzzelAttachmentDisplay muzzelScopeAttachment;
    [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    private void Awake(){
        SetCurrentMuzzel(attachmentDisplayGun.GetAttachment().currentMuzzel);
    }
    public void SetCurrentMuzzel(MuzzelSO muzzelType){
        // Calling form Ui Button;
        for (int i = 0; i < muzzelAttachmentList.Count; i++) {
            if(muzzelAttachmentList[i].GetMuzzel() == muzzelType){
                muzzelScopeAttachment = muzzelAttachmentList[i];
                break;
            }
        }
        RefreshMuzzel();
    }
    private void RefreshMuzzel(){
        for (int i = 0; i < muzzelAttachmentList.Count; i++) {
            if(muzzelScopeAttachment != muzzelAttachmentList[i]){
                muzzelAttachmentList[i].gameObject.SetActive(false);
            }else{
                muzzelAttachmentList[i].gameObject.SetActive(true);
            }
        }
        attachmentDisplayGun.SetMuzzel(muzzelScopeAttachment.GetMuzzel());
    }

}
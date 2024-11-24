using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BarrelSystemMenuUI : MonoBehaviour {
    [SerializeField] private List<BarrelAttachmentDisplay> barrelAttachmentList;
    [SerializeField] private BarrelAttachmentDisplay barrelAttachment;
    [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    private void Awake(){
        SetCurrentBarrels(attachmentDisplayGun.GetAttachment().currnetBarrel);
    }
    public void SetCurrentBarrels(BarrelSO barrelType){
        // Calling form Ui Button;
        for (int i = 0; i < barrelAttachmentList.Count; i++) {
            if(barrelAttachmentList[i].GetBarrelSo() == barrelType){
                barrelAttachment = barrelAttachmentList[i];
                break;
            }
        }
        RefreshBarrels();
    }
    private void RefreshBarrels(){
        for (int i = 0; i < barrelAttachmentList.Count; i++) {
            if(barrelAttachment != barrelAttachmentList[i]){
                barrelAttachmentList[i].gameObject.SetActive(false);
            }else{
                barrelAttachmentList[i].gameObject.SetActive(true);
            }
        }
        if(attachmentDisplayGun == null) return;
        if(barrelAttachment.GetBarrelSo() != null){
            attachmentDisplayGun.SetBarrel(barrelAttachment.GetBarrelSo());
        }
    }
}
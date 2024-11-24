using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MagzineSystemMenuUi : MonoBehaviour {
    [SerializeField] private List<MagzineAtttachmentDisplay> magzineAttachmentList;
    [SerializeField] private MagzineAtttachmentDisplay magzineAttachment;
    [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    private void Awake(){
        SetCurrentMagzine(attachmentDisplayGun.GetAttachment().currentMagzine);
    }
    public void SetCurrentMagzine(MagzineSO magzineType){
        // Calling form Ui Button;
        for (int i = 0; i < magzineAttachmentList.Count; i++) {
            if(magzineAttachmentList[i].GetMagzine() == magzineType){
                magzineAttachment = magzineAttachmentList[i];
                break;
            }
        }
        RefreshMagzine();
    }
    private void RefreshMagzine(){
        for (int i = 0; i < magzineAttachmentList.Count; i++) {
            if(magzineAttachment != magzineAttachmentList[i]){
                magzineAttachmentList[i].gameObject.SetActive(false);
            }else{
                magzineAttachmentList[i].gameObject.SetActive(true);
            }
        }
        attachmentDisplayGun.SetMagzine(magzineAttachment.GetMagzine());
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class StockSystemMenuUi : MonoBehaviour {
    [SerializeField] private List<StockAttachmentDisplay> stockAttachmentList;
    [SerializeField] private StockAttachmentDisplay stockAttachment;
    [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    private void Awake(){
        SetCurrentStock(attachmentDisplayGun.GetAttachment().currentStock);
    }
    public void SetCurrentStock(StockSO stockType){
        // Calling form Ui Button;
        for (int i = 0; i < stockAttachmentList.Count; i++) {
            if(stockAttachmentList[i].GetStock() == stockType){
                stockAttachment = stockAttachmentList[i];
                break;
            }
        }
        RefreshStocks();
    }
    private void RefreshStocks(){
        for (int i = 0; i < stockAttachmentList.Count; i++) {
            if(stockAttachment != stockAttachmentList[i]){
                stockAttachmentList[i].gameObject.SetActive(false);
            }else{
                stockAttachmentList[i].gameObject.SetActive(true);
            }
        }
        attachmentDisplayGun.SetStock(stockAttachment.GetStock());
    }
}
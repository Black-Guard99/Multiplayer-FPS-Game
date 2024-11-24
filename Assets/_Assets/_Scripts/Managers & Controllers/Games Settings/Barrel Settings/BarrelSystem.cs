using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BarrelSystem : MonoBehaviour {
    [SerializeField] private List<BarrelModel> barrelsModelsList;
    private BarrelModel currentMagzine;

    public void SetCurrentBarrels(BarrelSO.BarrelAttachmentType barrelsType){
        for (int i = 0; i < barrelsModelsList.Count; i++) {
            if(barrelsModelsList[i].GetBarrelsType() == barrelsType){
                currentMagzine = barrelsModelsList[i];
                break;
            }
            
        }
        RefreshBarrels();
    }

    

    private void RefreshBarrels(){
        for (int i = 0; i < barrelsModelsList.Count; i++) {
            if(currentMagzine != barrelsModelsList[i]){
                barrelsModelsList[i].gameObject.SetActive(false);
            }else{
                barrelsModelsList[i].gameObject.SetActive(true);
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class MagzineSystem : MonoBehaviour {
    [SerializeField] private List<MagzineModel> magzineModelsList;
    private MagzineModel currentMagzine;

    public void SetCurrentMagzine(MagzineSO.MagzineType muzzelType){
        for (int i = 0; i < magzineModelsList.Count; i++) {
            if(magzineModelsList[i].GetMagzineType() == muzzelType){
                currentMagzine = magzineModelsList[i];
                break;
            }
            
        }
        RefreshMuzzels();
    }

    

    private void RefreshMuzzels(){
        for (int i = 0; i < magzineModelsList.Count; i++) {
            if(currentMagzine != magzineModelsList[i]){
                magzineModelsList[i].gameObject.SetActive(false);
            }else{
                magzineModelsList[i].gameObject.SetActive(true);
            }
        }
    }
}
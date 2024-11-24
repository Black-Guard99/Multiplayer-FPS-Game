using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MuzzelSystem : MonoBehaviour {
    [SerializeField] private List<MuzzelsModel> muzzelsModelsList;
    private MuzzelsModel currentMuzzels;

    public void SetCurrentMuzzels(MuzzelSO.MuzzelType muzzelType){
        for (int i = 0; i < muzzelsModelsList.Count; i++) {
            if(muzzelsModelsList[i].GetMuzzelType() == muzzelType){
                currentMuzzels = muzzelsModelsList[i];
                break;
            }
            
        }
        RefreshMuzzels();
    }

    

    private void RefreshMuzzels(){
        for (int i = 0; i < muzzelsModelsList.Count; i++) {
            if(currentMuzzels != muzzelsModelsList[i]){
                muzzelsModelsList[i].gameObject.SetActive(false);
            }else{
                muzzelsModelsList[i].gameObject.SetActive(true);
            }
        }
    }
}
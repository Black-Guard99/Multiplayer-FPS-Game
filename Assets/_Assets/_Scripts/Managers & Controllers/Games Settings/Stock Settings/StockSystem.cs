using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class StockSystem : MonoBehaviour {
    [SerializeField] private List<StockModel> stockModelsList;
    private StockModel stockModel;

    public void SetCurrentStock(StockSO.StockType muzzelType){
        for (int i = 0; i < stockModelsList.Count; i++) {
            if(stockModelsList[i].GetStockType() == muzzelType){
                stockModel = stockModelsList[i];
                break;
            }
            
        }
        RefreshStock();
    }

    

    private void RefreshStock(){
        for (int i = 0; i < stockModelsList.Count; i++) {
            if(stockModel != stockModelsList[i]){
                stockModelsList[i].gameObject.SetActive(false);
            }else{
                stockModelsList[i].gameObject.SetActive(true);
            }
        }
    }
}
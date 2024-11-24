using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class StockModel : MonoBehaviour {
    [SerializeField] private StockSO stockSo;
    public StockSO.StockType GetStockType() {
        return stockSo.stockType;
    }
}
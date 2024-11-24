using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/Attachments/StockSO", fileName = "StockSO")]
public class StockSO : ScriptableObject {
    public enum StockType{
        None,
        Normal,
        Naked,
        CheekPad,

    }
    public StockType stockType;
    public Sprite iconSprite;
}
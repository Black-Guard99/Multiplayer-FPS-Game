using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class StockAttachmentDisplay : MonoBehaviour {
    [SerializeField] private StockSO stock;
    // [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    public StockSO GetStock() {
        return stock;
    }
}
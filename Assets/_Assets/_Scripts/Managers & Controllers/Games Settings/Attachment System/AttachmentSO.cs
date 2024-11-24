using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/AttachmentSO", fileName = "AttachmentSO")]
public class AttachmentSO : ScriptableObject {
    public ScopeSO currentScope;
    public MuzzelSO currentMuzzel;
    public MagzineSO currentMagzine;
    public StockSO currentStock;
    public BarrelSO currnetBarrel;
    public SkinSO currentSkin;
}


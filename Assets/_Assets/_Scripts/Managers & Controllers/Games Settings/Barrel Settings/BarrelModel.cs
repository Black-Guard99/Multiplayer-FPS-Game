using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BarrelModel : MonoBehaviour {
    [SerializeField] private BarrelSO magzineSo;
    public BarrelSO.BarrelAttachmentType GetBarrelsType() {
        return magzineSo.barrelAttachmentType;
    }
}
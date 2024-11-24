using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BarrelAttachmentDisplay : MonoBehaviour {
    [SerializeField] private BarrelSO barrelSo;
    // [SerializeField] private AttachmentDisplayGun attachmentDisplayGun;
    public BarrelSO GetBarrelSo(){
        return barrelSo;
    }
}
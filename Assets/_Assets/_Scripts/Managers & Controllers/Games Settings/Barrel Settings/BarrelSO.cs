using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/Attachments/BarrelSO", fileName = "BarrelSO")]
public class BarrelSO : ScriptableObject {
    public enum BarrelAttachmentType{
        None,
        PistolLaser,
        Tourch,
        RifleLaser,
    }
    public BarrelAttachmentType barrelAttachmentType;
    public Sprite iconSprite;
}
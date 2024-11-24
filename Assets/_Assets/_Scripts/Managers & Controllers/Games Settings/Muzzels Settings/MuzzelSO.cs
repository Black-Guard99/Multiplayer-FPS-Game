using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/Attachments/MuzzelSO", fileName = "MuzzelAttachments")]
public class MuzzelSO : ScriptableObject {
    public enum MuzzelType{
        None,
        Suppressor,
        Compensator,
        FlashHider,
    }
    public MuzzelType muzzelType;
    public Sprite iconSprite;
}
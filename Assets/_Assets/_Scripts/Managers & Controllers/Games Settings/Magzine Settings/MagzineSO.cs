using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

[CreateAssetMenu(menuName = "Configs/Attachments/MagzineSO", fileName = "MagzineSO")]
public class MagzineSO : ScriptableObject {
    public enum MagzineType{
        Normal,
        x2Mag,
        QuickMag
    }
    public MagzineType magzineType;
    public Sprite iconSprite;
}
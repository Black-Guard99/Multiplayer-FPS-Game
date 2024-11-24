using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/GunDisplaySO", fileName = "GunDisplaySO")]
public class GunDisplaySO : ScriptableObject {
    public Sprite gunSprite;
    public string gunName = "AKM";
    public GameObject displayPrefabs;
    public GunSO gunSo;
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MagzineModel : MonoBehaviour {
    [SerializeField] private MagzineSO magzineSo;
    public MagzineSO.MagzineType GetMagzineType() {
        return magzineSo.magzineType;
    }
}
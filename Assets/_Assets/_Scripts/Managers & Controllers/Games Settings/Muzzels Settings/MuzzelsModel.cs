using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class MuzzelsModel : MonoBehaviour {
    [SerializeField] private MuzzelSO muzzelSo;

    public MuzzelSO.MuzzelType GetMuzzelType(){
        return muzzelSo.muzzelType;
    }

}
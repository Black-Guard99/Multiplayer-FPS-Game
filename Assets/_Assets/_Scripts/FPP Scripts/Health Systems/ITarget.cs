using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public interface ITarget {
    void TakeHit(float damageValue,Vector3 hitPoint,int p_actor,Vector3 shooterPos,string username,string gunName);
}
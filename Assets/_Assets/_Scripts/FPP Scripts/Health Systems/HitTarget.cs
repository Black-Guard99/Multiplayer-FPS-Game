using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
public class HitTarget : MonoBehaviour,ITarget {
    [SerializeField] private HealthSystem bodyManager;
    public void TakeHit(float damageValue,Vector3 hitPoint,int p_actor,Vector3 shooterPos,string username,string gunName) {
        if(bodyManager.IsDead()) return;
        bodyManager.TakeDamageRPC(damageValue,hitPoint,p_actor,shooterPos,username,gunName);
    }
}
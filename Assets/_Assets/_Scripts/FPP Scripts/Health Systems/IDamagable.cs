using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;

public interface IDamagable {
    public float currentHealth {get;}
    public float totalHealth{get;}
    public delegate void TakeDamgageEvent(float damageValueNormalized,Vector3 damagePoint,Vector3 shooterPos,string username,string gunName);
    public event TakeDamgageEvent OnTakeDamgage;
    public delegate void DeathEvent(Vector3 deathPosition,int p_actor,string username,string gunName);
    public event DeathEvent OnDeath;
    public void TakeDamage(float damageValue,Vector3 damagePoint,int p_actor,Vector3 shooterPos,string username,string gunName);
    public void TakeDamageRPC(float damageValue,Vector3 damagePoint,int p_actor,Vector3 shooterPos,string username,string gunName);
}

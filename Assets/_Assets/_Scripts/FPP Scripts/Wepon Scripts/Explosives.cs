using Photon.Pun;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class Explosives : MonoBehaviour,ITarget {
    [SerializeField] private ParticleSystem explosionEffect;
    private HealthSystem damagable;
    private MeshRenderer meshRenderer;
    private void Awake(){
        damagable = GetComponent<HealthSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
        if(explosionEffect != null){
            explosionEffect.gameObject.SetActive(false);
        }
    }
    private void Start(){
        damagable.OnDeath += (Vector3 deathPoint,int p_actor,string username,string gunName)=>{
            meshRenderer.enabled = false;
            // ObjectPoolingManager.Current.SpawnEffectOverNetwork("Explosives",transform.position,Quaternion.Euler(new Vector3(-90,0,0)));
            if(explosionEffect != null){
                explosionEffect.gameObject.SetActive(true);
                explosionEffect.Play();
            }
            DestroyMySelfWithDelay(3f);
        };
    }
    public void DestroyMySelfWithDelay(float delay = 0){
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }

    private void DestroyNow() {
        CancelInvoke(nameof(DestroyNow));
        gameObject.SetActive(false);
    }

    public void TakeHit(float damageValue, Vector3 hitPoint,int p_actor,Vector3 shooterPos,string username,string gunName) {
        if(damagable.IsDead()) return;
        if(damagable.GetHealthNormalized() <= .5){
            CrossHairMovement.Current.ShowHitCrossHair(Color.red);
        }else{
            CrossHairMovement.Current.ShowHitCrossHair(Color.cyan);
        }
        damagable.TakeDamageRPC(damageValue,hitPoint,p_actor,shooterPos,username,gunName);
    }

    
}
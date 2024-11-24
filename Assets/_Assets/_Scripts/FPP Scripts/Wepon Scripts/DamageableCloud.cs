using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Photon.Pun;
using System;

public class DamageableCloud : MonoBehaviour{
    [SerializeField] private float maxDamageTime = 2f,lifeTime = 10f;
    [SerializeField] private float damageRadius = 10f;
    [SerializeField] private LayerMask damableMask;
    [SerializeField] private GunSO gunSo;
    private float damageTimer;
    private bool show;

    public Action OnTimerEnd;

    private void Awake(){
        damageTimer = maxDamageTime;
    }
    private void Start(){
        DestroyMySelfWithDelay(10f);
    }
    private void Update(){
        if(damageTimer <= 0f){
            damageTimer = maxDamageTime;
            Collider[] colis = Physics.OverlapSphere(transform.position,damageRadius,damableMask,QueryTriggerInteraction.Collide);
            if(colis.Length > 0){
                Collider damageCollider = colis[Random.Range(0,colis.Length)];
                if(damageCollider.TryGetComponent(out ITarget target)){
                    float distance = Vector3.Distance(transform.position,damageCollider.transform.position);
                    float normalized = 1 - distance / damageRadius;
                    target.TakeHit(gunSo.shootConfig.damageConfig.bodyDamageAmount * normalized,transform.position,-1,transform.position,gunSo.playerProfile.username,gunSo.playerProfile.gunName);
                }
                foreach(Collider col in colis){
                    if(col.TryGetComponent(out PoisonEffectController poisonEffectController)){
                        poisonEffectController.ShowPoison();
                    }
                }
                
            }
        }else{
            damageTimer -= Time.deltaTime;
        }
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,damageRadius);
    }

    public virtual void DestroyMySelfWithDelay(float delay = 0){
        CancelInvoke(nameof(DestroyNow));
        Invoke(nameof(DestroyNow),delay);
    }

    public virtual void DestroyNow(){
        CancelInvoke(nameof(DestroyNow));
        OnTimerEnd?.Invoke();
    }

    public void Show() {
        show = true;
        DestroyMySelfWithDelay(lifeTime);
    }
}
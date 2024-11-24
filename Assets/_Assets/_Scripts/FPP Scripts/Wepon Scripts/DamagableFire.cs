using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamagableFire : MonoBehaviour{
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float maxDamageTime = 2f;
    [SerializeField] private float damageRadius = 10f;
    [SerializeField] private GunSO gunSo;
    [SerializeField] private LayerMask damableMask;
    [SerializeField] private bool show;
    private float damageTimer;
    public Action OnTimerEnd;
    private void Awake(){
        damageTimer = maxDamageTime;
        show = false;
    }
    private void Update(){
        if(!show){
            return;
        }
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
        // gameObject.SetActive(false);
        // PhotonNetwork.Destroy(gameObject);
        OnTimerEnd?.Invoke();
    }

    public void Show() {
        show = true;
        DestroyMySelfWithDelay(lifeTime);
    }
}
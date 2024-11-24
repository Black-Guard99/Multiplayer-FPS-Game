using UnityEngine;
using Random = UnityEngine.Random;
public class Grenade : ThrowableObject {
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private float damageRadius = 10f;
    [SerializeField] private LayerMask damableMask;
    [SerializeField] private GunSO gunSo;
    public override void Throw(float throwForce,Vector3 forceDirection,float time,float drag,float mass,OffScreenIndicator indicator){
        base.Throw(throwForce,forceDirection,time,drag,mass,indicator);
    }
    public override void DestroyNow() {
        rb.isKinematic = true;
        transform.eulerAngles = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ShowHideVisual(false);
        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();
        Collider[] colis = Physics.OverlapSphere(transform.position,damageRadius,damableMask,QueryTriggerInteraction.Collide);
        if(colis.Length > 0){
            Collider damageCollider = colis[Random.Range(0,colis.Length)];
            if(damageCollider.TryGetComponent(out ITarget target)){
                float distance = Vector3.Distance(transform.position,damageCollider.transform.position);
                float normalized = 1 - distance / damageRadius;
                Debug.Log("Normalized value " + normalized);
                target.TakeHit(gunSo.shootConfig.damageConfig.bodyDamageAmount * normalized,transform.position,-1,transform.position,gunSo.playerProfile.username,gunSo.playerProfile.gunName);
            }
        }
        CancelInvoke(nameof(ActualDestroy));
        Invoke(nameof(ActualDestroy),5f);
    }
    private void ActualDestroy(){
        base.DestroyNow();
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,damageRadius);
    }
}
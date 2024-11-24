using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FlashBang : ThrowableObject {
    [SerializeField] private float damageRadius = 10f;
    [SerializeField] private LayerMask damableMask;
    public override void Throw(float throwForce,Vector3 forceDirection,float time,float drag,float mass, OffScreenIndicator indicator){
        base.Throw(throwForce,forceDirection,time,drag,mass,indicator);
    }
    public override void DestroyNow() {
        rb.isKinematic = true;
        // ObjectPoolingManager.Current.SpawnEffectOverNetwork("FlashEffect",transform.position,Quaternion.identity);
        ShowHideEffect(true);
        ShowHideVisual(false);
        Collider[] colis = Physics.OverlapSphere(transform.position,damageRadius,damableMask,QueryTriggerInteraction.UseGlobal);
        if(colis.Length > 0){
            foreach(Collider coli in colis){
                if(coli.TryGetComponent(out FlashBangShineController flashBangShineController)){
                    // target.TakeHit(1f/colis.Length,transform.position,-1);
                    flashBangShineController.StartShine();
                }
            }
        }
        base.DestroyNow();
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,damageRadius);
    }
}
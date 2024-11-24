using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class NuclearGrenade : ThrowableObject {
    [SerializeField] private DamageableCloud damageableCloud;
    private void Start(){
        damageableCloud.OnTimerEnd += ()=>{
            base.DestroyNow();
        };
    }
    public override void Throw(float throwForce,Vector3 forceDirection,float time,float drag,float mass,OffScreenIndicator indicator){
        base.Throw(throwForce,forceDirection,time,drag,mass,indicator);
    }
    public override void DestroyNow() {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        ShowHideVisual(false);
        ShowHideEffect(true);
        damageableCloud.Show();
    }
    
}
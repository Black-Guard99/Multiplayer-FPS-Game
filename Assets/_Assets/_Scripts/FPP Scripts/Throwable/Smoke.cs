using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Smoke : ThrowableObject {
    
    public override void Throw(float throwForce,Vector3 forceDirection,float time,float drag,float mass,OffScreenIndicator indicator){
        base.Throw(throwForce,forceDirection,time,drag,mass,indicator);
    }
    public override void DestroyNow() {
        rb.isKinematic = true;
        transform.eulerAngles = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        ShowHideEffect(true);
        ShowHideVisual(false);
        CancelInvoke(nameof(ActualDestroy));
        Invoke(nameof(ActualDestroy),5f);
    }
    private void ActualDestroy(){
        base.DestroyNow();
    }
}
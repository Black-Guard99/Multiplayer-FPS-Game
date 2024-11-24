using UnityEngine;
public class Molotove : ThrowableObject{
    [SerializeField] private LayerMask fireLayer;
    [SerializeField] private DamagableFire damagableFire;
    private bool canExplode;
    public override void Throw(float throwForce,Vector3 forceDirection,float time,float drag,float mass,OffScreenIndicator indicator){
        CancelInvoke(nameof(SetCanExplode));
        Invoke(nameof(SetCanExplode),.3f);
        base.Throw(throwForce,forceDirection,5f,drag,mass,indicator);
        
    }
    private void Start(){
        damagableFire.OnTimerEnd += ()=>{
            base.DestroyNow();
        };
    }
    private void SetCanExplode(){
        canExplode = true;
    }
    private void OnCollisionEnter(Collision coli){
        if(canExplode){
            if(Physics.Raycast(transform.position + Vector3.up * .4f,Vector3.down,out RaycastHit hit,Mathf.Infinity,fireLayer,QueryTriggerInteraction.UseGlobal)){
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                ShowHideVisual(false);
                ShowHideEffect(true);
                transform.eulerAngles = Vector3.zero;
                damagableFire.Show();
            }
        }
    }

    public override void DestroyNow(){
        if(Physics.Raycast(transform.position + Vector3.up * .4f,Vector3.down,out RaycastHit hit,Mathf.Infinity,fireLayer,QueryTriggerInteraction.UseGlobal)){
            rb.isKinematic = true;
        }
        canExplode = false;
        base.DestroyNow();
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BodyPartsHitTarget : MonoBehaviour,ITarget {
    [SerializeField] private HealthSystem bodyHealth;
    [SerializeField] private bool isHead = false;
    private Rigidbody rb;
    private void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    private void Start(){
        bodyHealth.OnTakeDamgage += (float damageValue,Vector3 damagePoint,Vector3 shooterPos,string username,string gunName)=>{
            if(isHead){
                CrossHairMovement.Current.ShowHitCrossHair(Color.red);
            }else{
                CrossHairMovement.Current.ShowHitCrossHair(Color.yellow);
            }
        };
    }
    
    public bool IsHead(){
        return isHead;
    }
    public void OnHit(Vector3 hitPoint){
        rb.AddForceAtPosition(-transform.forward * 40f,hitPoint,ForceMode.Impulse);
    }
    public void TakeHit(float damageValue,Vector3 hitPoint,int p_actor,Vector3 shooterPos,string username,string gunName) {
        if(bodyHealth.IsDead()) return;
        if(isHead) damageValue += 10;
        bodyHealth.TakeDamageRPC(damageValue,hitPoint,p_actor,shooterPos,username,gunName);
    }
}
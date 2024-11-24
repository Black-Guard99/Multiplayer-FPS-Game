using System;
using Photon.Pun;
using UnityEngine;
[DisallowMultipleComponent]
public class HealthSystem : MonoBehaviourPunCallbacks,IPunObservable, IDamagable {
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float health;
    [SerializeField] private float previousHealth;
    [SerializeField] private bool canRegenerate = true;
    [SerializeField] private float regenaratationSpeed = 20f;
    public float currentHealth{get => health;private set => health = value; }
    public float totalHealth {get => maxHealth; private set => maxHealth = value;}
    public float previousHealthAmount {get => previousHealth; private set => previousHealth = value;}
    public event IDamagable.TakeDamgageEvent OnTakeDamgage;
    public event IDamagable.DeathEvent OnDeath;
    public event Action OnHealthRegenerating;
    private bool isDead;
    private bool regenarate;
    private ProfileData profileData;
    private void Awake(){
        ResetHealth();
    }
    [PunRPC]
    public void TakeDamage(float damageValue,Vector3 deathPoint,int p_actor,Vector3 shooterPos,string username,string gunName) {
        float damageTaken = Mathf.Clamp(damageValue,0,currentHealth);
        previousHealth = currentHealth;
        currentHealth -= damageTaken;
        regenarate = false;
        CancelInvoke(nameof(StartRegeneratation));
        Invoke(nameof(StartRegeneratation),6f);
        // Debug.LogError("Took Damage By " + transform.name);
        OnTakeDamgage?.Invoke(GetHealthNormalized(),deathPoint,shooterPos,username,gunName);
        if(currentHealth <= 0 && damageTaken != 0 && !isDead){
            isDead = true;
            OnDeath?.Invoke(deathPoint,p_actor,username,gunName);
        }
    }
    public bool IsDead(){
        return isDead;
    }
    public void ResetHealth(){
        isDead = false;
        health = maxHealth;
        previousHealth = maxHealth;
        currentHealth = maxHealth;
    }
    public float GetHealthNormalized(){
        return currentHealth / maxHealth;
    }
    private void StartRegeneratation(){
        if(canRegenerate){
            regenarate = true;
        }
    }
    private void Update(){
        if(regenarate){
            photonView.RPC(nameof(HealthRegenrate),RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void HealthRegenrate(){
        previousHealth += Time.deltaTime * regenaratationSpeed;
        currentHealth += Time.deltaTime * regenaratationSpeed;
        if(previousHealth >= maxHealth){
            previousHealth = maxHealth;
        }
        if(currentHealth >= maxHealth){
            currentHealth = maxHealth;
        }
        if(previousHealth >= maxHealth && currentHealth >= maxHealth){
            regenarate = false;
        }
        OnHealthRegenerating?.Invoke();
    }
    public void TakeDamageRPC(float damageValue,Vector3 hitPoint,int p_actor,Vector3 shooterPos,string username,string gunName){
        photonView.RPC(nameof(TakeDamage),RpcTarget.AllBuffered,damageValue,hitPoint,p_actor,shooterPos,username,gunName);
    }
    public float GetInverseHealthNormalized(){
        return 1 - currentHealth / maxHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting){
            stream.SendNext(currentHealth);
        }else{
            currentHealth = (float)stream.ReceiveNext();
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Photon.Pun;
using UnityEngine.UI;

public class BotsHealthBar : MonoBehaviourPun, IPunObservable {
    [SerializeField] private GameObject opponentHealthBar;
    [SerializeField] private Image healthBar;
    [SerializeField] private float dissappearTime = 10f;
    private bool showOpponentHealthBar;
    private float healthNormalized;
    private HealthSystem playerHealthSystem;
    private void Awake(){
        playerHealthSystem = GetComponent<HealthSystem>();
    }
    private void Start(){
        playerHealthSystem.OnTakeDamgage += (float healthNormalized,Vector3 damagePoint,Vector3 shooterPos,string username,string gunName)=>{
            photonView.RPC(nameof(UpdateHealthValue),RpcTarget.All);
        };
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting){
            stream.SendNext(showOpponentHealthBar);
            stream.SendNext(healthNormalized);
        }else{
            showOpponentHealthBar = (bool)stream.ReceiveNext();
            healthNormalized = (float)stream.ReceiveNext();
            opponentHealthBar.SetActive(showOpponentHealthBar);
            healthBar.fillAmount = healthNormalized;
        }
    }
    private void HideHealthBar(){
        showOpponentHealthBar = false;
    }
    [PunRPC]
    private void UpdateHealthValue(){
        showOpponentHealthBar = true;
        this.healthNormalized = playerHealthSystem.GetHealthNormalized();
        opponentHealthBar.SetActive(showOpponentHealthBar);
        healthBar.fillAmount = healthNormalized;
        CancelInvoke(nameof(HideHealthBar));
        Invoke(nameof(HideHealthBar),dissappearTime);
    }
}
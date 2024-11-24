using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class TimeController : MonoBehaviourPunCallbacks,IPunObservable {
    [SerializeField] private float maxTime;
    [SerializeField] private bool startTime;
    private float currentTime;
    private void Start(){
        currentTime = maxTime;
    }
    public void StarTime(){
        startTime = true;
        currentTime = maxTime;
    }
    private void Update(){
        if(!startTime) return;
        currentTime -= Time.deltaTime;
        if(currentTime <= 0){
            currentTime = 0;
        }
        float minit = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        
        // UIControllers.Current?.DisplayTime(minit,seconds);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting){
            stream.SendNext(currentTime);
        }else{
            currentTime = (float)stream.ReceiveNext();
        }
    }
}
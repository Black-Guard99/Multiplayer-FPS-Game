using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UI;
public class AI_PlayerMovement : MonoBehaviourPun {
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 2f;
    [Header("External Referances")]
    [SerializeField] private bool isAI;
    public ProfileData playerProfile;
    public int actorNumber;
    public int ViewID;
    [SerializeField] protected TextMeshProUGUI playerNickName;
    [SerializeField] protected bool awayTeam;
    [SerializeField] protected Image[] teamIndicators;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private ParticleSystem bloodEffect;
    [SerializeField] private BodyAnimationManager bodyAnimationManager;
    [SerializeField] private AIWeaponAnimationManager aIWeaponAnimationManager;
    protected PlayerData playerData;
    protected bool isReady;
    private NavMeshAgent agent;
    private Transform currentTarget;
    private void Awake(){
        ViewID = photonView.ViewID;
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start(){
        if(BotSpawnManager.current.GetAllBotsList().Count > 0){
            Transform newTarget = BotSpawnManager.current.GetAllBotsList()[Random.Range(0,BotSpawnManager.current.GetAllBotsList().Count)].transform; 
            if(newTarget != transform){
                currentTarget = newTarget;
            }
        }
        healthSystem.OnDeath += OnDeath_HealthSystem;
        healthSystem.OnTakeDamgage += OnTakeDamgage_HealthSystem;
    }
    private void OnTakeDamgage_HealthSystem(float damageValueNormalized,Vector3 damagePoint,Vector3 shooterPos,string username,string gunName){
        Debug.Log("Took Damge " + healthSystem.currentHealth + " by " + transform.name);
        photonView.RPC(nameof(PlayBloodEffect),RpcTarget.AllBuffered,damagePoint);
    }
    private void Update(){
        switch(MatchHandler.Current.GetGameState()){
            case GameState.Starting:
            case GameState.Ending:
                return;
        }
        if(currentTarget != null){
            if(Vector3.Distance(transform.position ,currentTarget.position) <= agent.stoppingDistance){
                currentTarget = BotSpawnManager.current.GetAllBotsList()[Random.Range(0,BotSpawnManager.current.GetAllBotsList().Count)].transform; 
                agent.isStopped = true;
            }else{
                agent.isStopped = false;
                MoveToTarget(currentTarget);
            }
            if(agent.isStopped){
                bodyAnimationManager.SetSpeed(0f,1f);
                aIWeaponAnimationManager.SetSpeed(1f);
            }
        }else{
            currentTarget = BotSpawnManager.current.GetAllBotsList()[Random.Range(0,BotSpawnManager.current.GetAllBotsList().Count)].transform; 
        }
    }

    private void OnDeath_HealthSystem(Vector3 deathPosition, int p_actor, string username, string gunName) {
        // MatchHandler.Current?.SpawnBot(awayTeam ? SpawningArea.SpawnAreaType.Away : SpawningArea.SpawnAreaType.Home);
        Debug.Log("Bot Destoryed");
        // BotSpawnManager.current.RemoveEnemyFromList(this.transform);
        // PhotonNetwork.Destroy(this.gameObject);
        BotSpawnManager.current?.OnBotDeath(deathPosition,p_actor,username,gunName,this);
    }

    private void MoveToTarget(Transform newTarget){
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        agent.SetDestination(newTarget.position);
        
    }

    public void TrySync(string newName,bool IsAwayTeam,int actorNumber) {
        // playerProfile.username = newName;
        this.actorNumber = actorNumber;
        Debug.LogError("Bot Actor Number " + actorNumber);
        photonView.RPC(nameof(SyncProfile), RpcTarget.AllBuffered, newName);
        switch(MatchHandler.Current.GetGameSettingsSO().gameMode){
            case GameMode.FFA:
            case GameMode.NIGHTMARE:
                playerData.SetPlayerType(PlayerData.PlayerType.Frindely);
                ColorTeamIndicators(Random.ColorHSV());
            break;
            case GameMode.TDM:
                photonView.RPC(nameof(SyncTeam), RpcTarget.AllBuffered,IsAwayTeam);
            break;
        }
        /* if(photonView.IsMine){
            if(MatchHandler.Current.GetGameSettingsSO().gameMode == GameMode.FFA){
            }else{
                this.awayTeam = IsAwayTeam;
            }
        } */
        // photonView.RPC(nameof(ShowNickName),RpcTarget.AllBuffered);
    }

    private void ColorTeamIndicators (Color p_color) {
        foreach (Image renderer in teamIndicators) {
            renderer.color = p_color;
        }
    }
    [PunRPC]
    private void SyncProfile(string p_username) {
        playerProfile = new ProfileData(p_username,0,0);
    }

    [PunRPC]
    private void SyncTeam(bool p_awayTeam) {
        awayTeam = p_awayTeam;
        if (awayTeam){
            ColorTeamIndicators(Color.red);
        }else{
            ColorTeamIndicators(Color.blue);
        }
    }
    [PunRPC]
    private void PlayBloodEffect(Vector3 hitPoint){
        bloodEffect.gameObject.SetActive(true);
        bloodEffect.transform.position = hitPoint;
        bloodEffect.Play();
    }
    [PunRPC]
    private void ShowNickName(){
        playerNickName.text = playerProfile.username;
        Debug.Log("Bot " + playerProfile.username + " Id " + photonView.ViewID);
    }

}
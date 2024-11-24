using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BotSpawnManager : MonoBehaviour {
    [SerializeField] private Dictionary<int ,int> botDatasList;
    public static BotSpawnManager current{get;private set;}
    [SerializeField] private GameObject aiBotPrefab;
    [SerializeField] private Transform[] botSpawnPoint;
    [SerializeField] private List<Transform> allBotsAndPlayers;
    private void Awake(){
        botDatasList = new Dictionary<int, int>();
        if(current == null){
            current = this;
        }else{
            Destroy(current);
        }
    }
    public void SpawnBot(GameMode gameMode,int botsAmount, int firstActorNumber){
        int BotCount = firstActorNumber;
        if(gameMode == GameMode.FFA){
            for (int i = 0; i < botsAmount; i++) {
                GameObject botPlayer = PhotonNetwork.Instantiate(aiBotPrefab.name, botSpawnPoint[Random.Range(0,botSpawnPoint.Length)].position, Quaternion.identity);
                if(botPlayer.TryGetComponent(out AI_PlayerMovement aI_PlayerMovement)){
                    aI_PlayerMovement.TrySync(string.Concat("Bot ",Random.Range(10,1000000)),MatchHandler.Current.GetGameSettingsSO().IsAwayTeam,BotCount);
                    botDatasList.Add(aI_PlayerMovement.ViewID,aI_PlayerMovement.actorNumber);
                    MatchHandler.Current?.NewPlayer_S(aI_PlayerMovement.playerProfile);
                }
                BotCount ++;
            }
        }
        foreach(GameObject newPlayers in GameObject.FindGameObjectsWithTag("Player")){
            if(!allBotsAndPlayers.Contains(newPlayers.transform)){
                allBotsAndPlayers.Add(newPlayers.transform);
            }
        }
    }
    public void RemoveEnemyFromList(Transform bodyToRemove){
        if(allBotsAndPlayers.Contains(bodyToRemove)){
            allBotsAndPlayers.Remove(bodyToRemove);
            
        }
    }
    private void RespawnBots(GameMode gameMode,int botsAmount,int lastBotViewId){
        if(gameMode == GameMode.FFA){
            for (int i = 0; i < botsAmount; i++) {
                GameObject botPlayer = PhotonNetwork.Instantiate(aiBotPrefab.name, botSpawnPoint[Random.Range(0,botSpawnPoint.Length)].position, Quaternion.identity);
                if(botPlayer.TryGetComponent(out AI_PlayerMovement aI_PlayerMovement)){
                    aI_PlayerMovement.TrySync(string.Concat("Bot ",Random.Range(10,1000000)),MatchHandler.Current.GetGameSettingsSO().IsAwayTeam,botDatasList[lastBotViewId]);
                }
            }
        }
    }
    public void OnBotDeath(Vector3 deathPosition, int p_actor, string username, string gunName,AI_PlayerMovement aI_PlayerMovement){
        // MatchHandler.Current?.Spawn(awayTeam ? SpawningArea.SpawnAreaType.Away : SpawningArea.SpawnAreaType.Home);
        MatchHandler.Current?.ChangeStat_S(aI_PlayerMovement.actorNumber, 1, 1,gunName);

        if (p_actor >= 0){
            MatchHandler.Current?.ChangeStat_S(p_actor, 0, 1,aI_PlayerMovement.playerProfile.gunName);
        }
        RemoveEnemyFromList(aI_PlayerMovement.transform);
        RespawnBots(MatchHandler.Current.GetGameSettingsSO().gameMode,1,aI_PlayerMovement.ViewID);
        PhotonNetwork.Destroy(aI_PlayerMovement.gameObject);
    }
    public List<Transform> GetAllBotsList(){
        return allBotsAndPlayers;
    }
}
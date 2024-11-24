using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/PlayerProfileDataSO", fileName = "PlayerProfileDataSO")]
public class PlayerProfileDataSO : ScriptableObject {
    public string username;
    public SpawningArea.SpawnAreaType spawnArea;
    public int teamNumber;
    public int level;
    public int xp;
    public string gunName;
    public PlayerInfo playerInfo;
    public void SetPlayerInfo(PlayerInfo playerInfo){
        this.playerInfo = playerInfo;
    }
}
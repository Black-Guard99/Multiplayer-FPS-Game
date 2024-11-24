using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/GameSettingsSO", fileName = "GameSettingsSO")]
public class GameSettingsSO : ScriptableObject {
    public GameMode gameMode = GameMode.FFA;
    public MapData mapData;
    public float maxPlayer;
    public bool IsAwayTeam = false;
    public bool hasBots = false;
}
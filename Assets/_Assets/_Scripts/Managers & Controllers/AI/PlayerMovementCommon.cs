using TMPro;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class PlayerMovementCommon : MonoBehaviourPunCallbacks {
    [Header("External Referances")]
    [SerializeField] private bool isAI;
    public ProfileData playerProfile;
    [SerializeField] protected TextMeshPro playerNickName;
    [SerializeField] protected bool awayTeam;
    [SerializeField] protected Renderer[] teamIndicators;
    protected PlayerData playerData;
    protected bool isReady;

    public virtual void TrySync () {
        if (photonView.IsMine){
            if(!isAI){
                photonView.RPC(nameof(SyncProfile), RpcTarget.All, Launcher.myProfile.username, Launcher.myProfile.level, Launcher.myProfile.xp);
                if (MatchHandler.Current.GetGameSettingsSO().gameMode == GameMode.TDM) {
                    photonView.RPC(nameof(SyncTeam), RpcTarget.All, MatchHandler.Current.GetGameSettingsSO().IsAwayTeam);
                }else{
                    playerData.SetPlayerType(PlayerData.PlayerType.Frindely);
                    ColorTeamIndicators(Random.ColorHSV());
                }
            }
        }else{
            playerData.SetPlayerType(PlayerData.PlayerType.Enemy);
        }
    }
    private void ColorTeamIndicators (Color p_color) {
        foreach (Renderer renderer in teamIndicators) {
            renderer.material.color = p_color;
        }
    }
    [PunRPC]
    private void SyncProfile(string p_username, int p_level, int p_xp) {
        playerProfile = new ProfileData(p_username, p_level, p_xp);
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
    public bool GetIsRead(){
        return isReady;
    }
}
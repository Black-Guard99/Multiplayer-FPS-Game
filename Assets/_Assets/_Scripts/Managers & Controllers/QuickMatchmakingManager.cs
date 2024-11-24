using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Photon.Realtime;

public class QuickMatchmakingManager : MonoBehaviour,IMatchmakingCallbacks {
    [SerializeField]
    private int maxPlayers = 4;
    private LoadBalancingClient loadBalancingClient;
    

    public void CreateRoom() {
        loadBalancingClient = new LoadBalancingClient(ExitGames.Client.Photon.ConnectionProtocol.Udp);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayers;
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        loadBalancingClient.OpCreateRoom(enterRoomParams);
    }

    public void QuickMatch() {
        loadBalancingClient.OpJoinRandomRoom();
    }

    // do not forget to register callbacks via loadBalancingClient.AddCallbackTarget
    // also deregister via loadBalancingClient.RemoveCallbackTarget
    #region IMatchmakingCallbacks

    void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) {
        CreateRoom();
    }

    void IMatchmakingCallbacks.OnJoinedRoom()
    {
        // joined a room successfully
        Debug.Log("Joined a Room");
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList) {
        
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        
    }

    public void OnLeftRoom() {
        
    }

    // [..] Other callbacks implementations are stripped out for brevity, they are empty in this case as not used.

    #endregion
    
}
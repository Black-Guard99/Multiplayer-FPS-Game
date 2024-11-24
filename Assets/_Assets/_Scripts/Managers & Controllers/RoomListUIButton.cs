using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class RoomListUIButton : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI roomNameText,mapNameText,playerCount;
    private Launcher launcher;
    public void SetRoomName(string roomName){
        roomNameText.SetText(roomName);
    }
    public void SetPlayerCount(string amount){
        playerCount.SetText(amount);
    }
    public void SetMapName(string mapName){
        mapNameText.SetText(mapName);
    }
    public void EnterRoom(){
        launcher.JoinRoom(this.gameObject.transform);
    }
    public void SetLaucher(Launcher launcher){
        this.launcher = launcher;
    }
    public string GetRoomName(){
        return roomNameText.text;
    }
    
}
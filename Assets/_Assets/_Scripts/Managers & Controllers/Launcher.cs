using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
// using ExitGames.Client.Photon;
using System.Collections.Generic;
using Random = UnityEngine.Random;
[System.Serializable]
public class ProfileData {
    public string username;
    public SpawningArea.SpawnAreaType spawnArea;
    public int teamNumber;
    public int level;
    public int xp;
    public string gunName;

    public ProfileData() {
        this.username = "";
        this.level = 1;
        this.xp = 0;
    }

    public ProfileData(string u, int l, int x) {
        this.username = u;
        this.level = l;
        this.xp = x;
    }
}

[System.Serializable]
public class MapData {
    public string name;
    public int scene;
}
public class Launcher : MonoBehaviourPunCallbacks,IMatchmakingCallbacks {
    private const string MAP = "MAP";
    private const string MODE = "MODE";
    [SerializeField] private GameSettingsSO gameSettingsSo;
    [SerializeField] private Button createRoomBtn,roomListBtn,createBtn;

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField roomnameField;
    [SerializeField] private TextMeshProUGUI mapValue;
    [SerializeField] private TextMeshProUGUI modeValue;
    [SerializeField] private Slider maxPlayersSlider; 
    [SerializeField] private TextMeshProUGUI maxPlayersValue; 
    [SerializeField] private GameObject tabMain;
    [SerializeField] private GameObject tabRooms;
    [SerializeField] private Transform roomContants;
    [SerializeField] private GameObject tabCreate; 
    [SerializeField] private RoomListUIButton buttonRoom;
    [SerializeField] private GameObject connectingWindow;
    [SerializeField] private BaseManagerUI baseManagerUi;

    private List<RoomInfo> roomList;
    public static ProfileData myProfile = new ProfileData();
    public void Awake() {
        createRoomBtn.interactable = false;
        roomListBtn.interactable = false;
        createBtn.interactable = false;
        connectingWindow.SetActive(false);
        myProfile = (ProfileData)Data.LoadData("Profile");
        maxPlayersValue.SetText (string.Concat(gameSettingsSo.maxPlayer.ToString()));
        // TryConnect();
    }
    // Call from Battle Btn.
    public void TryConnect(){
        if(!PhotonNetwork.IsConnected){
            // connectingWindow.SetActive(true);
            Debug.Log("Starting To Connect");
            Connect();
        }else{
            PhotonNetwork.Disconnect();
            Connect();
        }
    }
    private void Update(){
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
    }
    public override void OnDisconnected(DisconnectCause cause){
        Debug.Log("Disconnected Due to " + cause);
        createRoomBtn.interactable = false;
        roomListBtn.interactable = false;
        createBtn.interactable = false;
        base.OnDisconnected(cause);
    }
    


    public override void OnConnectedToMaster() {
        Debug.Log("CONNECTED!");
        baseManagerUi.OpenMatchSettings();
        PhotonNetwork.JoinLobby();
        connectingWindow.SetActive(false);
        createRoomBtn.interactable = true;
        roomListBtn.interactable = true;
        createBtn.interactable = true;
        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom() {
        connectingWindow.SetActive(true);
        StartGame();
        base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        connectingWindow.SetActive(true);
        Create();
        Debug.Log("Random Room Joined Failed");
        base.OnJoinRandomFailed(returnCode,message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message){
        connectingWindow.SetActive(true);
        Create();
        base.OnJoinRoomFailed(returnCode,message);
    }

    public void Connect () {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Trying to Connect...");
        PhotonNetwork.ConnectUsingSettings();
    }
    // Calling From Battle Btn UI............
    public void StartBattle(){
        if(!PhotonNetwork.IsConnectedAndReady){
            Debug.LogError("Not Connected or Ready to Start Game");
            return;
        }
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        GameMode currentMode = gameSettingsSo.gameMode;
        string currentMap = gameSettingsSo.mapData.name;
        switch(currentMode){
            case GameMode.FFA:
                gameSettingsSo.maxPlayer = 10;
            break;
            case GameMode.TDM:
                gameSettingsSo.maxPlayer = 4;
            break;
        }
        float maxPlayer = gameSettingsSo.maxPlayer;
        properties.Add(MODE, currentMode);
        properties.Add(MAP, currentMap);
        Debug.Log($"Finding A Room Or Creating With Game Mode of {(GameMode)currentMode} With Map {currentMap}");
        // PhotonNetwork.JoinRandomRoom(properties,(byte)maxPlayer,MatchmakingMode.FillRoom,TypedLobby.Default,null,null);
        PhotonNetwork.JoinRandomRoom(properties,(byte)maxPlayer);
    }    

    public void Create(){
        Debug.LogError("Creating a New Room");
        RoomOptions options = new RoomOptions();
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        string currentMap = gameSettingsSo.mapData.name;
        GameMode currentMode = gameSettingsSo.gameMode;
        properties.Add(MODE, currentMode);
        properties.Add(MAP, currentMap);
        switch(currentMode){
            case GameMode.FFA:
                gameSettingsSo.maxPlayer = 10;
            break;
            case GameMode.TDM:
                gameSettingsSo.maxPlayer = 4;
            break;
        }
        float maxPlayer = gameSettingsSo.maxPlayer;
        options.MaxPlayers = (byte) maxPlayer;
        options.CustomRoomPropertiesForLobby = new string[] {MAP,MODE};

        options.CustomRoomProperties = properties;
        Debug.Log($"Creating A Room With Game Mode of {(GameMode)currentMode} With Map {currentMap} With Max Player of {maxPlayer}");
        PhotonNetwork.CreateRoom(null, options);
    }

    public void ChangeMap () {
        // currentmap++;
        // if (currentmap >= maps.Length) currentmap = 0;
        // mapValue.SetText(string.Concat( "MAP: ",gameSettingsSo.mapData.name.ToUpper()));
    }


    public void ChangeMaxPlayersSlider (float t_value)  {
        maxPlayersValue.SetText (string.Concat(t_value.ToString()));
        gameSettingsSo.maxPlayer = Mathf.RoundToInt(t_value);
    }

    public void TabCloseAll() {
        tabMain.SetActive(false);
        // tabRooms.SetActive(false);
        tabCreate.SetActive(false); 
    }

    public void TabOpenMain () {
        TabCloseAll();
        tabMain.SetActive(true);
    }

    public void TabOpenRooms () {
        TabCloseAll();
        // tabRooms.SetActive(true);
    }

    public void TabOpenCreate () {
        TabCloseAll();
        switch(gameSettingsSo.gameMode){
            case GameMode.FFA:
                maxPlayersSlider.minValue = 2;
            break;
            case GameMode.TDM:
                maxPlayersSlider.minValue = 4;
                maxPlayersSlider.maxValue = 10;
            break;
        }
        tabCreate.SetActive(true);

        roomnameField.text = "";

        // currentmap = 0;
        // mapValue.SetText(string.Concat("MAP: ",gameSettingsSo.mapData..name.ToUpper()));

        gameSettingsSo.gameMode = (GameMode)0;
        modeValue.SetText(string.Concat("MODE: ",System.Enum.GetName(typeof(GameMode), (GameMode)0)));

        maxPlayersSlider.value = maxPlayersSlider.maxValue;
        maxPlayersValue.SetText(Mathf.RoundToInt(maxPlayersSlider.value).ToString());
    }

    private void ClearRoomList () {
        Transform content = roomContants;
        foreach (Transform a in roomContants) Destroy(a.gameObject);
    }

    private void VerifyUsername () {
        if (string.IsNullOrEmpty(usernameField.text)) {
            myProfile.username = "USER_" + Random.Range(100, 1000);
        } else {
            myProfile.username = usernameField.text;
        }
        PhotonNetwork.NickName = myProfile.username;
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> p_list) {
        
        roomList = p_list;
        ClearRoomList();

        Transform content = roomContants;

        foreach (RoomInfo a in p_list) {
            RoomListUIButton roomListUIButton = Instantiate(buttonRoom, content);
            roomListUIButton.SetLaucher(this);
            roomListUIButton.SetRoomName(a.Name);
            roomListUIButton.SetPlayerCount(string.Concat(a.PlayerCount , " / " , a.MaxPlayers));
            if (a.CustomProperties.ContainsKey("map")){
                // roomListUIButton.SetMapName(gameSettingsSo.mapData[(int)a.CustomProperties["map"]].name);
            }else{
                roomListUIButton.SetMapName("_____");
            }
        }
        Debug.Log("Room List Count " + p_list.Count);
        base.OnRoomListUpdate(p_list);
    }

    public void JoinRoom (Transform p_button){
        connectingWindow.SetActive(true);
        if(p_button.TryGetComponent(out RoomListUIButton roomListUIButton)){
            string t_roomName = roomListUIButton.GetRoomName();
            VerifyUsername();

            RoomInfo roomInfo = null;
            Transform buttonParent = p_button.parent;
            for (int i = 0; i < buttonParent.childCount; i++) {
                if (buttonParent.GetChild(i).Equals(p_button)) {
                    roomInfo = roomList[i];
                    break;
                }
            }

            if (roomInfo != null) {
                LoadGameSettings(roomInfo);
                PhotonNetwork.JoinRoom(t_roomName);
            }
        }
    }

    public void LoadGameSettings (RoomInfo roomInfo) {
        gameSettingsSo.gameMode = (GameMode)roomInfo.CustomProperties[MODE];
    }

    public void StartGame () {
        VerifyUsername();
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            Data.SaveData((ProfileData) myProfile,"Profile");
            PhotonNetwork.LoadLevel(gameSettingsSo.mapData.scene);
        }
    }
}
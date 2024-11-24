using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using Baracuda.Monitoring;
using Unity.Mathematics;
using System;
[System.Serializable]
public class PlayerInfo {
    public ProfileData profile;
    public int actor;
    public short kills;
    public short deaths;
    public bool awayTeam;

    public PlayerInfo (ProfileData p, int a, short k, short d, bool t) {
        this.profile = p;
        this.actor = a;
        this.kills = k;
        this.deaths = d;
        this.awayTeam = t;
    }
}

public enum GameState{
    Waiting = 0,
    Starting = 1,
    Playing = 2,
    Ending = 3
}
[System.Serializable]
public class SpawningArea{
    public enum SpawnAreaType{
        Home,Away
    }
    public SpawnAreaType spawnAreaType;

    public Transform[] spawnPointArrey;
}
[System.Serializable]
public class LeanderBoardWindow{
    public GameObject mainWindow;
    public LeaderBoardMode ffaWindow,tdmWindow;
    public Transform TDM_PlayerCardsContainer,FAA_PlayerCardsContainer;
}

public class MatchHandler : MonoBehaviourPunCallbacks, IOnEventCallback {
    #region Fields
    public static MatchHandler Current{get;private set;}
    [SerializeField] private ObjectPoolingManager poolingManager;
    [SerializeField] private int mainmenu = 0;
    [SerializeField] private int killcount = 45;
    [SerializeField] private int matchLength = 180;
    [SerializeField] private bool perpetual = false;
    [SerializeField] private int myind;
    [SerializeField] private GameObject mapcam;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private SpawningArea spawnArea_A,spawnArea_B;
    [SerializeField] private List<PlayerInfo> playerInfo = new List<PlayerInfo>();


    [SerializeField] private LeanderBoardWindow ui_leaderboard,ui_LeaderboardEndWindow;
    [SerializeField] private LeaderboardPlayerCard leaderboardPlayerCard;
    [SerializeField] private RectTransform killFeedUiHolder;
    [SerializeField] private KillFeedUI killFeedUIPrefab;
    [SerializeField] private BotSpawnManager botSpawnManager;
    [SerializeField] private GameSettingsSO gameSettingsSo;
    [SerializeField] private KeyCode leaderBoardKeyCode = KeyCode.Tab;
    [Monitor,MTextColor(ColorPreset.LightBlue)] private int playerCount;
    [Monitor,MTextColor(ColorPreset.Green)] private int ping;
    [Monitor,MPosition(UIPosition.UpperRight)] private string connectionStatus;
    private GameState state = GameState.Waiting;
    private float currentBotSpawnTime  = 5f;
    private bool isLeaderBoardOpen;
    private float startingTime = 10f;
    private bool playerAdded;
    private int currentMatchTime;
    private Coroutine timerCoroutine;
    private string killer = "";
    private string killed = "";
    private string weponName = "";

    #endregion

    #region Codes

    public enum EventCodes : byte {
        NewPlayer,
        UpdatePlayers,
        ChangeStat,
        NewMatch,
        RefreshTimer
    }

    #endregion

    #region MB Callbacks
    private void Awake(){
        Application.targetFrameRate = 60;
        Monitor.StartMonitoring(this);
        // Or use this extension method:
        this.StartMonitoring();
        if(Current == null){
            Current = this;
        } else{
            Destroy(Current.gameObject);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void OnDestroy() {
        Monitor.StopMonitoring(this);
        // Or use this extension method:
        this.StopMonitoring();
    }
    private void OnDrawGizmos(){
        Gizmos.color = Color.cyan;
        foreach(Transform spawnPoint in spawnArea_A.spawnPointArrey){
            Gizmos.DrawSphere(spawnPoint.position,.1f);
        }
        foreach(Transform spawnPoint in spawnArea_B.spawnPointArrey){
            Gizmos.DrawSphere(spawnPoint.position,.1f);
        }
    }

    private void Start() {
        ObjectPoolingManager.Current.Init();

        ValidateConnection();
        
        NewPlayer_S(Launcher.myProfile);
        state = GameState.Waiting;

        mapcam.SetActive(false);
        if (PhotonNetwork.IsMasterClient){
            playerAdded = true;
            Spawn(SpawningArea.SpawnAreaType.Home);
        }
    }
    private void SetKillFeed(string killedBy,string killedWith,string killedTo){
        KillFeedUI newKillFeed = Instantiate(killFeedUIPrefab,killFeedUiHolder);
        newKillFeed.SetKills(killedBy,killedWith,killedTo);
        Destroy(newKillFeed.gameObject,5f);
    }
    private void Update() {

        ping = PhotonNetwork.GetPing();
        connectionStatus = PhotonNetwork.NetworkClientState.ToString();
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        switch(state){
            case GameState.Waiting:
                UIControllers.Current.ShowHideEndGameWindow(false);
                UIControllers.Current.ShowHideWaitingWindow(false);
                if (Input.GetKeyDown(leaderBoardKeyCode)) {
                    isLeaderBoardOpen = !isLeaderBoardOpen;

                    LeaderboardGame();
                }
                if(!IsRoomFull()){
                    if(gameSettingsSo.hasBots){
                        if(currentBotSpawnTime <= 0f){
                            botSpawnManager.SpawnBot(gameSettingsSo.gameMode,/* (PhotonNetwork.CurrentRoom.MaxPlayers - GetTotalPlayercount()) */4,PhotonNetwork.LocalPlayer.ActorNumber);
                            currentBotSpawnTime = 0f;
                            gameSettingsSo.hasBots = false;
                        }else{
                            currentBotSpawnTime -= Time.deltaTime;
                        }
                    }
                    return;
                }
                PhotonNetwork.CurrentRoom.IsVisible = false;
                state = GameState.Starting;
            break;
            case GameState.Starting:
                if(GetTotalPlayercount() != PhotonNetwork.CurrentRoom.MaxPlayers){
                    return;
                }
                foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player")) {
                    if(playerObject.TryGetComponent(out NewPlayerMovement newPlayerMovement)){
                        if(!newPlayerMovement.GetIsRead()){
                            return;
                        }
                    }
                }
                if (Input.GetKeyDown(leaderBoardKeyCode)) {
                    isLeaderBoardOpen = !isLeaderBoardOpen;
                    LeaderboardGame();
                }
                UIControllers.Current.ShowHideWaitingWindow(true);
                startingTime -= Time.deltaTime;
                UIControllers.Current.ShowWaitingTime(Mathf.CeilToInt(startingTime));
                if(startingTime <= 0f){
                    UIControllers.Current.ShowHideWaitingWindow(false);
                    state = GameState.Playing;
                    InitializeUI();
                    InitializeTimer();
                }
            break;
            case GameState.Playing:
                if (Input.GetKeyDown(leaderBoardKeyCode)) {
                    isLeaderBoardOpen = !isLeaderBoardOpen;
                    LeaderboardGame();
                }
            break;
            case GameState.Ending:
                UIControllers.Current.ShowHideEndGameWindow(true);
                LeaderboardEndGame();
            break;
        }
        
    }
    private bool IsRoomFull(){
        return GetTotalPlayercount() == PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    public int GetTotalPlayercount(){
        return GameObject.FindGameObjectsWithTag("Player").Length;
    }
    
    public GameState GetGameState(){
        return state;
    }

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion

    #region Photon

    public void OnEvent (EventData photonEvent) {
        if (photonEvent.Code >= 200) return;

        EventCodes eventCodes = (EventCodes) photonEvent.Code;
        object[] customDataObjects = (object[]) photonEvent.CustomData;

        switch (eventCodes){
            case EventCodes.NewPlayer:
                NewPlayer_R(customDataObjects);
                break;

            case EventCodes.UpdatePlayers:
                UpdatePlayers_R(customDataObjects);
                break;

            case EventCodes.ChangeStat:
                ChangeStat_R(customDataObjects);
                break;

            case EventCodes.NewMatch:
                NewMatch_R();
                break;

            case EventCodes.RefreshTimer:
                RefreshTimer_R(customDataObjects);
                break;
        }
    }

    #endregion

    #region Methods
    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Connection Disconnet");
        base.OnDisconnected(cause);
    }
    public override void OnLeftRoom (){
        base.OnLeftRoom();
        SceneManager.LoadScene(mainmenu);
    }

    public void Spawn (SpawningArea.SpawnAreaType spawnAreaType) {
        SpawningArea area = new SpawningArea();
        switch(spawnAreaType){
            case SpawningArea.SpawnAreaType.Home:
                area = spawnArea_A;
            break;
            case SpawningArea.SpawnAreaType.Away:
                area = spawnArea_B;
            break;
        }
        
        Transform t_spawn = area.spawnPointArrey[Random.Range(0, spawnArea_A.spawnPointArrey.Length)];
        if (PhotonNetwork.IsConnected) {
            Debug.Log("Spawning Over Network");
            PhotonNetwork.Instantiate(playerPrefab.name, t_spawn.position, Quaternion.identity);
            /* if(newPlayer.TryGetComponent(out PlayerMovementCommon newplayerMove)){
                userLeftProfileData = newplayerMove.playerProfile;
            } */
        } else {
            Debug.Log("Spawning Not from Network");
            Instantiate(playerPrefab, t_spawn.position, Quaternion.identity);
        }
    }
    
    
    public override void OnPlayerLeftRoom(Player player){
        // Debug.Log("Current Player UserName : + " + userLeftProfileData);
        Debug.LogError("Player Left this Room : " + player.NickName);
        base.OnPlayerLeftRoom(player);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.LogError("New Player Left this Room : " + newPlayer.NickName);
        base.OnPlayerEnteredRoom(newPlayer);
    }


    private void InitializeUI () {
        RefreshMyStats();
    }

    private void RefreshMyStats () {
        if (playerInfo.Count > myind) {
            UIControllers.Current.SetKills(playerInfo[myind].kills);
            UIControllers.Current.SetDeath(playerInfo[myind].deaths);
            SetKillFeed(killer,weponName,killed);
        } else {
            UIControllers.Current.SetKills(0);
            UIControllers.Current.SetDeath(0);
        }
    }

    private void LeaderboardEndGame () {
        ui_LeaderboardEndWindow.mainWindow.gameObject.SetActive(true);
        ui_leaderboard.mainWindow.gameObject.SetActive(false);
        switch(gameSettingsSo.gameMode){
            case GameMode.FFA:
                ui_leaderboard.tdmWindow.gameObject.SetActive(true);
                ui_leaderboard.ffaWindow.gameObject.SetActive(false);
                // clean up
                for (int i = 0; i < ui_LeaderboardEndWindow.FAA_PlayerCardsContainer.childCount; i++) {
                    Destroy(ui_LeaderboardEndWindow.FAA_PlayerCardsContainer.GetChild(i).gameObject);
                }

                // set details
                ui_LeaderboardEndWindow.ffaWindow.InitLeaderBoard(Enum.GetName(typeof(GameMode), gameSettingsSo.gameMode),SceneManager.GetActiveScene().name);
                ui_LeaderboardEndWindow.ffaWindow.SetHomeandAwayScore("0","0");

                // sort
                List<PlayerInfo> faaSorted = SortPlayers(playerInfo);

                // display
                bool faa_alternateColors = false;
                for (int i = 0; i < faaSorted.Count; i++) {
                    LeaderboardPlayerCard newcard = Instantiate(leaderboardPlayerCard, ui_LeaderboardEndWindow.FAA_PlayerCardsContainer);
                    newcard.SetIsAwayTeam(false);
                    newcard.SetIsHomeTeam(false);
                    if (faa_alternateColors) newcard.SetCardsColor(new Color32(0, 0, 0, 180));
                    faa_alternateColors = !faa_alternateColors;
                    newcard.SetCardsData(faaSorted[i].profile.level.ToString("00"),faaSorted[i].profile.username,(faaSorted[i].kills * 100).ToString(),faaSorted[i].kills.ToString(),faaSorted[i].deaths.ToString());
                }

            break;
            case GameMode.TDM:
                ui_LeaderboardEndWindow.mainWindow.gameObject.SetActive(true);
                ui_leaderboard.ffaWindow.gameObject.SetActive(true);
                ui_leaderboard.tdmWindow.gameObject.SetActive(false);
                // clean up
                for (int i = 0; i < ui_LeaderboardEndWindow.TDM_PlayerCardsContainer.childCount; i++) {
                    Destroy(ui_LeaderboardEndWindow.TDM_PlayerCardsContainer.GetChild(i).gameObject);
                }

                // set details
                ui_LeaderboardEndWindow.tdmWindow.InitLeaderBoard(Enum.GetName(typeof(GameMode), gameSettingsSo.gameMode),SceneManager.GetActiveScene().name);
                ui_LeaderboardEndWindow.tdmWindow.SetHomeandAwayScore("0","0");
                List<PlayerInfo> TdmSorted = SortPlayers(playerInfo);

                // display
                bool Tdm_alternateColors = false;
                for (int i = 0; i < TdmSorted.Count; i++) {
                    LeaderboardPlayerCard newcard = Instantiate(leaderboardPlayerCard, ui_LeaderboardEndWindow.TDM_PlayerCardsContainer);
                    newcard.SetIsHomeTeam(!TdmSorted[i].awayTeam);
                    newcard.SetIsAwayTeam(TdmSorted[i].awayTeam);
                    if (Tdm_alternateColors) newcard.SetCardsColor(new Color32(0, 0, 0, 180));
                    Tdm_alternateColors = !Tdm_alternateColors;
                    newcard.SetCardsData(TdmSorted[i].profile.level.ToString("00"),TdmSorted[i].profile.username,(TdmSorted[i].kills * 100).ToString()
                        ,TdmSorted[i].kills.ToString(),TdmSorted[i].deaths.ToString());
                }
            break;
        }
    }
    private void LeaderboardGame () {
        ui_leaderboard.mainWindow.gameObject.SetActive(isLeaderBoardOpen);
        switch(gameSettingsSo.gameMode){
            case GameMode.FFA:
                ui_leaderboard.ffaWindow.gameObject.SetActive(isLeaderBoardOpen);
                ui_leaderboard.tdmWindow.gameObject.SetActive(false);
                // clean up
                for (int i = 0; i < ui_leaderboard.FAA_PlayerCardsContainer.childCount; i++) {
                    Destroy(ui_leaderboard.FAA_PlayerCardsContainer.GetChild(i).gameObject);
                }

                // set details
                ui_leaderboard.ffaWindow.InitLeaderBoard(Enum.GetName(typeof(GameMode), gameSettingsSo.gameMode),SceneManager.GetActiveScene().name);
                ui_leaderboard.ffaWindow.SetHomeandAwayScore("0","0");
                List<PlayerInfo> faaSorted = SortPlayers(playerInfo);

                // display
                Debug.Log("Sorted " + faaSorted.Count);
                bool faa_alternateColors = false;
                for (int i = 0; i < faaSorted.Count; i++) {
                    LeaderboardPlayerCard newcard = Instantiate(leaderboardPlayerCard);
                    newcard.transform.SetParent(ui_leaderboard.FAA_PlayerCardsContainer);
                    newcard.SetIsAwayTeam(false);
                    newcard.SetIsHomeTeam(false);

                    if (faa_alternateColors) newcard.SetCardsColor( new Color32(0, 0, 0, 180));
                    faa_alternateColors = !faa_alternateColors;
                    newcard.SetCardsData(faaSorted[i].profile.level.ToString("00"),faaSorted[i].profile.username,(faaSorted[i].kills * 100).ToString(),
                        faaSorted[i].kills.ToString(),faaSorted[i].deaths.ToString());
                }
            break;
            case GameMode.TDM:
                // ui_leaderboard.mainWindow.gameObject.SetActive(isLeaderBoardOpen);
                // activate
                ui_leaderboard.tdmWindow.gameObject.SetActive(isLeaderBoardOpen);
                ui_leaderboard.ffaWindow.gameObject.SetActive(false);
                // clean up
                for (int i = 0; i < ui_leaderboard.TDM_PlayerCardsContainer.childCount; i++) {
                    Destroy(ui_leaderboard.TDM_PlayerCardsContainer.GetChild(i).gameObject);
                }

                // set details
                ui_leaderboard.tdmWindow.InitLeaderBoard(Enum.GetName(typeof(GameMode), gameSettingsSo.gameMode),SceneManager.GetActiveScene().name);
                ui_leaderboard.tdmWindow.SetHomeandAwayScore("0","0");
                List<PlayerInfo> TdmSorted = SortPlayers(playerInfo);
                Debug.Log("Sorted Count = " + TdmSorted);
                bool tdm_alternateColors = false;
                for (int i = 0; i < TdmSorted.Count; i++) {
                    LeaderboardPlayerCard newcard = Instantiate(leaderboardPlayerCard, ui_leaderboard.TDM_PlayerCardsContainer);
                    newcard.SetIsHomeTeam(!TdmSorted[i].awayTeam);
                    newcard.SetIsAwayTeam(TdmSorted[i].awayTeam);

                    if (tdm_alternateColors) newcard.SetCardsColor(new Color32(0, 0, 0, 180));
                    tdm_alternateColors = !tdm_alternateColors;
                    newcard.SetCardsData(TdmSorted[i].profile.level.ToString("00"),TdmSorted[i].profile.username,(TdmSorted[i].kills * 100).ToString(),
                        TdmSorted[i].kills.ToString(),TdmSorted[i].deaths.ToString());
                }

            break;
        }
        
    }

    private List<PlayerInfo> SortPlayers (List<PlayerInfo> p_info) {
        List<PlayerInfo> sorted = new List<PlayerInfo>();
        if (gameSettingsSo.gameMode == GameMode.FFA) {
            while (sorted.Count < p_info.Count) {
                // set defaults
                short highest = -1;
                PlayerInfo selection = p_info[0];

                // grab next highest player
                foreach (PlayerInfo a in p_info) {
                    if (sorted.Contains(a)) continue;
                    if (a.kills > highest) {
                        selection = a;
                        highest = a.kills;
                    }
                }

                // add player
                sorted.Add(selection);
            }
        }

        if (gameSettingsSo.gameMode == GameMode.TDM) {
            List<PlayerInfo> homeSorted = new List<PlayerInfo>();
            List<PlayerInfo> awaySorted = new List<PlayerInfo>();

            int homeSize = 0;
            int awaySize = 0;

            foreach (PlayerInfo p in p_info) {
                if (p.awayTeam) awaySize++;
                else homeSize++;
            }

            while (homeSorted.Count < homeSize) {
                // set defaults
                short highest = -1;
                PlayerInfo selection = p_info[0];

                // grab next highest player
                foreach (PlayerInfo a in p_info) {
                    if (a.awayTeam) continue;
                    if (homeSorted.Contains(a)) continue;
                    if (a.kills > highest) {
                        selection = a;
                        highest = a.kills;
                    }
                }

                // add player
                homeSorted.Add(selection);
            }

            while (awaySorted.Count < awaySize) {
                // set defaults
                short highest = -1;
                PlayerInfo selection = p_info[0];

                // grab next highest player
                foreach (PlayerInfo a in p_info) {
                    if (!a.awayTeam) continue;
                    if (awaySorted.Contains(a)) continue;
                    if (a.kills > highest) {
                        selection = a;
                        highest = a.kills;
                    }
                }

                // add player
                awaySorted.Add(selection);
            }

            sorted.AddRange(homeSorted);
            sorted.AddRange(awaySorted);
        }

        return sorted;
    }

    private void ValidateConnection () {
        if (PhotonNetwork.IsConnected) return;
        SceneManager.LoadSceneAsync(mainmenu);
    }
    public void LeaveRoom(){
        state = GameState.Ending;
        PhotonNetwork.LeaveRoom(false);
    }
    private void StateCheck () {
        if (state == GameState.Ending) {
            EndGame();
        }
    }

    private void ScoreCheck () {
        // define temporary variables
        bool detectwin = false;

        // check to see if any player has met the win conditions
        foreach (PlayerInfo a in playerInfo) {
            // free for all
            if(a.kills >= killcount) {
                detectwin = true;
                break;
            }
        }

        // did we find a winner?
        if (detectwin) {
            // are we the master client? is the game still going?
            if (PhotonNetwork.IsMasterClient && state != GameState.Ending) {
                // if so, tell the other players that a winner has been detected
                UpdatePlayers_S((int)GameState.Ending, playerInfo);
            }
        }
    }

    private void InitializeTimer () {
        currentMatchTime = matchLength;
        RefreshTimerUI();

        if (PhotonNetwork.IsMasterClient) {
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    private void RefreshTimerUI() {
        float minutes = (currentMatchTime / 60);
        float seconds = (currentMatchTime % 60);
        UIControllers.Current.DisplayTime(minutes,seconds);
    }

    private void EndGame() {
        // set game state to ending
        state = GameState.Ending;

        // set timer to 0
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        currentMatchTime = 0;
        RefreshTimerUI();

        // disable room
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.DestroyAll();

            if (!perpetual) {
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

        // activate map camera
        mapcam.SetActive(true);

        // show end game ui
        UIControllers.Current.ShowHideEndGameWindow(true);
        LeaderboardEndGame();

        // wait X seconds and then return to main menu
        StartCoroutine(End(6f));
    }

    private bool CalculateTeam () {
        return PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0;
    }

    #endregion

    #region Events

    public void NewPlayer_S (ProfileData p) {
        object[] package = new object[7];

        package[0] = p.username;
        package[1] = p.level;
        package[2] = p.xp;
        package[3] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[4] = (short) 0;
        package[5] = (short) 0;
        package[6] = CalculateTeam();

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }
    public void NewPlayer_R (object[] data) {
        PlayerInfo p = new PlayerInfo(
            new ProfileData(
                (string) data[0],
                (int) data[1],
                (int) data[2]
            ),
            (int) data[3],
            (short) data[4],
            (short) data[5],
            (bool) data[6]
        );

        playerInfo.Add(p);

        //resync our local player information with the new player
        foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player"))  {
            if(playerObject.TryGetComponent(out NewPlayerMovement playerMovement)){
                playerMovement.TrySync();
                // userLeftProfileData = playerMovement.playerProfile;
            }
        }

        UpdatePlayers_S((int)state, playerInfo);
    }

    public void UpdatePlayers_S (int state, List<PlayerInfo> info) {
        object[] package = new object[info.Count + 1];

        package[0] = state;
        for (int i = 0; i < info.Count; i++) {
            object[] piece = new object[7];

            piece[0] = info[i].profile.username;
            piece[1] = info[i].profile.level;
            piece[2] = info[i].profile.xp;
            piece[3] = info[i].actor;
            piece[4] = info[i].kills;
            piece[5] = info[i].deaths;
            piece[6] = info[i].awayTeam;

            package[i + 1] = piece;
        }

        PhotonNetwork.RaiseEvent (
            (byte)EventCodes.UpdatePlayers, 
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All }, 
            new SendOptions { Reliability = true }
        );
    }
    public void UpdatePlayers_R (object[] data) {
        state = (GameState)data[0];

        //check if there is a new player
        if (playerInfo.Count < data.Length - 1) {
            foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player")) {
                //if so, resync our local player information
                if(playerObject.TryGetComponent(out NewPlayerMovement playerMovement)){
                    playerMovement.TrySync();
                }
            }
        }

        playerInfo = new List<PlayerInfo>();

        for (int i = 1; i < data.Length; i++) {
            object[] extract = (object[]) data[i];

            PlayerInfo p = new PlayerInfo (
                new ProfileData (
                    (string) extract[0],
                    (int) extract[1],
                    (int) extract[2]
                ),
                (int) extract[3],
                (short) extract[4],
                (short) extract[5],
                (bool) extract[6]
            );

            playerInfo.Add(p);

            if (PhotonNetwork.LocalPlayer.ActorNumber == p.actor) {
                myind = i - 1;

                //if we have been waiting to be added to the game then spawn us in
                if (!playerAdded) {
                    playerAdded = true;
                    gameSettingsSo.IsAwayTeam = p.awayTeam;
                    if(gameSettingsSo.IsAwayTeam){
                        Spawn(SpawningArea.SpawnAreaType.Away);
                    }else{
                        Spawn(SpawningArea.SpawnAreaType.Home);
                    }
                }
            }
        }

        StateCheck();
    }

    public void ChangeStat_S (int actor, byte stat, byte amt,string weponName){
        object[] package = new object[] { actor, stat, amt,weponName};

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ChangeStat,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    
    public void ChangeStat_R (object[] data){
        int actor = (int) data[0];
        byte stat = (byte) data[1];
        byte amt = (byte) data[2];
        string weponName = (string) data[3];
        for (int i = 0; i < playerInfo.Count; i++){
            if(playerInfo[i].actor == actor) {
                this.weponName = weponName;
                switch(stat){
                    case 0: //kills
                        playerInfo[i].kills += amt;
                        killer = playerInfo[i].profile.username;
                        Debug.Log($"Player {playerInfo[i].profile.username} : Kills = {playerInfo[i].kills}");
                        break;

                    case 1: //deaths
                        playerInfo[i].deaths += amt;
                        killed = playerInfo[i].profile.username;
                        Debug.Log($"Player {playerInfo[i].profile.username} : Deaths = {playerInfo[i].deaths}");
                        break;
                }
                if(i == myind) RefreshMyStats();
                if (ui_leaderboard.mainWindow.activeSelf) LeaderboardGame();
                break;
            }
        }

        ScoreCheck();
    }

    public void NewMatch_S (){
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewMatch,
            null,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    public void NewMatch_R (){
        // set game state to waiting
        state = GameState.Waiting;

        // deactivate map camera
        mapcam.SetActive(false);

        // hide end game ui
        UIControllers.Current.ShowHideEndGameWindow(false);

        // reset scores
        foreach (PlayerInfo p in playerInfo){
            p.kills = 0;
            p.deaths = 0;
        }

        // reset ui
        RefreshMyStats();

        // reinitialize time
        InitializeTimer();

        // spawn
        Spawn(SpawningArea.SpawnAreaType.Home);
    }

    public void RefreshTimer_S() {
        object[] package = new object[] { currentMatchTime };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.RefreshTimer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    public void RefreshTimer_R(object[] data) {
        currentMatchTime = (int)data[0];
        RefreshTimerUI();
    }

    #endregion

    #region Coroutines

    private IEnumerator Timer () {
        yield return new WaitForSeconds(1f);

        currentMatchTime -= 1;

        if (currentMatchTime <= 0) {
            timerCoroutine = null;
            UpdatePlayers_S((int)GameState.Ending, playerInfo);
        } else {
            RefreshTimer_S();
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    private IEnumerator End (float p_wait) {
        yield return new WaitForSeconds(p_wait);

        if(perpetual) {
            // new match
            if(PhotonNetwork.IsMasterClient) {
                NewMatch_S();
            }
        } else {
            // disconnect
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        }
    }
    public GameSettingsSO GetGameSettingsSO(){
        return gameSettingsSo;
    }

    #endregion
}
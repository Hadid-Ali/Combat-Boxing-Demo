using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityAtoms;


public class ConnectionController : MonoBehaviourPunCallbacks
{
    [SerializeField] private ConnectionControllerView m_ControllerView;
    [SerializeField] private NetworkMatchStartHandler _matchStartHandler;
    [SerializeField] private int m_PlayersCount = 2;
    [SerializeField] private PhotonView m_PhotonView;

    private TypedLobby customLobby = new("gameLobby", LobbyType.SqlLobby);
    string sqlLobbyFilter = "C0 = '1'";

    private RegionHandler m_RegionHandler;

    private int m_RequiredPlayersCount = 2;
    private List<RoomInfo> m_Rooms = new();

    public bool m_IsTestConnection = true;
    public bool m_IsJoiningRoom = false;
    private bool m_comingFromButton = false;

    private string _matchStart = "Start Match Timer";
    [SerializeField] private float MatchStartTime = 3;
    
    private Photon.Realtime.Player cachedMasterClient;

    private void Start()
    {
        m_ControllerView.Initialize(StartGame, OnRegionSelect, CreateRoom, OnCheckForRoomJoining);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        //GameEvents.MenuEvents.MatchStartRequested.Register(StartMatchTimer);
        GameEvents.MenuEvents.RoomJoinRequested.Register(OnRoomJoinRequested);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        //GameEvents.MenuEvents.MatchStartRequested.UnRegister(StartMatchTimer);
        GameEvents.MenuEvents.RoomJoinRequested.UnRegister(OnRoomJoinRequested);
    }

    private void OnCheckForRoomJoining()
    {
        m_IsJoiningRoom = true;

        TypedLobby typedLobby = new TypedLobby(PhotonNetwork.CurrentLobby.Name, LobbyType.SqlLobby);
        PhotonNetwork.GetCustomRoomList(typedLobby, sqlLobbyFilter);
    }

    private void OnRoomJoinRequested(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        UpdateConnectionStatus("Joining Room...");
    }

    private void UpdateConnectionStatus(string status)
    {
        NetworkManager.Instance.SetStatus(status);
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
        m_IsTestConnection = false;
    }

    public void StartGame(MatchMode matchMode,string userName)
    {
        m_comingFromButton = true;
        PhotonNetwork.LocalPlayer.NickName = userName;
        switch (matchMode)
        {
            case MatchMode.ONLINE:
                ConnectToServer();
                break;
            
            case MatchMode.OFFLINE:
                StartOfflineMatch();
                break;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        
        if (cause == DisconnectCause.ClientTimeout || 
            cause == DisconnectCause.AuthenticationTicketExpired || 
            cause == DisconnectCause.CustomAuthenticationFailed)
        {
            Debug.Log("Will not reconnect due to specific disconnection cause.");
            return;
        }
        //PhotonNetwork.Reconnect();
        
        //GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.Raise();
        Debug.LogError($"{cause}");
    }

    private void ConnectToServer()
    {
        UpdateConnectionStatus("Connecting...");
        PhotonNetwork.ConnectUsingSettings();

        print("Connecting To  Server");
    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.OfflineMode && m_comingFromButton)
        {
            print("Creating Offline Room");
            PhotonNetwork.CreateRoom("Offline");
        }
        else if (m_IsTestConnection)
        {
            print("Test ");
            UpdateConnectionStatus("Connected to Server, Finding Best Regions to Connect");
            Invoke(nameof(OnRegionsPingCompleted), 1f);
        }
        else
        {
            PhotonNetwork.JoinLobby(customLobby);
        }

        print("Connected To Master Server");
    }

    public override void OnRegionListReceived(RegionHandler regionHandler)
    {
        m_RegionHandler = regionHandler;
    }

    private void OnRegionsPingCompleted()
    {
        if(m_RegionHandler.EnabledRegions == null)
            return;
        
        List<Region> regions = m_RegionHandler.EnabledRegions;
        //GameEvents.NetworkEvents.ConnectionTransition.Raise(new RegionConfig()
      //  {
         //   Availableregions = regions,
          //  BestRegion = m_RegionHandler.BestRegion
       // });
        
        OnRegionSelect(m_RegionHandler.BestRegion);
    }

    public void OnRegionSelect(Region region)
    {
        PhotonNetwork.Disconnect();
        m_IsTestConnection = false;

        string regionCode = region.Code;
        PhotonNetwork.ConnectToRegion(regionCode);

        // UpdateConnectionStatus(
        //     $"Connected To {NetworkManager.Instance.RegionsRegistry.GetRegionName(regionCode)}, Finding Lobby");
        
        UpdateConnectionStatus(
            $"Connected To Game Server, Finding Lobby");
    }

    public void CreateRoom(RoomOptions roomOptions)
    {
        m_RequiredPlayersCount = roomOptions.MaxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", "1" } };
        roomOptions.CustomRoomPropertiesForLobby = new[] { "C0" };
        roomOptions.CleanupCacheOnLeave = false;

        PhotonNetwork.JoinOrCreateRoom(Guid.NewGuid().ToString(), roomOptions, TypedLobby.Default);
        UpdateConnectionStatus("Setting Up Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if (!m_IsJoiningRoom)
            return;

        OnRoomsReceivedInternal(roomList);
    }

    private void OnRoomsReceivedInternal(List<RoomInfo> roomList)
    {
        Debug.Log("Rooms Received");
        if (!roomList.Any())
        {
            GameEvents.NetworkEvents.RoomJoinFailed.Raise();
            print("Room list is empty");
            return;
        }

        List<string> rooms = new();

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.IsOpen)
            {
                rooms.Add(roomInfo.Name);
            }
        }

        GameEvents.MenuEvents.RoomsListUpdated.Raise(rooms);
        GameEvents.MenuEvents.MenuTransitionEvent.Raise(MenuName.RoomSelection);
    }

    public override void OnJoinedLobby()
    {
        print("On Lobby Joined");
        GameEvents.NetworkEvents.LobbyJoined.Raise();
    }

    public void RequestRandomRoomJoin()
    {
        UpdateConnectionStatus("Joined Lobby, Finding Match");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.OfflineMode)
        {
            NetworkManager.Instance.LoadGameplay("GameplayFinal");
            return;
        }

        UpdateConnectionStatus($"Match Found,Waiting For Others");

        GameEvents.MenuEvents.MenuTransitionEvent.Raise(MenuName.InsideRoom);
        

        UpdatePlayersList();

        UpdateConnectionStatus($"Waiting for host to start the match");

        cachedMasterClient = PhotonNetwork.MasterClient;
        if (!PhotonNetwork.IsMasterClient)
            return;

        GameEvents.NetworkEvents.GameRoomCreated.Raise();
    }
    
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (cachedMasterClient != null && otherPlayer.ActorNumber == cachedMasterClient.ActorNumber)
        {
            GameEvents.NetworkPlayerEvents.OnMasterLeftRoom.Raise();
            print("Master Client Disconnected");
        }
        else
        {
            GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.Raise(otherPlayer.ActorNumber);
            print($"Player {otherPlayer.ActorNumber} left.");
        }

        cachedMasterClient = PhotonNetwork.MasterClient;
        UpdatePlayersList();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        GameEvents.NetworkEvents.PlayerJoinedRoom.Raise();
        UpdatePlayersList();
    }

    private void UpdatePlayersList()
    {
        List<string> playerNames = new();
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            playerNames.Add(players[i].NickName);
        }

        GameEvents.MenuEvents.PlayersListUpdated.Raise(playerNames);
    }
    
    // private void StartMatchTimer()
    // {
    //     NetworkManager.NetworkUtilities.RaiseRPC(m_PhotonView, nameof(StartMatchTimer_RPC), RpcTarget.All, null);
    // }
    // [PunRPC]
    // private void StartMatchTimer_RPC()
    // {
    //     GameEvents.TimerEvents.ExecuteActionRequest.Raise(new TimerDataObject()
    //     {
    //         Title = _matchStart, 
    //         TimeDuration = MatchStartTime,
    //         ActionToExecute = StartMatchInternal,
    //     });
    // }
    

    private void StartMatchInternal()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsOpen = false;

        PhotonNetwork.LoadLevel(2);
        //NetworkManager.Instance.LoadGameplay("PokerGame");
    }

    private void StartOfflineMatch()
    {
        if(m_comingFromButton)
            StartCoroutine(ValidateDisconnection());
    }

    IEnumerator ValidateDisconnection()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        PhotonNetwork.OfflineMode = true;
        m_comingFromButton = false;
        GameData.SessionData.CurrentRoomPlayersCount = 1;
        GameEvents.MenuEvents.MenuTransitionEvent.Raise(MenuName.ConnectionScreen);
        UpdateConnectionStatus($"Starting Offline match");
        
    }
}
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;

[RequireComponent(typeof(NetworkPlayerSpawner))]
public class NetworkGameplayManager : MonoBehaviour
{
    public int BotCount {private set; get;}
    [SerializeField] private NetworkPlayerSpawner m_NetworkPlayerSpawner;
    [SerializeField] protected PhotonView m_NetworkGameplayManagerView;
    [SerializeField] private NetworkMatchManager m_NetworkMatchManager;
    
    [SerializeField] private NetworkGameplayScoreHandler m_NetworkScoreHandler;
    [SerializeField] private NetworkMatchStartRules m_MatchStartRules;
    
    private List<NetworkDataObject> m_AllDecks = new();
    public PhotonView NetworkViewComponent => m_NetworkGameplayManagerView;
    
    private string _roundRestart = "RoundContinue";
    private string _roundStart = "RoundStart";

    public virtual void Awake()
    {
        m_NetworkPlayerSpawner.Initialize(OnPlayerSpawned);
        m_NetworkScoreHandler.Initialize(OnPlayerWin);
        
    }
    
    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
            GameEvents.NetworkEvents.OnMasterGameplayLoaded.Raise();
        
        StartMatchInternal();
        Application.runInBackground = true;
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }


    private void OnEnable()
    {
        GameEvents.NetworkGameplayEvents.NetworkSubmitRequest.Register(OnNetworkSubmitRequest);
        GameEvents.NetworkGameplayEvents.PlayerJoinedGame.Register(OnGameplayJoined);
        GameEvents.GameplayEvents.UserHandsEvaluated.Register(OnRoundScoreEvaluated);
        GameEvents.GameFlowEvents.RestartRound.Register(RestartGame);
        GameEvents.GameplayEvents.RoundMenuEnabled.Register(OnRoundCompleted);
        GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.Register(OnPlayerDisconnected);
    }

    private void OnPlayerDisconnected(int localID)
    {
        GameData.SessionData.CurrentRoomPlayersCount -= 1;
        
        NetworkManager.NetworkUtilities.RaiseRPC(m_NetworkGameplayManagerView, nameof(SyncPlayerCount_RPC),
            RpcTarget.AllBuffered, new object[]
            {
                GameData.SessionData.CurrentRoomPlayersCount
            });
        
        Evaluate();
    }

    private void OnDisable()
    {
        GameEvents.NetworkGameplayEvents.NetworkSubmitRequest.UnRegister(OnNetworkSubmitRequest);
        GameEvents.NetworkGameplayEvents.PlayerJoinedGame.UnRegister(OnGameplayJoined);
        GameEvents.GameplayEvents.UserHandsEvaluated.UnRegister(OnRoundScoreEvaluated);
        GameEvents.GameFlowEvents.RestartRound.UnRegister(RestartGame);
        GameEvents.GameplayEvents.RoundMenuEnabled.UnRegister(OnRoundCompleted);
        GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.UnRegister(OnPlayerDisconnected);
    }

    // private void OnApplicationFocus(bool hasFocus)
    // {
    //     if(!hasFocus)
    //         PhotonNetwork.Disconnect();
    //
    //     
    //         
    // }

    private void OnApplicationPause(bool pauseStatus)
    {
        // if (!pauseStatus)
        // {
        //     if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        //     {
        //         GameEvents.NetworkPlayerEvents.OnMasterLeftRoom.Raise();
        //         print("Not Connected");
        //         return;
        //     }
        // }
        //
        //
        //
        // NetworkViewComponent.RPC(nameof(OnPlayerAppUnFocus), RpcTarget.MasterClient, pauseStatus);
        //
        if(PhotonNetwork.OfflineMode)
            return;
        
        
        switch (pauseStatus)
        {
            case true:
                PhotonNetwork.Disconnect();
                break;
            case false:
            {
                if(!PhotonNetwork.IsConnected)
                    GameEvents.NetworkPlayerEvents.OnMasterLeftRoom
                        .Raise();
                break;
            }
        }
    }


    private void OnRoundCompleted()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;
            
        GameEvents.TimerEvents.ExecuteActionRequest.Raise(new TimerDataObject()
        {
            Title = _roundRestart,
            TimeDuration = GameData.MetaData.WaitBeforeAutomaticRoundStart,
            ActionToExecute =  RoundRestart,
            TickTimeEvent = TimerTick,
            IsNetworkGlobal = false
        });
    }
    public void TimerTick(string time)
    {
        NetworkViewComponent.RPC(nameof(GlobalTimerTick), RpcTarget.All, time);
    }

    private void RoundRestart()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        GameEvents.GameFlowEvents.RestartRound.Raise();
    }

    [PunRPC]
    public void OnPlayerAppUnFocus(bool status)
    {
        // if (status)
        //     GameData.SessionData.CurrentRoomPlayersCount--;
        // else
        //     GameData.SessionData.CurrentRoomPlayersCount++;
        //
        // print($"Players now : {GameData.SessionData.CurrentRoomPlayersCount}");
    }

    [PunRPC]
    public void GlobalTimerTick(string time)
    {
        GameEvents.NetworkEvents.RoundRestartTimer.Raise(time);
    }

    
    private void OnPlayerWin(int networkViewID,int runnerUpID,int secondRunnerUpID)
    {
        NetworkManager.NetworkUtilities.RaiseRPC(m_NetworkGameplayManagerView,nameof(AnnounceWinner_RPC),RpcTarget.All,new object[]
        {
            networkViewID,
            runnerUpID,
            secondRunnerUpID
        });
    }

    private void StartMatchInternal()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        StartMatch();
    }

    private void OnPlayerSpawned(PlayerController playerController)
    {
        m_NetworkMatchManager.OnPlayerSpawnedInMatch(playerController);
    }
    
    public virtual void OnNetworkSubmitRequest(NetworkDataObject networkDataObject)
    {
        string jsonData = NetworkDataObject.Serialize(networkDataObject);

        NetworkManager.NetworkUtilities.RaiseRPC(m_NetworkGameplayManagerView, nameof(OnNetworkSubmitRequest_RPC),
            RpcTarget.All, new object[] { jsonData });
    }

    private HashSet<int> m_AlreadyCheckedDecks = new ();
    
    [PunRPC]
    public void OnNetworkSubmitRequest_RPC(string jsonData)
    {
        NetworkDataObject dataObject = NetworkDataObject.DeSerialize(jsonData);
        
        if(!m_AlreadyCheckedDecks.Contains(dataObject.PhotonViewID))
            m_AllDecks.Add(dataObject);
        
        m_AlreadyCheckedDecks.Add(dataObject.PhotonViewID);
        Debug.LogError(m_AllDecks.Count);

        Evaluate();
    }

    private void Evaluate()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (m_AllDecks.Count == GameData.SessionData.CurrentRoomPlayersCount)
        {
            OnNetworkDeckReceived();
        }
    }
    
    private void OnNetworkDeckReceived()
    {
        GameEvents.NetworkGameplayEvents.AllUserHandsReceived.Raise(m_AllDecks);
    }

    private void OnRoundScoreEvaluated(Dictionary<int, PlayerScoreObject> userScores)
    {
        SyncUserScoresOverNetwork(new SerializableList<PlayerScoreObject>()
        {
            Contents = userScores.Values.ToList()
        });
        
        foreach (KeyValuePair<int, PlayerScoreObject> playerScores in userScores)
        {
            KeyValuePair<int, PlayerScoreObject> scoreItem = playerScores;
            m_NetworkPlayerSpawner.GetPlayerAgainstViewID(scoreItem.Key).AwardPlayerPoints(scoreItem.Value.Score);
        }
    }

    private void SyncUserScoresOverNetwork(SerializableList<PlayerScoreObject> playerScores)
    {
        string dataString = JsonUtility.ToJson(playerScores);
        
        NetworkManager.NetworkUtilities.RaiseRPC(m_NetworkGameplayManagerView,nameof(SyncUserScores_RPC),RpcTarget.All,new object[]
        {
            dataString
        });
    }

    public void OnGameplayJoined(PlayerController playerController)
    {
        m_NetworkPlayerSpawner.RegisterPlayer(playerController);
    }

    public void RestartGame()
    {
        NetworkManager.NetworkUtilities.RaiseRPC(m_NetworkGameplayManagerView, nameof(RestartGame_RPC), RpcTarget.All,
            null);

        GameData.RuntimeData.CURRENT_BOTS_FOR_SPAWNING = 0;
    }

    public void StartMatch()
    {
        BotCount = m_MatchStartRules.GetBotCount(GameData.SessionData.CurrentRoomPlayersCount);
        
        GameData.SessionData.CurrentRoomPlayersCount += BotCount;
        GameData.RuntimeData.CURRENT_BOTS_FOR_SPAWNING = BotCount;
        int count = GameData.SessionData.CurrentRoomPlayersCount;
        
        NetworkManager.NetworkUtilities.RaiseRPC(m_NetworkGameplayManagerView, nameof(SyncPlayerCount_RPC),
            RpcTarget.AllBuffered, new object[]
            {
                count
            });

    }

    [PunRPC]
    private void AnnounceWinner_RPC(int winnerNetworkViewID, int runnerUpNetworkViewID, int secondRunnerUpNetworkViewID)
    {
        List<int> scoresList = new List<int>()
        {
            winnerNetworkViewID,
            runnerUpNetworkViewID
        };

        if (secondRunnerUpNetworkViewID != GameData.MetaData.NullID)
            scoresList.Add(secondRunnerUpNetworkViewID);

        GameEvents.NetworkGameplayEvents.MatchWinnersAnnounced.Raise(scoresList,
            Dependencies.PlayersContainer.GetLocalPlayerNetworkID() == winnerNetworkViewID);

        GameEvents.GameFlowEvents.MatchOver.Raise();
    }

    [PunRPC]
    public void SyncPlayerCount_RPC(int count)
    {
        GameData.SessionData.CurrentRoomPlayersCount = count;
        print($"{count} RPC ");
    }
    
    [PunRPC]
    public void SyncUserScores_RPC(string data)
    {
        SerializableList<PlayerScoreObject> playerScores =
            JsonUtility.FromJson<SerializableList<PlayerScoreObject>>(data);
        
        GameEvents.NetworkGameplayEvents.PlayerScoresReceived.Raise(m_AllDecks, playerScores.Contents);
    }

    [PunRPC]
    public void RestartGame_RPC()
    {
        ResetMatch();
        m_NetworkMatchManager.RestartMatch();
        Invoke(nameof(DelayedReIteratePlayers), 1f);
    }
    
    
    private void ResetMatch()
    {
        m_AllDecks.Clear();   
        m_AlreadyCheckedDecks.Clear();
    }

    private void DelayedReIteratePlayers()
    {
        m_NetworkPlayerSpawner.ReIteratePlayerSpawns();
    }
}

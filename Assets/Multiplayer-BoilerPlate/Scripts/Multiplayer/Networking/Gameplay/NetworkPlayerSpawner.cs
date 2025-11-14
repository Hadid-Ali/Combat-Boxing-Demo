using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviour, INetworkPlayerSpawner
{
    [Tooltip("List of player prefab names, e.g. Player1, Player2, Player3")]
    [SerializeField] private List<string> m_PlayerPrefabNames = new();

    [Header("Spawn Points")]
    [Tooltip("Assign spawn points for players in order")]
    [SerializeField] private List<Transform> m_SpawnPoints = new();

    [SerializeField] private List<PlayerController> m_JoinedPlayers = new();
    private GameEvent<PlayerController> m_OnPlayerSpawned = new();

    [SerializeField] private NetworkGameplayManager m_Manager;
    private NetworkPlayerController m_PlayerController;

    private PhotonView _PhotonView;


    private void Awake()
    {
        Dependencies.PlayersContainer = this;
        if(!m_Manager) m_Manager = GetComponent<NetworkGameplayManager>();
    }

    private void Start()
    {
        _PhotonView = GetComponent<PhotonView>();
        SpawnPlayer();
        //Invoke(nameof(SpawnPlayer), 2f);
    }

    private void OnEnable()
    {
        GameEvents.NetworkGameplayEvents.PlayerScoresReceived.Register(OnPlayerScoresReceived);
    }

    private void OnDisable()
    {
        GameEvents.NetworkGameplayEvents.PlayerScoresReceived.UnRegister(OnPlayerScoresReceived);
    }

    public int GetPlayerLocalID(int photonViewID) => m_JoinedPlayers.Find(player => player.ID == photonViewID).LocalID;
    public int GetPlayerSelectedCard(int ID) => (int) m_JoinedPlayers.Find(player => player.ID == ID).SelectedCard;

    public int GetLocalPlayerNetworkID() => m_JoinedPlayers.Find(player => player.IsLocalPlayer).ID;

    public void Initialize(Action<PlayerController> onPlayerSpawned)
    {
        m_OnPlayerSpawned.Register(onPlayerSpawned);
    }

    public void SpawnPlayer()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Photon not ready yet. Cannot spawn player.");
            return;
        }

        // Use ActorNumber to determine index
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = (actorNumber - 1) % m_PlayerPrefabNames.Count;

        string prefabName = m_PlayerPrefabNames[index];

        Vector3 spawnPos = (m_SpawnPoints.Count > index)
            ? m_SpawnPoints[index].position
            : Vector3.zero;

        Vector3 rotation = (m_SpawnPoints.Count > index)
            ? m_SpawnPoints[index].eulerAngles
            : Vector3.zero;

        Debug.Log($"Spawning player prefab '{prefabName}' at position {spawnPos}");
        var player = PhotonNetwork.Instantiate($"Network/Player/{prefabName}", 
            spawnPos, Quaternion.Euler(rotation), 0);
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        m_JoinedPlayers.Add(playerController);
        OnPlayerSpawned(playerController);
        
        m_JoinedPlayers.RemoveAll(player => player == null);
    }

    public void ReIteratePlayerSpawns()
    {
        for (int i = 0; i < m_JoinedPlayers.Count; i++)
        {
            OnPlayerSpawned(m_JoinedPlayers[i]);
        }
    }

    public PlayerController GetPlayerAgainstViewID(int ID) =>
        m_JoinedPlayers.Find(player => player.ID == ID);
    
    public PlayerController GetPlayerAgainstActorID(int ID)=>
        m_JoinedPlayers.Find(player => player.LocalID == ID);
    
    public string GetPlayerName(int ID) => GetPlayerAgainstViewID(ID).Name;
    
    private void OnPlayerScoresReceived(List<NetworkDataObject> networkDataObjects, List<PlayerScoreObject> playerScoreObjects)
    {
        int ownID =  m_JoinedPlayers.Find(player => player.IsLocalPlayer && !player.IsBot).ID;
        PlayerScoreObject obje = playerScoreObjects.Find(player => player.UserID == ownID);
        
        GameData.RuntimeData.AddToTotalPlayerScore(obje.Score);
        GameEvents.GameplayEvents.RoundCompleted.Raise();
    }
    
    private void OnPlayerSpawned(PlayerController playerController)
    {
        m_OnPlayerSpawned.Raise(playerController);
    }
}

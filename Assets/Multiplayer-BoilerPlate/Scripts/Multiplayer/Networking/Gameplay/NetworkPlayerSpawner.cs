using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviour, INetworkPlayerSpawner
{
    [SerializeField] private List<PlayerController> m_JoinedPlayers = new();
    private GameEvent<PlayerController> m_OnPlayerSpawned = new();

    [SerializeField] private NetworkGameplayManager m_Manager;
    private NetworkPlayerController m_PlayerController;
    

    private void Awake()
    {
        Dependencies.PlayersContainer = this;
        if(!m_Manager) m_Manager = GetComponent<NetworkGameplayManager>();
    }

    private void Start()
    {
        Invoke(nameof(SpawnPlayer), 2f);
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
        PhotonNetwork.Instantiate($"Network/Player/Avatars/PlayerAvatar", Vector3.zero,
            Quaternion.identity, 0);
        ;
        if(m_Manager.BotCount <= 0 || !PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < m_Manager.BotCount; i++)
        {
            PhotonNetwork.Instantiate($"Network/Player/Avatars/BotAvatar", Vector3.zero,
                Quaternion.identity, 0);
        }
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

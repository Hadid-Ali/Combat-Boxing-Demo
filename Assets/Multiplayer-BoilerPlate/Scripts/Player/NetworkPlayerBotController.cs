
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkPlayerBotController : NetworkPlayerController
{
    [FormerlySerializedAs("_botManager")] [SerializeField] private NetworkBotCardsController botCardsController;

    private string m_DefaultName = "Lenart"; 
    private string[] m_BotNames = { "Mark", "Hudson" };
    
    protected override void Start()
    {
        base.Start();
        IsBot = true;
    }

    protected override void SetPlayerDataOverServer()
    {
        Name = GameData.RuntimeData.TOTAL_BOTS_SPAWNED <=0 ? m_DefaultName : m_BotNames[Random.Range(0, m_BotNames.Length)];
        
        int totalPlayers = GameData.SessionData.CurrentRoomPlayersCount;
        int actorNum = (totalPlayers - (GameData.RuntimeData.CURRENT_BOTS_FOR_SPAWNING--)) + 1;  
        
        
        NetworkManager.NetworkUtilities.RaiseRPC(m_PhotonView, nameof(SetPlayerData_RPC), RpcTarget.All,
            new object[] { Name, actorNum ,Random.Range(0,8),(int) ItemName.DefaultCardB});
        
        GameData.RuntimeData.TOTAL_BOTS_SPAWNED++;
    }
    
    [PunRPC]
    public override void ReceiveHandFromNetwork(string data, int _ID)
    {
        if (!m_PhotonView.IsMine || _ID != ID)
            return;

        NetworkHandObject handObject = NetworkHandObject.DeSerialize(data);
        botCardsController.ReceiveHandData(handObject.PlayerHand, _ID);
    }
    
    protected override void OnSubmitDeck()
    {
        string jsonData = NetworkDataObject.Serialize(new NetworkDataObject(botCardsController.GetCards(), ID));
        
        if(!PhotonNetwork.IsMasterClient)
            return;
        
        NetworkGameplayManager manager = FindObjectOfType<NetworkGameplayManager>();
        
        NetworkManager.NetworkUtilities.RaiseRPC(manager.NetworkViewComponent, nameof(manager.OnNetworkSubmitRequest_RPC),
            RpcTarget.All, new object[] { jsonData });
    }
}

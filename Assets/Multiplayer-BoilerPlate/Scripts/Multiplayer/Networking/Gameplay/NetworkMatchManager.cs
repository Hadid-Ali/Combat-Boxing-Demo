using UnityEngine;
using Photon.Pun;

public class NetworkMatchManager : MonoBehaviour
{
    [SerializeField] private NetworkCardsDealer m_CardsDealer;
    [SerializeField] private bool m_CanDeal = true;
    
    private int numberOfTimesDealt = 0;

    private void OnEnable()
    {
        GameEvents.GameFlowEvents.RestartRound.Register(OnRestartRound);
    }


    private void OnDisable()
    {
        GameEvents.GameFlowEvents.RestartRound.UnRegister(OnRestartRound);
    }

    private void OnRestartRound()
    {
        m_CanDeal = true;
    }
    
    public void OnPlayerSpawnedInMatch(PlayerController playerController)
    {
        //Invoke(nameof(StopFurtherDealing), 1f);
        
        if (!PhotonNetwork.IsMasterClient )//|| !m_CanDeal)
            return;

        m_CardsDealer.DealCardsToNetworkPlayer(playerController);
    }


    private void StopFurtherDealing()
    {
        m_CanDeal = false;
    }

    public void RestartMatch()
    {
        GameEvents.GameFlowEvents.RoundStart.Raise();
    }
}

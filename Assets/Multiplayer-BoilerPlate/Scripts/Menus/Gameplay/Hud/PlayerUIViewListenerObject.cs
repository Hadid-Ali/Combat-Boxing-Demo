using System.Collections.Generic;
using UnityEngine;

public class PlayerUIViewListenerObject : PlayerViewListenerObject
{
    [SerializeField] private List<PlayerScoreUIObject> m_ScoreObjects = new();

    protected override void Awake()
    {
        base.Awake();
        GameEvents.GameplayEvents.PlayerScoreReceived.Register(OnPlayerScoreReceived);
        GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.Register(OnPlayerLeftRoom);
    }

    private void OnPlayerLeftRoom(int objj)
    {
        PlayerScoreUIObject scoreObject = m_ScoreObjects.Find(obj => obj.PositionIndex == objj);
        scoreObject.SetContainerStatus(false);
        scoreObject.SetScore(0);
    }

    protected override void OnDestroy() 
    {
        base.OnDestroy();
        GameEvents.GameplayEvents.PlayerScoreReceived.UnRegister(OnPlayerScoreReceived);
        GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.UnRegister(OnPlayerLeftRoom);
    }

    private void OnPlayerScoreReceived(int score, int playerId)
    {
        PlayerScoreUIObject scoreObject = m_ScoreObjects.Find(obj 
            => obj.PositionIndex == playerId);
        
        PlayerController playerController = 
            Dependencies.PlayersContainer.GetPlayerAgainstActorID(playerId);
        
        if (playerController == null)
            return;
        
        scoreObject.SetContainerStatus(true);
        scoreObject.SetScore(score);
    }
    
    protected override void OnLocalPlayerJoined(PlayerViewDataObject viewDataObject)
    {
        PlayerScoreUIObject scoreObject = m_ScoreObjects.Find(obj => obj.PositionIndex == viewDataObject.LocalID);
        scoreObject.SetContainerStatus(true);
        scoreObject.SetName(viewDataObject.Name);
    }
}

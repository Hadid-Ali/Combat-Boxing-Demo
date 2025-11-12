using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTableViewListenerObject : PlayerViewListenerObject
{
    [SerializeField] private List<PlayerTablePosition> m_TablePositions = new();
    [SerializeField] private GameObject m_CardsHeaderObject;

    protected override void Awake()
    {
        base.Awake();
        GameEvents.GameFlowEvents.MatchOver.Register(OnMatchOver);
        GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.Register(OnPlayerLeftRoom);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEvents.GameFlowEvents.MatchOver.UnRegister(OnMatchOver);
        GameEvents.NetworkPlayerEvents.OnPlayerLeftRoom.UnRegister(OnPlayerLeftRoom);
    }

    private void OnMatchOver()
    {
        m_CardsHeaderObject.SetActive(false);
    }

    protected override void OnLocalPlayerJoined(PlayerViewDataObject viewDataObject)
    {
        PlayerTablePosition position =
            m_TablePositions.Find(player => player.TablePositionIndex == viewDataObject.LocalID);
        
        position.SetAvatarIndex(viewDataObject.AvatarID);
    }
    
    private void OnPlayerLeftRoom(int obj)
    {
        PlayerTablePosition position =
            m_TablePositions.Find(player => player.TablePositionIndex == obj);
        
        position.ResetPosition();
    }
}

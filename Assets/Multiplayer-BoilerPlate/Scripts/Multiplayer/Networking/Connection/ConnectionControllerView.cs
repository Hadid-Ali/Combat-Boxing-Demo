using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ConnectionControllerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_MatchStartTimerComponent;
    
    private GameEvent<MatchMode,string> m_MatchStartRequested = new();
    private GameEvent<Region> m_OnRegionSelect = new();
    private GameEvent<RoomOptions> m_OnCreateRoom = new();
    private GameEvent m_OnCharacterSelected = new();

    private void OnEnable()
    {
        GameEvents.MenuEvents.MatchStartRequested.Register(OnMatchStartRequested);
        GameEvents.NetworkEvents.PlayerRegionSelect.Register(OnRegionSelection);
        GameEvents.NetworkEvents.PlayerRoomCreation.Register(OnCreateRoom);
        GameEvents.NetworkEvents.PlayerCharacterSelected.Register(OnPlayerCharacterSelected);
    }

    private void OnDisable()
    {
        GameEvents.MenuEvents.MatchStartRequested.UnRegister(OnMatchStartRequested);
        GameEvents.NetworkEvents.PlayerRegionSelect.UnRegister(OnRegionSelection);
        GameEvents.NetworkEvents.PlayerRoomCreation.UnRegister(OnCreateRoom);
        GameEvents.NetworkEvents.PlayerCharacterSelected.UnRegister(OnPlayerCharacterSelected);
    }
    
    public void Initialize(Action<MatchMode,string> onLogin, Action<Region> onRegionSelect, Action<RoomOptions> onCreateRoom,
        Action onCharacterSelect)
    {
        m_MatchStartRequested.Register(onLogin);
        m_OnRegionSelect.Register(onRegionSelect);
        m_OnCreateRoom.Register(onCreateRoom);
        m_OnCharacterSelected.Register(onCharacterSelect);
    }

    private void OnMatchStartRequested(MatchMode matchMode,string userName)
    {
        m_MatchStartRequested.Raise(matchMode, userName);
    }

    private void OnRegionSelection(Region region)
    {
        m_OnRegionSelect.Raise(region);
    }

    private void OnCreateRoom(RoomOptions roomOptions)
    {
        m_OnCreateRoom.Raise(roomOptions);
    }

    private void OnPlayerCharacterSelected()
    {
        m_OnCharacterSelected.Raise();
    }
}

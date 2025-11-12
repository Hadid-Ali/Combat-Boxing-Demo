using Assets.Scripts._UXUI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LoadingScreen : UIMenuBase
{
    [Header("Component Refs")]
    [SerializeField] private AnimatedTextWidget m_AnimatedTextWidget;
    [SerializeField] private WaitingText m_WaitingText;
    
    [Header("UI Refs")]
    [SerializeField] private TextMeshProUGUI m_LobbyStatusText;
    
    private void OnEnable()
    {
        GameEvents.NetworkEvents.NetworkStatus.Register(UpdateLobbyStatus);
        //GameEvents.MenuEvents.TimeBasedActionRequested.Register(OnTimeBasedActionRequested);
        
        GameEvents.NetworkEvents.RoomJoinFailed.Register(OnRoomJoinFailed);
        GameEvents.NetworkEvents.LobbyJoined.Register(OnCreateRoom);
    }

    private void OnDisable()
    {
        GameEvents.NetworkEvents.NetworkStatus.UnRegister(UpdateLobbyStatus);
        //GameEvents.MenuEvents.TimeBasedActionRequested.UnRegister(OnTimeBasedActionRequested);
        
        GameEvents.NetworkEvents.RoomJoinFailed.UnRegister(OnRoomJoinFailed);
        GameEvents.NetworkEvents.LobbyJoined.UnRegister(OnCreateRoom);
    }
    


    private void OnRoomJoinFailed()
    {
        int maxPlayerCount = GameData.MetaData.MaxPlayersLimit;
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayerCount,
            IsOpen = true,
            IsVisible = true,
            EmptyRoomTtl = 100,
        };

        GameEvents.NetworkEvents.PlayerRoomCreation.Raise(roomOptions);
    }



    private void OnCreateRoom()
    {
        GameEvents.NetworkEvents.PlayerCharacterSelected.Raise();
    }

    private void OnServerDisconnect(RegionConfig obj)
    {
        ChangeMenuState(MenuName.LoginScreen);
    }

    private void UpdateLobbyStatus(string status)
    {
        m_LobbyStatusText.text = status;
        m_AnimatedTextWidget.AnimateText(status);
    }
    private void OnTimeBasedActionRequested(string message, float wait)
    {
        m_WaitingText.StartTimer(message, wait);
    }
}

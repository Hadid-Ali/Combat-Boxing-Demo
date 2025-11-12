using UnityEngine;

public class MainMenu : UIMenuBase
{
    [SerializeField] private ButtonWidget m_PlayButton;
    [SerializeField] private ButtonWidget m_PlayOfflineButton;
    [SerializeField] private ButtonWidget m_SettingsButton;
    [SerializeField] private ButtonWidget m_StoreButton;
    [SerializeField] private ButtonWidget m_HowToPlay;
    
    [SerializeField] private VideoControllerUIHandler m_HowToPlayVideoObject;
    
    
    private void Start()
    {
        m_PlayButton.SubscribeAction(LoginBtnEvent);
        m_SettingsButton.SubscribeAction(OnSettingsButton);
        m_PlayOfflineButton.SubscribeAction(StartOffline);
        m_StoreButton.SubscribeAction(OnStoreButtonTap);
        
        
        m_HowToPlay.SubscribeAction(StartHowToPlayVideo);
        m_HowToPlayVideoObject.Initialize();
    }
    private void StartHowToPlayVideo()
    {
        m_HowToPlayVideoObject.Show();
    }

    private void OnStoreButtonTap()
    {
        ChangeMenuState(MenuName.StoreSelection);
    }
    
    private void StartOffline()
    {
        TransitToConnectionScreen(MatchMode.OFFLINE);
    }

    private void OnSettingsButton()
    {
        ChangeMenuState(MenuName.SettingsMenu);
    }
    
    public void LoginBtnEvent()
    {
        TransitToConnectionScreen(MatchMode.ONLINE);
    }

    private void TransitToConnectionScreen(MatchMode matchMode)
    {
        GameEvents.MenuEvents.MatchStartRequested.Raise(matchMode,GameData.PersistentData.UserName);
        ChangeMenuState(MenuName.ConnectionScreen);
    }
}
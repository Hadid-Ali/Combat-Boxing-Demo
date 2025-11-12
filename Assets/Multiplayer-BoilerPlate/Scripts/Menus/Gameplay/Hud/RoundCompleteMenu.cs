using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoundCompleteMenu : UIMenuBase
{
    [Header("Components")] 
    [SerializeField] private TMP_Text m_RewardText;
    [SerializeField] private TMP_Text m_AutoStartText;
    //[SerializeField] private Button m_RestartButton;

    private void Start()
    {
       GameEvents.NetworkEvents.RoundRestartTimer.Register(TimerTick);
    }

    private void OnDestroy()
    {
       GameEvents.NetworkEvents.RoundRestartTimer.UnRegister(TimerTick);
    }

    protected override void OnContainerEnable()
    {
        base.OnContainerEnable();
        GameEvents.GameplayEvents.RoundMenuEnabled.Raise();
    }

    private void OnEnable()
    {
        GameEvents.GameplayUIEvents.PlayerRewardReceived.Register(OnPlayerRewardReceived);
        GameEvents.GameFlowEvents.RoundStart.Register(DisableMenu);
        
    }

    private void OnDisable()
    {
        GameEvents.GameplayUIEvents.PlayerRewardReceived.UnRegister(OnPlayerRewardReceived);
        GameEvents.GameFlowEvents.RoundStart.UnRegister(DisableMenu);
    }

    private void DisableMenu()
    {
        SetMenuActiveState(false);
    }

    private void OnPlayerRewardReceived(int reward)
    {
        m_RewardText.text = $"You Received {reward} Points in this Round";
    }


    private void TimerTick(string str)
    {
        m_AutoStartText.SetText($"Next Round Starting in {str}...");
    }

    private void OnRestartTap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //GameEvents.GameFlowEvents.RestartRound.Raise();
        }
        // else
        // {
        //     ChangeMenuState(MenuName.LoadingScreen);
        // }
    }
}

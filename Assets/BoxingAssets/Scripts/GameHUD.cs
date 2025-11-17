using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI player1PointsText;
    [SerializeField] private TextMeshProUGUI player2PointsText;

    private int totalRounds = 3;

    private void OnEnable()
    {
        GameEvents.BoxingDemoGameFlowEvents.RoundStart.Register(UpdateRoundText);
        GameEvents.BoxingDemoGameFlowEvents.RoundComplete.Register(UpdatePlayerPoints);
    }

    private void OnDisable()
    {
        GameEvents.BoxingDemoGameFlowEvents.RoundStart.UnRegister(UpdateRoundText);
        GameEvents.BoxingDemoGameFlowEvents.RoundComplete.UnRegister(UpdatePlayerPoints);
    }


    public void UpdateRoundText(int round)
    {
        roundText.text = "Rounds " + round.ToString() + "/" + totalRounds;
    }

    public void UpdatePlayerPoints(int winnerID, int currentRound)
    {
        UpdateRoundText(currentRound);

        int currentPoints1 = GameplayManager.instance.roundTracker.Getplayer1Wins();
        player1PointsText.text = "Points: " + currentPoints1.ToString();

        int currentPoints2 = GameplayManager.instance.roundTracker.Getplayer2Wins();
        player2PointsText.text = "Points: " + currentPoints2.ToString();
    }
}

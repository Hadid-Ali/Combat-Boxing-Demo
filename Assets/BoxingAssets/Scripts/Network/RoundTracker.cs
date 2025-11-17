using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoundTracker : MonoBehaviourPun
{
    // Settings
    public int maxRounds = 3;

    //Temporary UI Elements
    public GameObject matchResultUI;
    public TextMeshProUGUI matchResultText;

    // Runtime tracking
    private int currentRound = 0;
    private int player1Wins = 0;
    private int player2Wins = 0;


    public int Getplayer1Wins()
    {
        return player1Wins;
    }

    public int Getplayer2Wins()
    {
        return player2Wins;
    }


    public void RegisterRoundWinner(int winnerPlayerID)
    {
        // Broadcast to all clients
        photonView.RPC(nameof(RPC_RegisterRoundWinner), RpcTarget.All, winnerPlayerID);
    }


    public void ShowWinPanel()
    {
        if (matchResultUI != null)
        {
            matchResultText.text = player1Wins > player2Wins ? 
                "Player 1 Wins!" : player2Wins > player1Wins ? "Player 2 Wins!" : "It's a Draw!";
            matchResultUI.SetActive(true);
        }
    }


    [PunRPC]
    private void RPC_RegisterRoundWinner(int winnerPlayerID)
    {
        currentRound++;

        if (winnerPlayerID == 1) player1Wins++;
        else if (winnerPlayerID == 2) player2Wins++;

        GameEvents.BoxingDemoGameFlowEvents.RoundComplete.Raise(winnerPlayerID, currentRound); 

        if (currentRound >= maxRounds)
            EvaluateMatchWinner();
    }

    private void EvaluateMatchWinner()
    {
        int matchWinner = -1; // 0 = draw

        if (player1Wins > player2Wins) matchWinner = 1;
        else if (player2Wins > player1Wins) matchWinner = 2;

        photonView.RPC(nameof(RPC_MatchFinished), RpcTarget.All, matchWinner);
    }

    [PunRPC]
    private void RPC_MatchFinished(int matchWinnerID)
    {
        GameEvents.BoxingDemoGameFlowEvents.MatchFinished.Raise(matchWinnerID);
    }

    public void ResetRounds()
    {
        currentRound = 0;
        player1Wins = 0;
        player2Wins = 0;
    }

    public int CurrentRound => currentRound;
    public int Player1Wins => player1Wins;
    public int Player2Wins => player2Wins;
}

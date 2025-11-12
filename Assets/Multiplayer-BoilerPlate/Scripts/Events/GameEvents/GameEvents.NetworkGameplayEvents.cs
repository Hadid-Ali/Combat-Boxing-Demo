using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static class NetworkGameplayEvents
    {
        public static GameEvent<NetworkDataObject> NetworkSubmitRequest = new();
        public static GameEvent<List<NetworkDataObject>> AllUserHandsReceived = new();
        public static GameEvent<CardData[], int> UserHandReceivedEvent = new();
        public static GameEvent<List<NetworkDataObject>, List<PlayerScoreObject>> PlayerScoresReceived = new();
        public static GameEvent<PlayerController> PlayerJoinedGame = new();
        public static GameEvent<List<int>,bool> MatchWinnersAnnounced = new();
    }
    
    public static class NetworkPlayerEvents
    {
        public static readonly GameEvent<int, bool> OnPlayerTurn = new();
        public static readonly GameEvent<int, int> OnPlayerCreditsChanged = new();
        public static readonly GameEvent<int, string> OnPlayerActionPop = new();
        
        public static readonly GameEvent<List<CardData>> ExposePocketCardsLocally = new();

        public static GameEvent<int, int> OnSetPlayerRaiseLimits = new();
        public static GameEvent<int> OnPlayerLeftRoom = new();
        public static GameEvent<int, bool> OnPlayerApplicationFocused = new();
        public static GameEvent OnMasterLeftRoom = new();

    }
}

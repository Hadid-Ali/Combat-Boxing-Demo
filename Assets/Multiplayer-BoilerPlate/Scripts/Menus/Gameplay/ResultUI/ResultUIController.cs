using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResultUIController : MonoBehaviour
{
    [Header("Refs")]
    
    [SerializeField] private ResultUIView m_ResultUiView;
    
    [Header("Attributes")]
    
    [SerializeField] private float m_WaitBeforeWinnerReveal;
    
    private List<PlayerScoreObject> m_UsersScoreList = new();
    private List<PlayerDecksObject> m_PlayerDecks = new();
    
    private GameEvent m_OnResultViewClose = new();
    

    private void OnEnable()
    {
        GameEvents.NetworkGameplayEvents.PlayerScoresReceived.Register(OnPlayerScoresReceived);
    }

    private void OnDisable()
    {
        GameEvents.NetworkGameplayEvents.PlayerScoresReceived.UnRegister(OnPlayerScoresReceived);
    }

    public void Initialize(Action onMenuClose)
    {
        m_OnResultViewClose.Register(onMenuClose);
    }
        
    private void OnPlayerScoresReceived(List<NetworkDataObject> playerNetworkDataObjects,
        List<PlayerScoreObject> playerScoreObjects)
    {
        m_UsersScoreList = playerScoreObjects;
        InitializePlayerDecks(playerNetworkDataObjects);
        
        SetupShowResultUI();
    }

    private void InitializePlayerDecks(List<NetworkDataObject> playerNetworkDataObjects)
    {
        m_PlayerDecks.Clear();
        for (int i = 0; i < playerNetworkDataObjects.Count; i++)
        {
            NetworkDataObject obj = playerNetworkDataObjects[i];

            m_PlayerDecks.Add(new PlayerDecksObject()
            {
                userID = obj.PhotonViewID,
                Decks = obj.GetDeck()
            });
        }
    }
    
    private void SetupShowResultUI()
    {
        StartCoroutine(ShowResultantUI_Routine());
    }

    private IEnumerator ShowResultantUI_Routine()
    {
        int index = 0;
        int totalHands = GameData.MetaData.DecksCount;
        
        GameEvents.GameplayEvents.GameplayStateSwitched.Raise(GameplayState.Result_Deck_View);
        
        WaitForSeconds wait = new(GameData.SessionData.CurrentRoomPlayersCount > 3
            ? GameData.MetaData.RevealTimeForSixPlayersGame
            : GameData.MetaData.RevealTimeForThreePlayersGame);
        
        while (index < totalHands)
        {
            ShowDecksAtIndex(index++);
            
            var waitTime = GameData.SessionData.CurrentRoomPlayersCount > 3 ? 
                GameData.MetaData.RevealTimeForSixPlayersGame : GameData.MetaData.RevealTimeForThreePlayersGame;
            
            Debug.Log(waitTime);
            yield return wait;
        }

        m_ResultUiView.Reset();
        m_ResultUiView.SetActiveState(false);
        m_OnResultViewClose.Raise();
        
        GameEvents.GameplayEvents.GameplayStateSwitched.Raise(GameplayState.Result_Score_View);
    }

    Cardvalue[] _specialstraightSet = 
    {
        Cardvalue.value_2, Cardvalue.value_3,
        Cardvalue.value_4, Cardvalue.value_5,
        Cardvalue.value_A
    };
    private void ShowDecksAtIndex(int index)
    {
        ResultHandDataObject[] resultObjects = new ResultHandDataObject[m_PlayerDecks.Count];
        
        for (int i = 0; i < m_PlayerDecks.Count; i++)
        {
            PlayerDecksObject deckObject = m_PlayerDecks[i];
            CardData [] cardData = deckObject.Decks[index];

            var tempcData = cardData;

            foreach (var v in tempcData)
                if(v.value == Cardvalue.valueS_A)
                    v.value = Cardvalue.value_A;
            
            HandEvaluator.Evaluate(tempcData, out HandTypes handTypes);

            List<CardData> cardList = cardData.ToList();

            
            if (handTypes == HandTypes.Straight || handTypes == HandTypes.StraightFlush) // Check both cases
            {
                bool isSpecialCase = cardData.Select(card => card.value)
                    .OrderBy(value => value) // Sort to match the sequence
                    .SequenceEqual(_specialstraightSet);

                if (isSpecialCase)
                {
                    foreach (var card in cardData)
                    {
                        if (card.value == Cardvalue.value_A)
                            card.value = Cardvalue.valueS_A; // Change Ace's value to custom lowest
                        print("Special Case");
                    }
                }
            }
            
            if (handTypes is HandTypes.FullHouse)//Exception for fullhouse
            {
                var groupedByValue = cardList.GroupBy(card => card.value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                List<CardData> threeOfAKind = null;
                List<CardData> pair = null;

                // Check for three of a kind and pair
                foreach (var group in groupedByValue.Values)
                {
                    switch (group.Count)
                    {
                        case 3:
                            threeOfAKind = group;
                            break;
                        case 2:
                            pair = group;
                            break;
                    }
                }
                if (threeOfAKind != null && pair != null)
                {
                    cardList = threeOfAKind.Concat(pair).ToList(); //Concatenating with pair so threeOfAKind comes before pair 
                    cardData = cardList.ToArray(); 
                }
            }
            else //Order the final list by Descending
            {
                var descOrdered = cardList.OrderByDescending(x => x.value);
                cardData = descOrdered.ToArray();
            }
            
            
            resultObjects[i] = new ResultHandDataObject()
            {
                CardBack = CardsRegistery.Instance.GetCardSprite((ItemName)m_UsersScoreList[i].SelectedCard),
                Cards = GetCardSprites(cardData),
                Score = GameData.MetaData.HandWinReward,
                PlayerName = Dependencies.PlayersContainer.GetPlayerName(deckObject.userID),
                IsWinner = IsHandIndexWinner(deckObject.userID, index),
                WinnerRevealDuration = m_WaitBeforeWinnerReveal,
                HandName = DeckHandsRegistry.Instance.GetHandTypeName(handTypes)
            };
        }

        m_ResultUiView.SetResultData(resultObjects, index);
    }

    private Sprite[] GetCardSprites(CardData[] cardData)
    {
        Sprite[] sprites = new Sprite[cardData.Length];

        for (int i = 0; i < cardData.Length; i++)
        {
            sprites[i] = CardsRegistery.Instance.GetCardSprite(cardData[i].type, cardData[i].value);
        }

        return sprites;
    }

    private int UserScore(int userID) => m_UsersScoreList.Find(user => user.UserID == userID).Score;

    private bool IsHandIndexWinner(int userID,int handIndex)
    {
        PlayerScoreObject scoreObject = m_UsersScoreList.Find(user => user.UserID == userID);

        if (!scoreObject.WinningHandsIndexes.Any())
            return false;

        return scoreObject.WinningHandsIndexes.Contains(handIndex);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkBotCardsController : MonoBehaviour
{
    [SerializeField] public List<CardData> cards = new();
    [SerializeField] private NetworkPlayerBotController _botController;
    [SerializeField] private PhotonView view;
    
    private int shuffleLimit = 15;


    private BotsDifficulty _botDifficulty;

    private void Awake()
    {
        SetBotDifficulty();
    }

    public void SetBotDifficulty()
    {
        int botsDiffInt = PlayerPrefs.GetInt(GameData.MetaData.BotDifficulty, 
            (int) GameData.MetaData.DefaultBotDifficulty);

        _botDifficulty = (BotsDifficulty) botsDiffInt;
        
        // shuffleLimit = _botDifficulty  switch
        // {
        //     BotsDifficulty.Easy => 0,
        //     BotsDifficulty.Medium => 7,
        //     BotsDifficulty.Hard => 15,
        //     _ => shuffleLimit
        // };
    }
    public void ReceiveHandData(CardData[] obj, int _ID)
    {
        if(_ID != _botController.ID)
            return;

        cards.Clear();
        cards.AddRange(obj);
        foreach (var v in cards.Where(v => v.value == Cardvalue.valueS_A))
        {
            v.value = Cardvalue.value_A;
        }

        switch (_botDifficulty)
        {
            case BotsDifficulty.Easy:
                break;
            case BotsDifficulty.Medium:
                StartCoroutine(ShuffleToBestCards(BotsDifficulty.Medium));
                break;
            case BotsDifficulty.Hard:
                StartCoroutine(ShuffleToBestCards(BotsDifficulty.Hard));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private Handd hand1 = new();

    private Handd Besthand1 = new();
    private Handd Besthand2 = new();
    private Handd Besthand3 = new();

    IEnumerator  ShuffleToBestCards(BotsDifficulty botDifficulty)
    {
        for (int i = 0; i < shuffleLimit; i++)
            hand1.Add(cards[i]);

        Besthand1.Clear();
        Besthand2.Clear();
        Besthand3.Clear();
        
        Besthand1 = CombinationСalculator.GetBestHanddEfficiently(hand1);

        yield return new WaitForSeconds(.3f);

        foreach (var t in Besthand1._Handd)
            hand1.Remove(t);
        
        Besthand2 = CombinationСalculator.GetBestHanddEfficiently(hand1);

        yield return new WaitForSeconds(.3f);
        
        foreach (var t in Besthand2._Handd)
            hand1.Remove(t);
        
        Besthand3 = CombinationСalculator.GetBestHanddEfficiently(hand1);
        
        yield return new WaitForSeconds(.3f);
            
        foreach (var t in Besthand3._Handd)
            hand1.Remove(t);

        List<CardData> shuffledCards = new();
        
        yield return new WaitForSeconds(.3f);
        
        
        shuffledCards.AddRange(Besthand1._Handd);
        
        yield return new WaitForSeconds(.1f);
        if (_botDifficulty == BotsDifficulty.Hard)
        {
            shuffledCards.AddRange(Besthand2._Handd);
            shuffledCards.AddRange(Besthand3._Handd);
            shuffledCards.AddRange(hand1._Handd);
            
            hand1.Clear();
            cards.Clear();
            cards = shuffledCards;
        }
        else
        {
            for (int i = 0; i < shuffledCards.Count; i++)
            {
                cards[i] = shuffledCards[i];
            }
        }
    }
    public void ShuffleAndInterchangeGroups()
    {
        if (cards.Count != 15)
        {
            Debug.LogWarning("The card list must contain exactly 15 cards to shuffle and interchange groups.");
            return;
        }
        
        var group1 = cards.GetRange(0, 5);
        var group2 = cards.GetRange(5, 5);
        var group3 = cards.GetRange(10, 5);
        
        var groups = new List<List<CardData>> { group1, group2, group3 };
        groups = groups.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                cards[i * 5 + j] = groups[i][j];
            }
        }
    }

    public List<CardData> GetCards()
    {
        ShuffleAndInterchangeGroups();
        return cards;
    }
    
}

using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    [SerializeField] CardNamesScriptable cardDictionary;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardParent;
    [SerializeField] int totalCardsNeeded;
    [SerializeField] int index = 0;
    [SerializeField] List<GameObject> cardsInstantiated;

    public delegate void CloneCards();
    public static event CloneCards onCloneCard;
    [SerializeField] List<CardNamesScriptable.Card> powers;
    private void Start()
    {
        powers.AddRange(cardDictionary.cards);
        InstantiatingCards();
    }
    private void OnEnable()
    {
        onCloneCard += IntantiateCards;
    }

    private void OnDisable()
    {
        onCloneCard -= IntantiateCards;
    }

    public static void InstantiatingCards()
    {
        onCloneCard?.Invoke();
    }
    void IntantiateCards()
    {
        if (index < totalCardsNeeded)
        {
            if (powers.Count < totalCardsNeeded)
            {
                powers.Clear();
                powers.AddRange(cardDictionary.cards);
            }
            int r = Random.Range(0, (powers.Count-1));
            GameObject g = Instantiate(cardPrefab, cardParent.position, Quaternion.identity);
            cardsInstantiated.Add(g);
            g.transform.SetParent(cardParent.transform);
            Card_Info c_info = g.GetComponent<Card_Info>();
            c_info.m_TextMeshPro.text = cardDictionary.cards[r].name;
            if(cardDictionary.cards[r].cardSprite)
                c_info.icon.sprite = cardDictionary.cards[r].cardSprite;
            powers.Remove(powers[r]);
            index++;
            IntantiateCards();
        }
    }

    void ResetIndex()
    {
        index = 0;
        powers.AddRange(cardDictionary.cards);
        //powers.Reverse();
    }
}

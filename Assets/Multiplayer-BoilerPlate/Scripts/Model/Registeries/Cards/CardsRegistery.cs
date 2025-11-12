using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsRegistery : MonobehaviourSingleton<CardsRegistery>
{
    [SerializeField] private List<CardDataObject> m_Cards = new();
    [SerializeField] private List<ItemSO> m_CardBacks = new();

    public Sprite GetCardSprite(CardType cardType, Cardvalue cardValue) 
    {
        if (cardValue == Cardvalue.valueS_A)
        {
            cardValue = Cardvalue.value_A;
        }
        
        return m_Cards.Find(card => card.type == cardType && card.value == cardValue).CardImage;
    }

    public Sprite GetCardSprite(ItemName iName)
    {
        print(m_CardBacks.Count);
        try
        {
            return m_CardBacks.Find(x=>x.itemName == iName).property.Picture;
        }
        catch (Exception e)
        {
            print(e.Message);
            return m_CardBacks.GetRandomObject().property.Picture;
        }
    }
}

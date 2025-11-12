
using System.Collections.Generic;
using UnityEngine;

public class GameCardsData : SceneBasedSingleton<GameCardsData>
{
    [SerializeField] private Transform m_DecksContainer;
    
    public List<CardData> GetDecksData()
    {
        List<CardsDeck> decks = new();
        List<CardData> data = new();
      
        int childCount = m_DecksContainer.childCount;
      
        for (int i = 0; i < childCount; i++)
        {
            CardsDeck cardDeck = m_DecksContainer.GetChild(i).GetComponent<CardsDeck>();
            foreach (var v in cardDeck.CardsData) // So the Card value added for straight exceptional
                                                  // evaluation dosen't ruin normal evaluation
            {
                if(v.value == Cardvalue.valueS_A)
                    v.value = Cardvalue.value_A;
            }
            decks.Add(cardDeck);
        }
        
        decks.ForEach(deck => data.AddRange(deck.CardsData));
        return data;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestClass : MonoBehaviour
{
    [SerializeField] private Hand hand1;
    [SerializeField] private Hand hand2;


    [ContextMenu("Evaluate Cards")]
    private void TestCards()
    {
        print(TieBreakerComponent.DeepEvaluate(hand1, hand2));

       // List<CardData> cards = new List<CardData>();
        //cards = hand1.CardData.ToList();

       // cards.Find(x => x.value == Cardvalue.valueS_A).value = Cardvalue.value_A;
       foreach (var c in hand1.CardData)
       {
           if (c.value != Cardvalue.valueS_A) continue;
           c.value = Cardvalue.value_A;
           break;
       }
        /*foreach (var c in hand1.CardData)
        {
            if (c.value != Cardvalue.valueS_A) continue;
            c.value = Cardvalue.value_A;
            break;
        }

        HandEvaluator.Evaluate(hand1.CardData, out HandTypes handTypes);

        List<CardData> cardList = hand1.CardData.ToList();

        foreach (CardData cd in cardList)
            print(cd.value);

        cardList.OrderByDescending(card => card.value);

        if (handTypes is HandTypes.Straight or HandTypes.StraightFlush)
        {
            var temp = cardList.Find(x => x.value == Cardvalue.value_A);
            if (temp != null)
            {
                cardList.Remove(temp);
                cardList.Add(temp);
                print("ASdsad");
            }
        }

        hand1.CardData = cardList.ToArray();*/
    }
}

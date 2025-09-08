using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardNames",menuName ="CombatBoxing/CardNames")]
public class CardNamesScriptable : ScriptableObject
{
    [Serializable]
    public class Card
    {
        public string name;
        public Sprite cardSprite;
    }
    public List<Card> cards;
}

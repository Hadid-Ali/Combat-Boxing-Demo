using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardNames",menuName ="CombatBoxing/CardNames")]
public class CardNamesScriptable : ScriptableObject
{
    [Serializable]
    public class Boxing_Card
    {
        public Boxer.AttackType attackType;
        public string id;
        public int priority;
        public string name;
        public float speed;
        public Sprite cardSprite;
        public int rewardAmount;
        public string[] animations;
        public float minAnimFloat;
        public float maxAnimFloat;
        public string blendIndex;
    }
    public List<Boxing_Card> cards;
}

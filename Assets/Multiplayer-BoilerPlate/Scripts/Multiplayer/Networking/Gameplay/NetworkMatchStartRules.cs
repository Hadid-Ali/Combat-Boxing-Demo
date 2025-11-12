using System;
using UnityEngine;

public class NetworkMatchStartRules : MonoBehaviour
{
    [SerializeField] private MatchRule[] rules;

    [Serializable]
    public class MatchRule
    {
        public int numBots;    
    }

    public  int GetBotCount(int playerCount)
    {
        return rules[playerCount - 1].numBots;
    }
    
    

}

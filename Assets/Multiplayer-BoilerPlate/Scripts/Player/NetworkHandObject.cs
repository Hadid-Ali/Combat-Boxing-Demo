using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NetworkHandObject
{
    public CardData[] PlayerHand;

    public NetworkHandObject( CardData[]  cardsData)
    {
        PlayerHand = cardsData;
    }

    public static string Serialize(NetworkHandObject networkHandObject)
    {
        string data = JsonUtility.ToJson(networkHandObject);
        
        return data;
    }

    public static NetworkHandObject DeSerialize(string dataString)
    {
        return JsonUtility.FromJson<NetworkHandObject>(dataString);
    }
}

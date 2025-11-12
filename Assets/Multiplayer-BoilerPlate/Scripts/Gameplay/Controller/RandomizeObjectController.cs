using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class RandomizeObjectController : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<GameObject> m_RandomObjects;
    [SerializeField] private PhotonView view;

    private const string RandomObjectKey = "RandomObjectIndex";

    private void Start()
    {
        if (PhotonNetwork.OfflineMode)
        {
            int randomIndex = Random.Range(0, m_RandomObjects.Count);
            OnMasterInitialized(randomIndex);
            return;
        }
            
        
        if (PhotonNetwork.IsMasterClient)
        {
            // Generate a random index and store it in room properties
            int randomIndex = Random.Range(0, m_RandomObjects.Count);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { RandomObjectKey, randomIndex } });

            // Initialize the object for the master client
            OnMasterInitialized(randomIndex);
        }
        else 
        {
            int randomIndexx = (int)PhotonNetwork.CurrentRoom.CustomProperties[RandomObjectKey];
            OnMasterInitialized(randomIndexx);
        }
        
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // Check if the RandomObjectIndex property was updated
        if (propertiesThatChanged.ContainsKey(RandomObjectKey))
        {
            int randomIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties[RandomObjectKey];
            OnMasterInitialized(randomIndex);
        }
    }

    private void OnMasterInitialized(int val)
    {
        // Deactivate all objects first
        foreach (var obj in m_RandomObjects)
        {
            obj.SetActive(false);
        }

        // Activate the selected object
        m_RandomObjects[val].SetActive(true);
        Debug.Log($"Random Object Activated: {val}");
    }
}


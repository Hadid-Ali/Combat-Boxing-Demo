using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private List<StorePageName> storeNames = new();
    [SerializeField] private List<StorePageSO> storePages = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        storeNames.Clear();
        
        foreach (var v in storePages)
            storeNames.Add(v.pageName);
    }
#endif    

    private void Start()
    {
        GameEvents.StoreEvents.OnStoreButtonClicked.Register(OnStoreButtonClicked);
        GameEvents.StoreEvents.OnStoreItemSelected.Register(OnItemSelected);
        SetCurrentData();
    }

    private void SetCurrentData()
    {
        GameData.RuntimeData.SELECTED_CARD_BACK = (ItemName)GameData.PersistentData.SelectedCard;
        GameData.RuntimeData.SELECTED_CHARACTER = (ItemName)GameData.PersistentData.SelectedCharacter;
    }
    
    private void OnDestroy()
    {
        GameEvents.StoreEvents.OnStoreButtonClicked.UnRegister(OnStoreButtonClicked);
        GameEvents.StoreEvents.OnStoreItemSelected.UnRegister(OnItemSelected);
    }

    private void OnItemSelected(ItemName obj)
    {
        if ((int)obj > 7)
        {
            GameData.PersistentData.SelectedCard = (int)obj;
            GameData.RuntimeData.SELECTED_CARD_BACK = obj;
        }
        else
        {
            GameData.PersistentData.SelectedCharacter = (int)obj;
            GameData.RuntimeData.SELECTED_CHARACTER = obj;
        }
    }

    public void InitializeStore()
    {
        GameEvents.StoreEvents.OnStoreInitialize.Raise(storeNames);
        OnStoreButtonClicked(storeNames[0]);
        GameEvents.StoreEvents.OnStoreButtonClicked.Raise(storeNames[0]); 
    }

    private void OnStoreButtonClicked(StorePageName obj)
    {
        GameEvents.StoreEvents.OnDisplayStorePage.
            Raise(storePages.Find(x=> x.pageName == obj));
    }
}

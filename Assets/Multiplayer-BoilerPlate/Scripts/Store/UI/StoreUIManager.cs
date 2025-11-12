using System;
using System.Collections.Generic;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class StoreUIManager : MonoBehaviour
{
    [SerializeField] private StoreButton storeButtonPrefab;
    [SerializeField] private StoreItemUI[] itemPrefab;

    [SerializeField] private SimpleScrollSnap _scrollSnap;

    [SerializeField] private Transform storePagesContainer; 

    [SerializeField] private List<StoreButton> pooledStoreButtons = new(); 
    [SerializeField] private List<StoreItemUI> pooledStoreItemUI = new();
    [SerializeField] private List<StoreItemUI> pooledStoreCharacterItemUI = new();
    
    [SerializeField] private RenderRegistry renderRegistry;
    private static RenderRegistry _renderRegistry;

    private StoreButton _currentSelectedStoreButton;
    private StoreItemUI _currentSelectedItemButton;
    

    
    private void Awake()
    {
         GameEvents.StoreEvents.OnDisplayStorePage.Register(OnDisplayStorePage);
         GameEvents.StoreEvents.OnStoreInitialize.Register(OnInitializeStore);
         
         GameEvents.StoreEvents.OnStoreItemSelected.Register(OnStoreItemSelected);

         _renderRegistry = renderRegistry;
    }
    private void OnDestroy()
    {
        GameEvents.StoreEvents.OnDisplayStorePage.UnRegister(OnDisplayStorePage);
        GameEvents.StoreEvents.OnStoreInitialize.UnRegister(OnInitializeStore);
        GameEvents.StoreEvents.OnStoreItemSelected.UnRegister(OnStoreItemSelected);
    }

    public static RenderTexture GetTexture(ItemName name)
    {
        return _renderRegistry.GetRenderTexture(name);
    }

    private void OnStoreItemSelected(ItemName obj)
    {
        StoreItemUI itemToSelect = pooledStoreItemUI.Find(item => item.itemName == obj);

        if (!itemToSelect)
        {
            itemToSelect = pooledStoreCharacterItemUI.Find(item => item.itemName == obj);
            print("Item not found");
        }
        else if(!itemToSelect)
        {
            print("Returning");   
            return;
        }
        print("Item found");
        
        if(_currentSelectedItemButton)
            _currentSelectedItemButton.OnItemUnSelected();
            
        _currentSelectedItemButton = itemToSelect;
        _currentSelectedItemButton.OnItemSelected();
    }

    private void OnInitializeStore(List<StorePageName> obj)
    {
        if (pooledStoreButtons.Count >= obj.Count)
        {
            SetUpStoreButtons(obj);
        }
        else
        {
            int numToAdd = obj.Count - pooledStoreButtons.Count;
            for (int i = 0; i < numToAdd; i++)
            {
                StoreButton button =   Instantiate(storeButtonPrefab, storePagesContainer);
                pooledStoreButtons.Add(button);
            }
            
            SetUpStoreButtons(obj);
        }
    }
    private void SetUpStoreButtons(List<StorePageName> obj)
    {
        
        for (int i = 0; i < pooledStoreButtons.Count; i++)
        {
            if (i >= obj.Count)
            {
                pooledStoreButtons[i].gameObject.SetActive(false);
                continue;
            }
            pooledStoreButtons[i].gameObject.SetActive(true);
            pooledStoreButtons[i].storeName = obj[i];
            pooledStoreButtons[i].SetTitle(obj[i].ToString());
        }
    }

    private void SetupStorePAge(StorePageSO objectSO)
    {
        var defaultImageSize = objectSO.pageName == StorePageName.Characters ? 
            GameData.MetaData.deafaultCharacterImageSize
            : GameData.MetaData.deafaultCardImageSize;
        
        List<GameObject> active = new();
        List<GameObject> Inactive = new();

        switch (objectSO.pageName)
        {
            case StorePageName.Characters:
                for (int i = 0; i < pooledStoreCharacterItemUI.Count; i++)
                {
                    if (i >= objectSO.items.Count)
                    {
                        pooledStoreCharacterItemUI[i].gameObject.SetActive(false);
                        continue;
                    }
                    pooledStoreCharacterItemUI[i].gameObject.SetActive(true);
                    pooledStoreCharacterItemUI[i].InitializeItem(objectSO.items[i].property); 
                }
                foreach (StoreItemUI item in pooledStoreCharacterItemUI)
                {
                    if(item.gameObject.activeInHierarchy)
                        active.Add(item.gameObject);
                    else
                        Inactive.Add(item.gameObject);
                }
                Inactive.AddRange(pooledStoreItemUI.Select(item => item.gameObject));
                
                OnStoreItemSelected(GameData.RuntimeData.SELECTED_CHARACTER);
                
                break;
            case StorePageName.CardBacks:
                for (int i = 0; i < pooledStoreItemUI.Count; i++)
                {
                    if (i >= objectSO.items.Count)
                    {
                        pooledStoreItemUI[i].gameObject.SetActive(false);
                        continue;
                    }
                    pooledStoreItemUI[i].gameObject.SetActive(true);
                    pooledStoreItemUI[i].InitializeItem(objectSO.items[i].property); 
                }
                foreach (StoreItemUI item in pooledStoreItemUI)
                {
                    if(item.gameObject.activeInHierarchy)
                        active.Add(item.gameObject);
                    else
                        Inactive.Add(item.gameObject);
                }

                Inactive.AddRange(pooledStoreCharacterItemUI.Select(item => item.gameObject));
                OnStoreItemSelected(GameData.RuntimeData.SELECTED_CARD_BACK);
                break;
        }
        _scrollSnap.RemoveItems(Inactive);
        _scrollSnap.AddItems(active, defaultImageSize);
        
    }
    private void OnDisplayStorePage(StorePageSO obj)
    {
        switch (obj.pageName)
        {
            case StorePageName.Characters:
                if (pooledStoreCharacterItemUI.Count >= obj.items.Count)
                {
                    SetupStorePAge(obj);
                }
                else
                {
                    int numToAdd = obj.items.Count - pooledStoreCharacterItemUI.Count;
                    for (int i = 0; i < numToAdd; i++)
                    {
                        StoreItemUI item =  Instantiate(itemPrefab[1]);
                        pooledStoreCharacterItemUI.Add(item);
                    }
            
                    SetupStorePAge(obj);
                }
                break;
            case StorePageName.CardBacks:
                if (pooledStoreItemUI.Count >= obj.items.Count)
                {
                    SetupStorePAge(obj);
                }
                else
                {
                    int numToAdd = obj.items.Count - pooledStoreItemUI.Count;
                    for (int i = 0; i < numToAdd; i++)
                    {
                        StoreItemUI item =  Instantiate(itemPrefab[0]);
                        pooledStoreItemUI.Add(item);
                    }
            
                    SetupStorePAge(obj);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

}

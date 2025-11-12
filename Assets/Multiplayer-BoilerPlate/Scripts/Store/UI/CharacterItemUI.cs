using UnityEngine;
using UnityEngine.UI;

public class CharacterItemUI : StoreItemUI
{
    [SerializeField] private Image characterImage;
    [SerializeField] private RawImage backgroundImage;

    private void Awake()
    {
        SubscribeAction(OnItemClick);
    }

    private void OnItemClick()
    {
        GameEvents.StoreEvents.OnStoreItemSelected.Raise(itemName);
    }

    public override void InitializeItem(ItemProperty parameter1)
    {
        characterImage.sprite = parameter1.Picture;
        itemName = parameter1.name;
        OnItemUnSelected();
    }

    public void InitializeAnimatedCharacter(ItemProperty parameter1)
    {
        
    }
}

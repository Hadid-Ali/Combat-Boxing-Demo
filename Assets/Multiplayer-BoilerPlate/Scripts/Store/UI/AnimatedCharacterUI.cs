using UnityEngine;
using UnityEngine.UI;

public class AnimatedCharacterUI : StoreItemUI
{
    [SerializeField] private RawImage backgroundImage;
    private void Awake()
    {
        m_Button.onClick.AddListener(OnItemClick);   
    }

    private void OnItemClick()
    {
        GameEvents.StoreEvents.OnStoreItemSelected.Raise(itemName);
    }

    public override void InitializeItem(ItemProperty parameter1)
    {
        backgroundImage.texture = StoreUIManager.GetTexture(parameter1.name);
        itemName = parameter1.name;
        OnItemUnSelected();
    }
}

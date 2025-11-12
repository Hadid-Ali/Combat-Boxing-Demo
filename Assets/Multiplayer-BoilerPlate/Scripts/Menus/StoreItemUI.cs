using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class StoreItemUI : ButtonWidgetWithStatus
{
    [SerializeField] private CanvasGroup group;

    public ItemName itemName;
    public virtual void OnItemSelected()
    {
      SetFocusedAndPressed(true);
    }
    
    public virtual void OnItemUnSelected()
    {
        SetFocusedAndPressed(false);
    }
    
    public abstract void InitializeItem(ItemProperty parameter1);
}

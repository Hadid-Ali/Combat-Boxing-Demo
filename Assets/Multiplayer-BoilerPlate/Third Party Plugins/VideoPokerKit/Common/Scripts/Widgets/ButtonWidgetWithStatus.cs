using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonWidgetWithStatus : ButtonWidget
{
    [SerializeField] private GameObject _statusWidgetObject;

    public void SetFocusedAndPressed(bool status)
    {
        _statusWidgetObject.SetActive(status);
        SetInteractable(!status);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettingsController : MonoBehaviour
{
    void Start()
    {
        SetFrameRate();
        SetScreenTime();
        LoadData();
    }

    private void LoadData()
    {
        GameData.RuntimeData.SELECTED_CHARACTER = (ItemName)GameData.PersistentData.SelectedCharacter;
        GameData.RuntimeData.SELECTED_CARD_BACK = (ItemName)GameData.PersistentData.SelectedCard;
    }

    public void SetFrameRate()
    {
        Application.targetFrameRate = 60;
    }

    public void SetScreenTime()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}

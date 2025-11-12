using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLoginScreen : UIMenuBase
{
    [SerializeField] private TMP_InputField m_InputField;

    [SerializeField] private Button m_LoginButton;
    
    private void Start()
    {
        m_InputField.onValueChanged.AddListener(OnFieldValueChange);
        m_LoginButton.onClick.AddListener(OnLoginBtnEvent);

        SetButtonInteractionStatus(false);
        
        OnFieldValueChange(string.Empty);
        m_InputField.characterLimit = GameData.MetaData.MaximumNameLength;
        
    }

    protected override void OnContainerEnable()
    {
        base.OnContainerEnable();
        CheckForPreviousLogin();
    }

    private void OnFieldValueChange(string value)
    {
        bool hasValidLenght = value.Length >= GameData.MetaData.MinimumNameLength;
        SetButtonInteractionStatus(!string.IsNullOrEmpty(value) && hasValidLenght);
    }

    private void CheckForPreviousLogin()
    {
        if (!string.IsNullOrEmpty(GameData.PersistentData.UserName))
        {
            LoginInternal();
        }
    }

    public void OnLoginBtnEvent()
    {
        string userName = m_InputField.text;

        GameData.PersistentData.UserName = userName;
        GameData.RuntimeData.IS_LOGGED_IN = true;
        LoginInternal();
    }

    private void LoginInternal()
    {
        GameData.RuntimeData.USER_NAME = GameData.PersistentData.UserName;
        GameData.RuntimeData.IS_LOGGED_IN = true;
        
        ChangeMenuState(MenuName.MainMenu);
    }

    private void SetButtonInteractionStatus(bool status)
    {
        m_LoginButton.interactable = status;
    }
}

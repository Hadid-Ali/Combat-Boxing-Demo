using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreMenu : UIMenuBase
{
  [SerializeField] private Button m_BackButton;

  private void Start()
  {
    m_BackButton.onClick.AddListener(OnBackButton);
  }

  private void OnBackButton()
  {
    ChangeMenuState(MenuName.MainMenu);
  }
}

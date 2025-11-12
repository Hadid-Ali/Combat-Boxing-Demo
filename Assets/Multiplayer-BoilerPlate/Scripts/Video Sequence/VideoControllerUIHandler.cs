using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VideoControllerUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_Container;
    [SerializeField] private VideoController m_VideoController;

    [SerializeField] private ButtonWidget m_SkipButton;
    
    public void Initialize()
    {
        m_SkipButton.SubscribeAction(Hide);
        m_VideoController.Initialize(Hide);
    }
    
    public void Show()
    {
        m_Container.SetActive(true);
    }
    
    public void Hide()
    {
        m_Container.SetActive(false);
    }

}

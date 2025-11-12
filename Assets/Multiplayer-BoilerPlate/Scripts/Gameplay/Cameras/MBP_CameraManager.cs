using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public struct CameraObject
{
    [SerializeField] private ViewMode m_CameraType;
    [SerializeField] private CinemachineCamera m_CameraComponent;

    public ViewMode CameraType => m_CameraType;

    public void SetEnabled(bool enable)
    {
        m_CameraComponent.enabled = enable;
    }
}

public class MBP_CameraManager : SceneBasedSingleton<MBP_CameraManager>
{
    [SerializeField] private CameraObject[] m_Cameras;

    private ViewMode m_CameraType;

    private void Start()
    {
        SetCameraEnabledAtIndex(0);
    }
    
    private void SetCameraEnabledAtIndex(int index)
    {
        if (index >= m_Cameras.Length)
            return;
        
        SetCameraEnabled(m_Cameras[index].CameraType);
    }

    public void SetCameraEnabled(ViewMode cameraType)
    {
        if (m_CameraType == cameraType)
            return;
        
        for (int i = 0; i < m_Cameras.Length; i++)
        {
            m_Cameras[i].SetEnabled(m_Cameras[i].CameraType == cameraType);
        }

        m_CameraType = cameraType;
    }
}

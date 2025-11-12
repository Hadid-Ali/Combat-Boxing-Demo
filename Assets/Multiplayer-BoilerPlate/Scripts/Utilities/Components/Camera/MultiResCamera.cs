using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MultiResCamera : MultiResObject
{
    [SerializeField] private List<CameraResolutionFOV> m_CameraFOVs;
    [SerializeField] private CinemachineCamera m_CameraComponent;
    
    protected override void OnDeviceModeInput(DeviceMode deviceMode)
    {
        SetCameraFov(m_CameraFOVs.Find(fovObject => fovObject.DeviceMode == deviceMode).FOV);
    }

    public void SetCameraFov(float value)
    {
        m_CameraComponent.Lens.FieldOfView = value;
    }
}

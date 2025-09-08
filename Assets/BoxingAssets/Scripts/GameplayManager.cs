using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;

    public delegate void ChangeCameraPosition(Transform pos);
    public static event ChangeCameraPosition onChangingCameraPosition;

    private void OnEnable()
    {
        onChangingCameraPosition += ChangeCamTransform;
    }

    private void OnDisable()
    {
        onChangingCameraPosition -= ChangeCamTransform;
    }

    public static void ChangingCameraTransform(Transform pos)
    {
        onChangingCameraPosition?.Invoke(pos);
    }
    void ChangeCamTransform(Transform pos)
    {
        mainCamera.transform.position = pos.position;
        mainCamera.transform.eulerAngles = pos.eulerAngles;
    }
}

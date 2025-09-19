using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    [SerializeField] Transform playerPos;
    [SerializeField] Transform opponentPos;
    [SerializeField] Transform mainPos;
    private Tween suckTween;
    [SerializeField] float cameraMoveSpeed;

    public delegate void SwitchCameraToPlayer();
    public static event SwitchCameraToPlayer onCameraSwitchingToPlayer;

    public delegate void SwitchCameraToOpponent();
    public static event SwitchCameraToOpponent onCameraSwitchingToOpponent;

    public delegate void SwitchCameraToFightingPosition();
    public static event SwitchCameraToFightingPosition onCameraSwitchingToFight;
    private void OnEnable()
    {
        onCameraSwitchingToPlayer += SwitchToPlayer;
        onCameraSwitchingToOpponent += SwitchToOpponent;
        onCameraSwitchingToFight += SwitchToFighting;
    }

    private void OnDisable()
    {
        onCameraSwitchingToPlayer -= SwitchToPlayer;
        onCameraSwitchingToOpponent -= SwitchToOpponent;
        onCameraSwitchingToFight -= SwitchToFighting;

    }

    public static void SwitchToPlayerPosition()
    {
        onCameraSwitchingToPlayer?.Invoke();
    }
    void SwitchToPlayer()
    {
        SwitchPosition(playerPos);
    }
    public static void SwitchToOpponentPosition()
    {
        onCameraSwitchingToOpponent?.Invoke();
    }
    void SwitchToOpponent()
    {
        SwitchPosition(opponentPos);
    }
    public static void SwitchToFightingPosition()
    {
        onCameraSwitchingToFight?.Invoke();
    }
    void SwitchToFighting()
    {
        SwitchPosition(mainPos);
    }
    void SwitchPosition(Transform pos)
    {
        CameraShifting(pos);
    }

    void CameraShifting(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target Point not assigned!");
            return;
        }

        if (suckTween != null && suckTween.IsActive())
            suckTween.Kill();

        Sequence seq = DOTween.Sequence();

        seq.Join(mainCamera.transform.DOMove(target.position, cameraMoveSpeed).SetEase(Ease.OutCirc));

        seq.Join(mainCamera.transform.DORotate(target.eulerAngles, cameraMoveSpeed).SetEase(Ease.OutCirc));

        suckTween = seq;

    }
}

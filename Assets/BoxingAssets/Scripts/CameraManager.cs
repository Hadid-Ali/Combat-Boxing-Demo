using DG.Tweening;
using Photon.Pun;
using UnityEngine;

public class CameraManager : MonoBehaviourPun
{
    [Header("Camera Settings")]
    [SerializeField] private GameObject mainCamera;

    [Header("Camera Positions")]
    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform opponentPos;
    [SerializeField] private Transform mainPos;

    [SerializeField] private float moveDuration = 1f;

    private Tween activeTween;
    private System.Action onCameraMoveComplete;

    /// <summary>
    /// Switches camera based on winner ID
    /// 1 = Player, 2 = Opponent, otherwise = Main
    /// </summary>
    public void SwitchByWinner(int winnerID, System.Action callback = null)
    {
        Transform target = winnerID switch
        {
            1 => playerPos,
            2 => opponentPos,
            _ => mainPos
        };

        onCameraMoveComplete = callback;

        photonView.RPC(nameof(RPC_SwitchCamera),
            RpcTarget.All,
            target.position,
            target.rotation.eulerAngles);
    }

    public void SwitchToPlayer(System.Action callback = null)
        => SwitchTo(playerPos, callback);

    public void SwitchToOpponent(System.Action callback = null)
        => SwitchTo(opponentPos, callback);

    public void SwitchToMain(System.Action callback = null)
        => SwitchTo(mainPos, callback);


    private void SwitchTo(Transform target, System.Action callback)
    {
        onCameraMoveComplete = callback;

        photonView.RPC(nameof(RPC_SwitchCamera),
            RpcTarget.All,
            target.position,
            target.rotation.eulerAngles);
    }


    [PunRPC]
    private void RPC_SwitchCamera(Vector3 pos, Vector3 rot)
    {
        KillActiveTween();

        Sequence seq = DOTween.Sequence();

        seq.Join(mainCamera.transform.DOMove(pos, moveDuration).SetEase(Ease.OutCirc));
        seq.Join(mainCamera.transform.DORotate(rot, moveDuration).SetEase(Ease.OutCirc));

        activeTween = seq;

        seq.OnComplete(() =>
        {
            onCameraMoveComplete?.Invoke();
            onCameraMoveComplete = null;
        });
    }

    private void KillActiveTween()
    {
        if (activeTween != null && activeTween.IsActive())
            activeTween.Kill();
    }
}

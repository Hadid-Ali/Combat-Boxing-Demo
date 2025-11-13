using UnityEngine;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

public class AnimationRPC : MonoBehaviour
{
    private Animator animator;

#if PHOTON_UNITY_NETWORKING
    private PhotonView photonView;
#endif

    private void Awake()
    {
        animator = GetComponent<Animator>();

#if PHOTON_UNITY_NETWORKING
        photonView = GetComponent<PhotonView>();
#endif
    }

    // ========== LOCAL METHODS ==========

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }

    public void SetBool(string paramName, bool value)
    {
        animator.SetBool(paramName, value);
    }

    public void SetFloat(string paramName, float value)
    {
        animator.SetFloat(paramName, value);
    }

    public void SetInt(string paramName, int value)
    {
        animator.SetInteger(paramName, value);
    }

    // ========== MULTIPLAYER RPC WRAPPERS ==========

#if PHOTON_UNITY_NETWORKING

    public void RPC_PlayAnimation(string animationName)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_PlayAnimationInternal), RpcTarget.All, animationName);
    }

    [PunRPC]
    private void RPC_PlayAnimationInternal(string animationName)
    {
        PlayAnimation(animationName);
    }

    public void RPC_SetTrigger(string animationName)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetTriggerInternal), RpcTarget.All, animationName);
    }

    [PunRPC]
    private void RPC_SetTriggerInternal(string animationName)
    {
        SetTrigger(animationName);
    }

    public void RPC_ResetTrigger(string animationName)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_ResetTriggerInternal), RpcTarget.All, animationName);
    }

    [PunRPC]
    private void RPC_ResetTriggerInternal(string animationName)
    {
        ResetTrigger(animationName);
    }

    public void RPC_SetBool(string animationName, bool value)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetBoolInternal), RpcTarget.All, animationName, value);
    }

    [PunRPC]
    private void RPC_SetBoolInternal(string animationName, bool value)
    {
        SetBool(animationName, value);
    }

    public void RPC_SetFloat(string animationName, float value)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetFloatInternal), RpcTarget.All, animationName, value);
    }

    [PunRPC]
    private void RPC_SetFloatInternal(string animationName, float value)
    {
        SetFloat(animationName, value);
    }

    public void RPC_SetInt(string animationName, int value)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetIntInternal), RpcTarget.All, animationName, value);
    }

    [PunRPC]
    private void RPC_SetIntInternal(string animationName, int value)
    {
        SetFloat(animationName, value);
    }

#endif
}

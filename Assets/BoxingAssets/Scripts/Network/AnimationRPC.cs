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
        Debug.Log("<color=green>Setting Float Parameter:</color> " + paramName + " to " + value);
        animator.SetFloat(paramName, value);
    }

    public void SetInt(string paramName, int value)
    {
        animator.SetInteger(paramName, value);
    }


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

    public void RPC_SetTrigger(string triggerName)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetTriggerInternal), RpcTarget.All, triggerName);
    }

    [PunRPC]
    private void RPC_SetTriggerInternal(string triggerName)
    {
        SetTrigger(triggerName);
    }

    public void RPC_ResetTrigger(string triggerName)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_ResetTriggerInternal), RpcTarget.All, triggerName);
    }

    [PunRPC]
    private void RPC_ResetTriggerInternal(string triggerName)
    {
        ResetTrigger(triggerName);
    }

    public void RPC_SetBool(string paramName, bool value)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetBoolInternal), RpcTarget.All, paramName, value);
    }

    [PunRPC]
    private void RPC_SetBoolInternal(string paramName, bool value)
    {
        SetBool(paramName, value);
    }

    public void RPC_SetFloat(string paramName, float value)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetFloatInternal), RpcTarget.All, paramName, value);
    }

    [PunRPC]
    private void RPC_SetFloatInternal(string paramName, float value)
    {
        SetFloat(paramName, value);
    }

    public void RPC_SetInt(string paramName, int value)
    {
        if (photonView != null)
            photonView.RPC(nameof(RPC_SetIntInternal), RpcTarget.All, paramName, value);
    }

    [PunRPC]
    private void RPC_SetIntInternal(string paramName, int value)
    {
        SetInt(paramName, value);
    }

#endif
}

using DG.Tweening;
using Photon.Pun;
using UnityEngine;


public class AnimationHandler : MonoBehaviour
{
    [SerializeField] Transform originalPos;
    Tween moveTween;
    [SerializeField] float speed;
    [SerializeField] string boxerName;
    [SerializeField] Animator animator;

    [Header("IK Targets")]
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    [Range(0, 1)] public float ikWeight = 1.0f;


    public void OnAttackAnimationComplete()
    {
        Debug.Log("Attack complete, resetting round.");

        if(PhotonNetwork.IsMasterClient)
            GameplayManager.instance.ResetRoundInternal();
    }

    private void CallPlayerAttack()
    {
        CancelInvoke(nameof(CallPlayerAttack));
    }

    private void ResetToOriginalPosition()
    {
        this.GetComponent<Animator>().SetBool("IsDefending", false);

        moveTween?.Kill();

        moveTween = this.transform.DOMove(originalPos.position, speed).SetEase(Ease.Linear).OnComplete(() =>
        {

        });
    }

    private void OnDamage(string reaction)
    {
        if (boxerName.Equals("Player"))
        {
            //OpponentAI.GetRandomDefence(reaction);
        }
        else if (boxerName.Equals("Ai"))
        {
            Debug.LogError("body hit ");
            
        }
    }

    private void EffectOnRightHand(string attack)
    {
        if (boxerName.Equals("Player"))
        {
            Player.PlayRightHandEffect();
            //if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
                //OpponentAI.PlaySweatEffect();
        }
        else if (boxerName.Equals("Ai"))
        {
            //OpponentAI.PlayRightHandEffect();
            if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
                Player.PlaySweatEffect();
        }       
    }


    private void EffectOnLeftHand(string attack)
    {
        if (boxerName.Equals("Player"))
        {
            Player.PlayLeftHandEffect();
            //if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
                //OpponentAI.PlaySweatEffect();
        }
        else if (boxerName.Equals("Ai"))
        {
            //OpponentAI.PlayLeftHandEffect();
            if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
                Player.PlaySweatEffect();
        }      
    }

    private void OnKnockedOut()
    {
        if (boxerName.Equals("Player"))
        {
            //OpponentAI.Knockedout();
        }
        else if (boxerName.Equals("Ai"))
        {
            Time.timeScale = 0.2f;
            Player.Knockedout();
        }
    }

    private void GroundHit()
    {
        if (boxerName.Equals("Player"))
        {
            Player.EnableGroundHitEffect();
        }
        else if (boxerName.Equals("Ai"))
        {
        }
    }

    private void ResetOpponentAnimation()
    {
        //OpponentAI.OnResetAttackState();
        CancelInvoke(nameof(ResetOpponentAnimation));
    }

    private void ResetPlayerAnimation()
    {
        Player.OnResetAttackState();
        CancelInvoke(nameof(ResetPlayerAnimation));
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (rightHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }

        if (leftHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
    }
}

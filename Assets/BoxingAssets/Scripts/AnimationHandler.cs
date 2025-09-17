using DG.Tweening;
using UnityEngine;
using static Boxer;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] Transform originalPos;
    Tween moveTween;
    [SerializeField] float speed;
    [SerializeField] string boxerName;
    [SerializeField]Animator animator;
    [Header("IK Targets")]
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    [Range(0, 1)] public float ikWeight = 1.0f;

    public void OnAttackAnimationComplete()
    {
        ResetToOriginalPosition();
        if (GameplayManager.FirstAttacker() != "")
        {
            if (GameplayManager.FirstAttacker().ToLower().Equals(BoxerType.player.ToString().ToLower()))
                Invoke("CallOpponentAttack", 0.55f);
            else
                Invoke("CallPlayerAttack", 0.55f);

            GameplayManager.ResetFirstAttackerValue("");
        }
        
        //Debug.LogError("on animation complete");
    }

    void CallOpponentAttack()
    {
        Player.OnResetAttackState();
        OpponentAI.OnAttackAction(CardsManager.OnOpponentSelectedAttack());
        CancelInvoke("CallOpponentAttack");
    }

    void CallPlayerAttack()
    {
        OpponentAI.OnResetAttackState();
        Player.OnAttackAction(CardsManager.OnSelectedAttack());
        CancelInvoke("CallPlayerAttack");
    }
    void ResetToOriginalPosition()
    {
        this.GetComponent<Animator>().SetBool("IsDefending", false);

        moveTween?.Kill();

        moveTween = this.transform.DOMove(originalPos.position, speed).SetEase(Ease.Linear).OnComplete(() =>
        {
        });

        //Debug.LogError("player reset");
    }

    void OnDamage()
    {
        if (boxerName.Equals("Player"))
        {
            OpponentAI.GetRandomDefence();
            Invoke("ResetOpponentAnimation", 1.5f);
        }
        else if (boxerName.Equals("Ai"))
        {
            Player.GetRandomDefence();
            Invoke("ResetPlayerAnimation", 1.5f);
        }
    }

    private void ResetOpponentAnimation()
    {
        OpponentAI.OnResetAttackState();
        CancelInvoke("ResetOpponentAnimation");
    }
    private void ResetPlayerAnimation()
    {
        Player.OnResetAttackState();
        CancelInvoke("ResetPlayerAnimation");
    }

    void OnAnimatorIK(int layerIndex)
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

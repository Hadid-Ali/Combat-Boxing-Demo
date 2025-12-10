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

    [Header("Opponent Hit Points")]
    [SerializeField] Transform opponentFaceTarget;
    [SerializeField] Transform opponentBodyTarget;

    [Range(0, 1)] public float ikWeight = 1.0f;

    private void Start()
    {
        SetupOpponentTargets();
    }

    private void SetupOpponentTargets()
    {
        Player opponent = GetOpponent();
        if (opponent != null)
        {
            opponentFaceTarget = opponent.faceHB/* transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head")*/;

            opponentBodyTarget = opponent.bodyHB /*transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2")*/;

            //rightHandTarget = opponentFaceTarget;
            //leftHandTarget = opponentFaceTarget;
        }
    }
    /*private void Start()
    {
        //Invoke(nameof(SetupOpponentTargets), 0.5f);
    }

    private void SetupOpponentTargets()
    {
        Player opponent = GetOpponent();
        if (opponent != null)
        {
            opponentFaceTarget = opponent.GetFaceHitBox();
            opponentBodyTarget = opponent.GetBodyHitBox();

            if (opponentFaceTarget != null && opponentBodyTarget != null)
            {
                rightHandTarget = opponentFaceTarget;
                leftHandTarget = opponentFaceTarget;
                Debug.Log($"Successfully set up IK targets for {gameObject.name}");
            }
            else
            {
                Debug.LogError($"Failed to get hit box transforms from opponent on {gameObject.name}");
            }
        }
        else
        {
            Debug.LogError($"Could not find opponent for {gameObject.name}. Retrying...");
            Invoke(nameof(SetupOpponentTargets), 0.5f);
        }
    }*/

    private Player GetOpponent()
    {
        if (!TryGetComponent<PhotonView>(out var myPhotonView))
            return null;

        int myActorNumber = myPhotonView.Owner.ActorNumber;

        foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (player.GetPlayerID() != myActorNumber)
                return player;
        }

        return null;
    }
    public void OnAttackAnimationComplete()
    {
        Debug.Log("Attack complete, resetting round.");

        if (PhotonNetwork.IsMasterClient)
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
            int opponentID = GetOpponentPlayerID();
            if (opponentID != -1)
            {
                Player.TriggerOpponentDefence(opponentID, reaction);
            }
        }
        else if (boxerName.Equals("Ai"))
        {
            Debug.LogError("body hit ");

        }
    }

    private void EffectOnRightHand(string attack)
    {
        SoundManager.Instance.PlayPunchSound();
        if (boxerName.Equals("Player"))
        {
            Debug.Log(">>> 1st | right | one");
            Player.PlayRightHandEffect();

            int opponentID = GetOpponentPlayerID();
            if (opponentID != -1)
            {
                Player.TriggerOpponentSweat(opponentID, attack);
                //Player.TriggerOpponentDefence(opponentID, attack);

            }
            //if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
            //OpponentAI.PlaySweatEffect();
        }
        else if (boxerName.Equals("Ai"))
        {
            Debug.Log(">>> 1st | right | two");
            //OpponentAI.PlayRightHandEffect();
            if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
                Player.PlaySweatEffect();
        }
    }

    private void EffectOnLeftHand(string attack)
    {
        SoundManager.Instance.PlayPunchSound();
        if (boxerName.Equals("Player"))
        {
            Debug.Log(">>> 1st | left | one");
            Player.PlayLeftHandEffect();

            int opponentID = GetOpponentPlayerID();
            if (opponentID != -1)
            {
                Player.TriggerOpponentSweat(opponentID, attack);
                //Player.TriggerOpponentDefence(opponentID, attack);

            }
            //if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
            //OpponentAI.PlaySweatEffect();
        }
        else if (boxerName.Equals("Ai"))
        {
            Debug.Log(">>> 1st | left | two");
            //OpponentAI.PlayLeftHandEffect();
            if (attack.Equals("FacePunch") || attack.Equals("UppercutPunch"))
                Player.PlaySweatEffect();
        }
    }
    private int GetOpponentPlayerID()
    {
        if (!TryGetComponent<PhotonView>(out var myPhotonView)) return -1;

        int myActorNumber = myPhotonView.Owner.ActorNumber;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != myActorNumber)
            {
                return player.ActorNumber;
            }
        }

        return -1;
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
        //rightHandTarget = opponentFaceTarget;
        //leftHandTarget = opponentFaceTarget;

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

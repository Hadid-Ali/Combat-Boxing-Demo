using DG.Tweening;
using SmoothShakeFree;
using System.Collections.Generic;
using UnityEngine;
using static CardNamesScriptable;

public class Player : Boxer
{
    public static float playerAttackPriority => (CardsManager.GetPriorityValueForAttack(CardsManager.OnSelectedAttack()));

    public delegate void Attack(AttackType _type);
    public static event Attack onAttack;

    public delegate void PointsEarned(int points);
    public static event PointsEarned onEarnedPoints;

    [SerializeField] Transform boxer;
    [SerializeField] Transform targetToMove;

    public delegate void FetchRandomDefence(string reaction);
    public static event FetchRandomDefence onRandomDefence;
    Tween playerTween;

    public delegate AttackType CurrentAttackState();
    public static event CurrentAttackState onAttackState;

    public delegate void ReSetAttack();
    public static event ReSetAttack onAttackResetState;

    GameObject rightHandEffect;
    GameObject leftHandEffect;


    public delegate void ActivateRightHandEffect();
    public static event ActivateRightHandEffect onRightHandEffectActivation;
    public delegate void ActivateLeftHandEffect();
    public static event ActivateLeftHandEffect onLeftHandEffectActivation;

    public delegate void ActivateSweatEffect();
    public static event ActivateSweatEffect onSweatEffectPlay;
    private void Start()
    {
        foreach (GameObject effect in effectsPrefab)
        {
            GameObject right_g = Instantiate(effect, rightHandEffectParent, false);
            rightHandEffect = right_g;
            right_g.transform.SetParent(rightHandEffectParent);
            right_g.transform.localPosition = Vector3.zero;
            right_g.transform.localEulerAngles = Vector3.zero;

            GameObject left_g = Instantiate(effect, leftHandEffectParent, false);
            leftHandEffect = left_g;
            left_g.transform.SetParent(leftHandEffectParent);
            left_g.transform.localPosition = Vector3.zero;
            left_g.transform.localEulerAngles = Vector3.zero;
        }
    }
    private void OnEnable()
    {
        onAttack += AttackAction;
        onEarnedPoints += UpdatePoints;
        onRandomDefence += RandomDefense;
        onAttackState += GetAttackState;
        onAttackResetState += ResetAttackState;
        onRightHandEffectActivation += RightHandEffect;
        onLeftHandEffectActivation += LeftHandEffect;
        onSweatEffectPlay += SweatEffect;

    }
    private void OnDisable()
    {
        onAttack -= AttackAction;
        onEarnedPoints -= UpdatePoints;
        onRandomDefence -= RandomDefense;
        onAttackState -= GetAttackState;
        onAttackResetState -= ResetAttackState;
        onRightHandEffectActivation -= RightHandEffect;
        onLeftHandEffectActivation -= LeftHandEffect;
        onSweatEffectPlay -= SweatEffect;
    }
    #region Events Invoke

    public static void OnAttackAction(AttackType _type)
    {
        onAttack?.Invoke(_type);
    }
    public static void OnEarnedPoints(int _points)
    {
        onEarnedPoints?.Invoke(_points);
    }
    #endregion

    #region Functions
    protected void AttackAction(AttackType _attack)
    {
        playerTween?.Kill();
        playerTween = boxer.DOMove(targetToMove.position, moveSpeed).SetEase(Ease.Linear).OnComplete(() =>
                      {
                          TriggerAttackAnimation(_attack);
                      });
    }
    void TriggerAttackAnimation(AttackType _attack)
    {
        
        Card _card = null;
        foreach (Card c in cardType.cards)
        {
            if (c.name.ToLower().Equals(_attack.ToString()))
            {
                _card = c;
                break;
            }
        }
        float val = 0;
        animator.SetFloat("HitBlendIndex", 0);
        animator.ResetTrigger("hit");
        animator.SetBool("IsDefending", false);
        switch (_attack)
        {
            case AttackType.idle:
                attackType = AttackType.idle;
                break;
            case AttackType.powerpunch:
                //Debug.Log("-----POWER PUNCH CALLED-----");
                UpdatePoints(_card.rewardAmount);
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                animator.SetFloat(_card.blendIndex, 1);
                CallAnimation(AttackType.powerpunch);
                attackType = AttackType.powerpunch;
                break;
            case AttackType.combo:
                //Debug.Log("-----COMBO CALLED-----");
                UpdatePoints(_card.rewardAmount);
                //val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = 1;
                animator.SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.combo);
                attackType = AttackType.combo;

                break;
            case AttackType.uppercut:
                //Debug.Log("-----UPPER CUT CALLED-----");
                UpdatePoints(_card.rewardAmount);
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                animator.SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.uppercut);
                attackType = AttackType.uppercut;

                break;
            case AttackType.overhand:
                //Debug.Log("-----OVERHAND CALLED-----");
                UpdatePoints(_card.rewardAmount);
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                animator.SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.overhand);
                attackType = AttackType.overhand;

                break;
            case AttackType.hook:
                //Debug.Log("-----HOOK CALLED-----");
                UpdatePoints(_card.rewardAmount);
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                animator.SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.hook);
                attackType = AttackType.hook;

                break;
            case AttackType.body:
                //Debug.Log("-----BODY CALLED-----");
                UpdatePoints(_card.rewardAmount);
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                animator.SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.body);
                attackType = AttackType.body;

                break;
            case AttackType.jab:
                //Debug.Log("-----JAB CALLED-----");
                UpdatePoints(_card.rewardAmount);
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                animator.SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.jab);
                attackType = AttackType.jab;

                break;
        }

    }
    void UpdatePoints(int p)
    {
        pointsEarned += p;
        GameHUD.OnUpdatingPoints(BoxerType.player, p);
    }
    void CallAnimation(AttackType anim)
    {
        animator.ResetTrigger(anim.ToString().ToLower());
        animator.SetTrigger(anim.ToString().ToLower());
        //Debug.LogError("animation " + anim.ToString().ToLower());
    }
    public static void GetRandomDefence(string reaction)
    {
        onRandomDefence?.Invoke(reaction);
    }
    float valHit = 0;
    float valLastHit = 0;
    [SerializeField] float speed = 0;


    private Tween speedTween;
    public void SetBlendSpeed(float targetSpeed, float duration)
    {
        float current = animator.GetFloat("HitBlendIndex");
        if (Mathf.Approximately(current, targetSpeed) || Mathf.Approximately(valLastHit, targetSpeed))
            return;

        valLastHit = targetSpeed;

        if (speedTween != null && speedTween.IsActive())
            speedTween.Kill();

        speedTween = DOTween.To(() => animator.GetFloat("HitBlendIndex"), x =>
        {
            animator.SetFloat("HitBlendIndex", x);
        }, targetSpeed, duration).SetEase(Ease.Linear);
    }

    void RandomDefense(string hitReaction)
    {
        //int rand = Random.Range(0, 2);
        int rand = 0;
        float val = 0;
        
        if (rand == 0)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("hit"))
            {
                animator.ResetTrigger("hit");
                animator.SetTrigger("hit");
            }
            switch (hitReaction)
            {
                case "FacePunch":
                    valHit = 0.25f;
                    SetBlendSpeed(valHit, speed);
                    break;
                case "UppercutPunch":
                    valHit = 0.5f;
                    SetBlendSpeed(valHit, speed);
                    break;
                case "BodyHit":
                    valHit = 0.75f;
                    SetBlendSpeed(valHit, speed);
                    break;
                case "BodyUppercut":
                    valHit = 1.0f;
                    SetBlendSpeed(valHit, speed);
                    break;
            }
          
            Debug.Log($"{gameObject.name} took a hit!");
           
        }
        else
        {
            val = Random.Range(0.25f, 0.65f);
            animator.SetFloat("DefenceBlendINdex", val);
            animator.SetBool("IsDefending", true);
            Debug.Log($"{gameObject.name} guarded!");
        }
       
    }

    public static AttackType GetAttackState()
    {
        return onAttackState.Invoke();
    }
    AttackType GetCurrentAttackState()
    {
        return attackType;
    }

    public static void OnResetAttackState()
    {
         onAttackResetState.Invoke();
    }
    void ResetAttackState()
    {
        attackType = AttackType.idle;
        animator.SetFloat("HitBlendIndex", 0);
        animator.ResetTrigger("hit");
        animator.SetFloat("DefenceBlendINdex", 0);
        animator.SetBool("IsDefending", false);
    }
    public static void PlayLeftHandEffect()
    {
        onLeftHandEffectActivation?.Invoke();
    }
    public static void PlayRightHandEffect()
    {
        onRightHandEffectActivation?.Invoke();
    }
    void RightHandEffect()
    {
        rightHandEffect.GetComponent<ParticleSystem>().Play();
        StartShaking();
        Invoke("StopShaking", 0.07f);
    }

    void LeftHandEffect()
    {
        leftHandEffect.GetComponent<ParticleSystem>().Play();
        StartShaking();
        Invoke("StopShaking", 0.07f);
    }
    void StopShaking()
    {
        mainCamera.GetComponent<SmoothShake>().ForceStop();
    }
    void StartShaking()
    {
        mainCamera.GetComponent<SmoothShake>().StartShake();
    }
    public static void PlaySweatEffect()
    {
        onSweatEffectPlay?.Invoke();
    }
    void SweatEffect()
    {
        sweatEffect.GetComponent<ParticleSystem>().Play();
        bloodEffect.GetComponent<ParticleSystem>().Play();
    }
    #endregion
}

using DG.Tweening;
using ExitGames.Client.Photon;
using NaughtyAttributes.Test;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using static CardNamesScriptable;

public class Player : Boxer
{
    public static float playerAttackPriority => (CardsManager.GetPriorityValueForAttack(CardsManager.OnSelectedAttack()));

    public delegate void Attack(AttackType _type, int winnerID);
    public static event Attack onAttack;

    [SerializeField] Transform boxer;
    [SerializeField] Transform targetToMove;

    public delegate void FetchRandomDefence(string reaction);
    public static event FetchRandomDefence onRandomDefence;

    public delegate void PlayerKnockedOut();
    public static event PlayerKnockedOut onKnockedout;
    Tween playerTween;

    public delegate AttackType CurrentAttackState();
    public static event CurrentAttackState onAttackState;

    public delegate void ReSetAttack();
    public static event ReSetAttack onAttackResetState;
    [SerializeField] List<GameObject> rightHandEffect;
    [SerializeField] List<GameObject> leftHandEffect;

    public delegate void ActivateRightHandEffect();
    public static event ActivateRightHandEffect onRightHandEffectActivation;
    public delegate void ActivateLeftHandEffect();
    public static event ActivateLeftHandEffect onLeftHandEffectActivation;

    public delegate void ActivateSweatEffect();
    public static event ActivateSweatEffect onSweatEffectPlay;

    public delegate void GroundHitEffectActivation();
    public static event GroundHitEffectActivation onGroundHit;

    [SerializeField] private AnimationRPC _animationRPC;
    [SerializeField] private int playerID;

    private PhotonView _PhotonView => GetComponent<PhotonView>();

    private void Start()
    {
        _animationRPC = GetComponent<AnimationRPC>();
        SetPlayerID(_PhotonView.Owner.ActorNumber);
        GameplayManager.instance.RegisterPlayer(this);

        foreach (GameObject effect in effectsPrefab)
        {
            HitEffects.InstantiateHitEffects(effect, rightHandEffectParent, rightHandEffect);
            HitEffects.InstantiateHitEffects(effect, leftHandEffectParent, leftHandEffect);
        }
    }

    private void OnEnable()
    {
        onAttack += AttackAction;
        onRandomDefence += RandomDefense;
        onAttackState += GetAttackState;
        onAttackResetState += ResetAttackState;
        onRightHandEffectActivation += RightHandEffect;
        onLeftHandEffectActivation += LeftHandEffect;
        onSweatEffectPlay += SweatEffect;
        onKnockedout += KnockOut;
        onGroundHit += GroundHitEffect;
    }

    private void OnDisable()
    {
        onAttack -= AttackAction;
        onRandomDefence -= RandomDefense;
        onAttackState -= GetAttackState;
        onAttackResetState -= ResetAttackState;
        onRightHandEffectActivation -= RightHandEffect;
        onLeftHandEffectActivation -= LeftHandEffect;
        onSweatEffectPlay -= SweatEffect;
        onKnockedout -= KnockOut;
        onGroundHit += GroundHitEffect;

    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    #region Events Invoke

    public static void OnAttackAction(AttackType _type, int winnerID)
    {
        onAttack?.Invoke(_type, winnerID);
    }

    #endregion


    #region Functions
    protected void AttackAction(AttackType _attack, int winnerID)
    {
        Debug.Log("In attack action method");

        if (winnerID != _PhotonView.Owner.ActorNumber)
        {
            Debug.Log("Not the winner, skipping attack action for player: " + this.gameObject.name);
            return;
        }

        Debug.Log("Player Attack Action: " + _attack.ToString() + this.gameObject.name);

        if (targetToMove == null)
        {
            TriggerAttackAnimation(_attack);
        }
        else
        {
            playerTween?.Kill();
            playerTween = boxer.DOMove(targetToMove.position, moveSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                TriggerAttackAnimation(_attack);
            });
        }
    }

    private void TriggerAttackAnimation(AttackType _attack)
    {
        Boxing_Card _card = null;
        foreach (Boxing_Card c in cardType.cards)
        {
            if (c.name.ToLower().Equals(_attack.ToString()))
            {
                _card = c;
                break;
            }
        }

        Debug.Log("0n Attack Animation Triggered: " + _attack.ToString() + this.gameObject.name);

        float val = 0;
        _animationRPC.RPC_SetFloat("DefenceBlendINdex", 0);
        _animationRPC.RPC_ResetTrigger("hit");
        _animationRPC.RPC_SetBool("IsDefending", false);

        switch (_attack)
        {
            case AttackType.idle:
                attackType = AttackType.idle;
                break;
            case AttackType.powerpunch:
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = _card.maxAnimFloat;
                _animationRPC.RPC_SetFloat(_card.blendIndex, 1);
                CallAnimation(AttackType.powerpunch);
                attackType = AttackType.powerpunch;
                break;
            case AttackType.combo:
                val = 1;
                _animationRPC.RPC_SetFloat(_card.blendIndex, val);
                val = _card.maxAnimFloat;
                CallAnimation(AttackType.combo);
                attackType = AttackType.combo;

                break;
            case AttackType.uppercut:
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = _card.maxAnimFloat;
                _animationRPC.RPC_SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.uppercut);
                attackType = AttackType.uppercut;
                break;

            case AttackType.overhand:
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = _card.maxAnimFloat;
                _animationRPC.RPC_SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.overhand);
                attackType = AttackType.overhand;
                break;

            case AttackType.hook:
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = _card.maxAnimFloat;
                _animationRPC.RPC_SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.hook);
                attackType = AttackType.hook;
                break;

            case AttackType.body:
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = _card.maxAnimFloat;
                _animationRPC.RPC_SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.body);
                attackType = AttackType.body;
                break;

            case AttackType.jab:
                val = Random.Range(_card.minAnimFloat, _card.maxAnimFloat);
                val = _card.maxAnimFloat;
                _animationRPC.RPC_SetFloat(_card.blendIndex, val);
                CallAnimation(AttackType.jab);
                attackType = AttackType.jab;
                break;
        }
    }

    void CallAnimation(AttackType anim)
    {
        animator.ResetTrigger(anim.ToString().ToLower());
        animator.SetTrigger(anim.ToString().ToLower());
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
            _animationRPC.RPC_SetFloat("HitBlendIndex", x);

        }, targetSpeed, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            valLastHit = -1f;
        });
    }

    void RandomDefense(string hitReaction)
    {
        int rand = 0;
        float val = 0;

        if (rand == 0)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("hit"))
            {
                _animationRPC.RPC_ResetTrigger("hit");
                _animationRPC.RPC_SetTrigger("hit");
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
        }
        else
        {
            val = Random.Range(0.25f, 0.65f);
            _animationRPC.RPC_SetFloat("DefenceBlendINdex", val);
            _animationRPC.RPC_SetBool("IsDefending", true);
        }
    }


    public static void Knockedout()
    {
        onKnockedout?.Invoke();
    }

    private void KnockOut()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("knockout"))
        {
            _animationRPC.RPC_ResetTrigger("knockout");
            _animationRPC.RPC_SetTrigger("knockout");
        }

        valHit = 1.0f;
        _animationRPC.RPC_SetFloat("KnockoutBlendIndex", valHit);
        Invoke(nameof(EnableKnockOutCamer), 0.06f);
    }

    private void EnableKnockOutCamer()
    {
        knockOutCamera.SetActive(true);
        CancelInvoke(nameof(EnableKnockOutCamer));
    }

    public static void EnableGroundHitEffect()
    {
        onGroundHit?.Invoke();
    }

    private void GroundHitEffect()
    {
        Time.timeScale = 1.0f;

        groundHitEffect.SetActive(true);
        groundHitEffect.GetComponent<ParticleSystem>().Play();
    }

    public static AttackType GetAttackState()
    {
        return onAttackState.Invoke();
    }

    private AttackType GetCurrentAttackState()
    {
        return attackType;
    }

    public static void OnResetAttackState()
    {
        onAttackResetState.Invoke();
    }

    private void ResetAttackState()
    {
        attackType = AttackType.idle;
        _animationRPC.RPC_SetFloat("KnockoutBlendIndex", 0);
        _animationRPC.RPC_ResetTrigger("hit");
        _animationRPC.RPC_SetFloat("DefenceBlendINdex", 0);
        _animationRPC.RPC_SetBool("IsDefending", false);
    }

    public static void PlayLeftHandEffect()
    {
        onLeftHandEffectActivation?.Invoke();
    }

    public static void PlayRightHandEffect()
    {
        onRightHandEffectActivation?.Invoke();
    }

    private void RightHandEffect()
    {
        foreach (GameObject g in rightHandEffect)
        {
            g.SetActive(true);
            g.GetComponent<ParticleSystem>().Play();
        }
        StartShaking();
        Invoke(nameof(StopShaking), 0.07f);
    }

    private void LeftHandEffect()
    {
        foreach (GameObject g in leftHandEffect)
        {
            g.SetActive(true);
            g.GetComponent<ParticleSystem>().Play();
        }
        StartShaking();
        Invoke(nameof(StopShaking), 0.07f);
    }

    private void StopShaking()
    {
        //mainCamera.GetComponent<SmoothShake>().ForceStop();
    }

    private void StartShaking()
    {
        //mainCamera.GetComponent<SmoothShake>().StartShake();
    }

    public static void PlaySweatEffect()
    {
        onSweatEffectPlay?.Invoke();
    }

    private void SweatEffect()
    {
        Debug.Log(">>> 2nd");
        sweatEffect.gameObject.SetActive(true);
        sweatEffect.GetComponent<ParticleSystem>().Play();
        bloodEffect.SetActive(true);
        bloodEffect.GetComponent<ParticleSystem>().Play();
    }

    #endregion
}

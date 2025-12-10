using DG.Tweening;
using ExitGames.Client.Photon;
using NaughtyAttributes.Test;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardNamesScriptable;
using static GameConstants;

public class Player : Boxer
{
    public static float playerAttackPriority => (CardsManager.GetPriorityValueForAttack(CardsManager.OnSelectedAttack()));

    public delegate void Attack(AttackType _type, int winnerID);
    public static event Attack onAttack;

    [SerializeField] Transform boxer;
    [SerializeField] Transform targetToMove;

    public delegate void FetchRandomDefence(int targetPlayerID, string reaction);
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

    public delegate void PlayOpponentSweat(int targetPlayerID, string attackType);
    public static event PlayOpponentSweat onOpponentSweatEffect;

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
        onOpponentSweatEffect += CheckAndPlaySweatEffect;
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
        onOpponentSweatEffect -= CheckAndPlaySweatEffect;
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
        _animationRPC.RPC_SetFloat("DefenceBlendIndex", 0);
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

    public static void GetRandomDefence(int pId, string reaction)
    {
        //onRandomDefence?.Invoke(pId, reaction);
    }

    float valHit = 0;
    float valLastHit = 0;
    [SerializeField] float speed = 0;

    [Header("Hit Boxes Refs")]
    public Transform faceHB;
    public Transform bodyHB;
    //[SerializeField] private string faceHBPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head";
    //[SerializeField] private string bodyHBPath = "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2";

    private void Awake()
    {
        //InitializeHitBoxReferences();
    }

    private void InitializeHitBoxReferences()
    {
        if (faceHB == null)
        {
            Transform head = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head");
            if (head != null)
                faceHB = head;
            else
                Debug.LogError($"Could not find Head transform on {gameObject.name}");
        }

        if (bodyHB == null)
        {
            Transform body = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2");
            if (body != null)
                bodyHB = body;
            else
                Debug.LogError($"Could not find Body transform on {gameObject.name}");
        }
    }

    public Transform GetFaceHitBox()
    {
        if (faceHB == null)
            InitializeHitBoxReferences();
        return faceHB;
    }

    public Transform GetBodyHitBox()
    {
        if (bodyHB == null)
            InitializeHitBoxReferences();
        return bodyHB;
    }

    /* private Tween speedTween;
     public void SetBlendSpeed(float targetSpeed, float duration)
     {
         float current = animator.GetFloat(Animations.HitBlendIndex);
         if (Mathf.Approximately(current, targetSpeed) || Mathf.Approximately(valLastHit, targetSpeed))
             return;

         valLastHit = targetSpeed;

         if (speedTween != null && speedTween.IsActive())
             speedTween.Kill();

         speedTween = DOTween.To(() => animator.GetFloat(Animations.HitBlendIndex), x =>
         {
             _animationRPC.RPC_SetFloat(Animations.HitBlendIndex, x);

         }, targetSpeed, duration).SetEase(Ease.Linear).OnComplete(() =>
         {
             valLastHit = -1f;
         });

     }*/
    private Coroutine blendSpeedCoroutine;
    public void SetBlendSpeed(float targetSpeed, float duration)
    {
        float current = animator.GetFloat(Animations.HitBlendIndex);
        if (Mathf.Approximately(current, targetSpeed) || Mathf.Approximately(valLastHit, targetSpeed))
            return;

        valLastHit = targetSpeed;

        //_animationRPC.RPC_SetFloat(Animations.HitBlendIndex, targetSpeed);

        animator.SetFloat(Animations.HitBlendIndex, targetSpeed);

        /* if (blendSpeedCoroutine != null)
             StopCoroutine(blendSpeedCoroutine);

         blendSpeedCoroutine = StartCoroutine(AnimateBlendSpeed(targetSpeed, duration));*/
    }

    private IEnumerator AnimateBlendSpeed(float targetSpeed, float duration)
    {
        float startSpeed = animator.GetFloat(Animations.HitBlendIndex);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float currentValue = Mathf.Lerp(startSpeed, targetSpeed, t);

            //_animationRPC.RPC_SetFloat(Animations.HitBlendIndex, currentValue);

            yield return null;
        }

        _animationRPC.RPC_SetFloat(Animations.HitBlendIndex, targetSpeed);
        valLastHit = -1f;
    }

    void RandomDefense(int targetPlayerID, string hitReaction)
    {
        if (playerID != targetPlayerID)
        {
            return;
        }

        Debug.Log("<color=orange>>>></color> Random Defense Reaction: " + hitReaction);
        int rand = 0;
        float val = 0;

        if (rand == 0)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("hit"))
            {
                Debug.Log("<color=white>>>></color> Hit check");
                //_animationRPC.RPC_ResetTrigger("hit");
                //_animationRPC.RPC_SetTrigger("hit");
                animator.ResetTrigger("hit");
                animator.SetTrigger("hit");
            }

            switch (hitReaction)
            {
                case AnimationReactions.facePunch:
                    valHit = 0.25f;
                    SetBlendSpeed(valHit, speed);
                    break;
                case AnimationReactions.uppercutPunch:
                    valHit = 0.5f;
                    SetBlendSpeed(valHit, speed);
                    break;
                case AnimationReactions.bodyHit:
                    valHit = 0.75f;
                    SetBlendSpeed(valHit, speed);
                    break;
                case AnimationReactions.bodyUppercut:
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

    public static void TriggerOpponentSweat(int targetPlayerID, string attackType)
    {
        onOpponentSweatEffect?.Invoke(targetPlayerID, attackType);
    }
    public static void TriggerOpponentDefence(int targetPlayerID, string reaction)
    {
        Debug.Log("<color=red>>>></color> TriggerOpponentDefence called for playerID: " + targetPlayerID);
        onRandomDefence?.Invoke(targetPlayerID, reaction);
    }

    private void SweatEffect()
    {
        //Debug.Log(">>> 2nd");
        sweatEffect.gameObject.SetActive(true);
        sweatEffect.GetComponent<ParticleSystem>().Play();
        bloodEffect.SetActive(true);
        bloodEffect.GetComponent<ParticleSystem>().Play();
    }
    private void CheckAndPlaySweatEffect(int targetPlayerID, string attackType)
    {
        if (playerID == targetPlayerID)
        {
            bool shouldPlaySweat = attackType.Equals("FacePunch") || attackType.Equals("UppercutPunch");

            if (shouldPlaySweat)
            {
                Debug.Log(">>> 2nd");
                sweatEffect.gameObject.SetActive(true);
                sweatEffect.GetComponent<ParticleSystem>().Play();
                bloodEffect.SetActive(true);
                bloodEffect.GetComponent<ParticleSystem>().Play();
            }
        }
    }



    #endregion
}

/* Overall Network Synchronization Flow Analysis: 

    1. Master Client determines winner
       ↓
    2. GameplayManager.RPC "InvokeAttack" → RpcTarget.All (both clients)
       ↓
    3. Both clients receive RPC and call: Player.OnAttackAction(attackType, winnerID)
       ↓
    4. Static event "onAttack" fires on BOTH clients
       ↓
    5. ALL Player instances on both clients check: "Am I the winner?"
       ↓
    6. Only the WINNER Player triggers: _animationRPC.RPC_SetTrigger()
       ↓
    7. Animation RPC sends to ALL clients (RpcTarget.All)
       ↓
    8. Animation plays on WINNER character on BOTH clients
       ↓
    9. Animation Events fire LOCALLY during animation on BOTH clients
       ↓
    10. AnimationHandler.EffectOnRightHand() called LOCALLY on BOTH clients
       ↓
    11. Player.PlayRightHandEffect() static method called LOCALLY
       ↓
    12. Static event fires, triggering instance method on local Player
       ↓
    13. Particle effects play LOCALLY on BOTH clients

 * now after adding sweat effect logic*
 
    1. Winner's animation pla   ys (synchronized via RPC)
       ↓
    2. Animation Event fires locally on both clients
       ↓
    3. AnimationHandler.EffectOnRightHand() called locally
       ↓
    4. WINNER's punch effect plays (on attacker's hand)
       ↓
    5. Determine opponent's player ID
       ↓
    6. Call Player.TriggerOpponentSweat(opponentID, attackType)
       ↓
    7. Static event fires on all Player instances locally
       ↓
    8. Each Player checks: "Is this sweat for me?"
       ↓
    9. OPPONENT's sweat effect plays (on the one getting hit)
 
 */
using System.Collections.Generic;
using UnityEngine;

public class Boxer : MonoBehaviour
{
    public enum BoxerType
    {
        player,
        Ai
    }
    public enum AttackType
    {
        idle,
        powerpunch,
        combo,
        uppercut,
        overhand,
        hook,
        body,
        jab
    }
    
    [SerializeField] BoxerType type;
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected int pointsEarned;
    [SerializeField] protected CardNamesScriptable cardType;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected List<GameObject> effectsPrefab;
    [SerializeField] protected Transform rightHandEffectParent;
    [SerializeField] protected Transform leftHandEffectParent;
    [SerializeField] protected GameObject sweatEffect;

}

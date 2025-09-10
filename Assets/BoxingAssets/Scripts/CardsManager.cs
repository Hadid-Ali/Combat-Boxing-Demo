using System.Collections.Generic;
using UnityEngine;
using static Boxer;

public class CardsManager : MonoBehaviour
{
    [SerializeField] CardNamesScriptable cardDictionary;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform cardParent;
    [SerializeField] int totalCardsNeeded;
    [SerializeField] int index = 0;
    [SerializeField] List<GameObject> cardsInstantiated;

    public delegate void CloneCards();
    public static event CloneCards onCloneCard;
    [SerializeField] List<CardNamesScriptable.Card> powers;
    [SerializeField] AttackType selectedAttack;

    [SerializeField] AttackType opponentsAttack;

    public delegate AttackType GetSelectedAttack();
    public static event GetSelectedAttack onAttackSelected;

    public delegate AttackType GetOpponentSelectedAttack();
    public static event GetOpponentSelectedAttack onOpponentAttackSelected;

    public delegate void SetOpponentSelectedAttack(AttackType val);
    public static event SetOpponentSelectedAttack onOpponentAttackSelection;

    public delegate void SetSelectedAttack(AttackType val);
    public static event SetSelectedAttack onAttackSet;

    public delegate List<GameObject> GetCards();
    public static event GetCards onGettingAvailableCards;

    public delegate float SetAttackPriority(AttackType _type);
    public static event SetAttackPriority onPriorityCheck;
    private void Start()
    {
        powers.AddRange(cardDictionary.cards);
        InstantiatingCards();
    }
    private void OnEnable()
    {
        onCloneCard += IntantiateCards;
        onAttackSelected += GetAttackSelected;
        onAttackSet += SetAttackValue;
        onOpponentAttackSelected += GetAttackSelectedForOpponent;
        onGettingAvailableCards += GetCardsInstantiated;
        onOpponentAttackSelection += SetOpponentsAttack;
        onPriorityCheck += GetAttackPriority;
    }

    private void OnDisable()
    {
        onCloneCard -= IntantiateCards;
        onAttackSelected -= GetAttackSelected;
        onAttackSet -= SetAttackValue;
        onOpponentAttackSelected -= GetAttackSelectedForOpponent;
        onGettingAvailableCards += GetCardsInstantiated;
        onOpponentAttackSelection -= SetOpponentsAttack;
        onPriorityCheck -= GetAttackPriority;
    }

    public static void InstantiatingCards()
    {
        onCloneCard?.Invoke();
    }
    void IntantiateCards()
    {
        if (index < totalCardsNeeded)
        {
            if (powers.Count < totalCardsNeeded)
            {
                powers.Clear();
                powers.AddRange(cardDictionary.cards);
            }
            int r = Random.Range(0, (powers.Count-1));
            GameObject g = Instantiate(cardPrefab, cardParent.position, Quaternion.identity);
            cardsInstantiated.Add(g);
            g.transform.SetParent(cardParent.transform);
            g.transform.localEulerAngles = Vector3.zero;
            g.transform.localScale = Vector3.one;
            g.transform.localPosition = Vector3.zero;
            Card_Info c_info = g.GetComponent<Card_Info>();
                c_info.m_TextMeshPro.text = powers[r].name;
            if(powers[r].cardSprite)
                c_info.icon.sprite = powers[r].cardSprite;
            powers.Remove(powers[r]);
            index++;
            IntantiateCards();
        }
    }

    void ResetIndex()
    {
        index = 0;
        powers.AddRange(cardDictionary.cards);
        //powers.Reverse();
    }
    public static AttackType OnSelectedAttack()
    {
        return onAttackSelected.Invoke();
    }
    AttackType GetAttackSelected()
    {
        return selectedAttack;
    }
    public static AttackType OnOpponentSelectedAttack()
    {
        return onOpponentAttackSelected.Invoke();
    }
    AttackType GetAttackSelectedForOpponent()
    {
        return opponentsAttack;
    }
    public static void OnSetAttack(AttackType val)
    {
        onAttackSet?.Invoke(val);
    }
    void SetAttackValue(AttackType attack)
    {
        selectedAttack = attack;
        //Debug.LogError("selectedAttack "+ selectedAttack);
    }
    public static void SetOpponentSelectedCard(AttackType attack)
    {
        onOpponentAttackSelection?.Invoke(attack);
    }
    void SetOpponentsAttack(AttackType attack)
    {
        opponentsAttack = attack;
        Debug.LogError("selected opponent");
    }
   
    public static List<GameObject> GetAvailabeCards()
    {
        return onGettingAvailableCards.Invoke();
    }
    List<GameObject> GetCardsInstantiated()
    {
        return cardsInstantiated;
    }

    public static float GetPriorityValueForAttack(AttackType _type)
    {
       return onPriorityCheck.Invoke(_type);
    }
    float GetAttackPriority(AttackType _type)
    {
        float _priority = 0;
        foreach (CardNamesScriptable.Card c in cardDictionary.cards)
        {
            if (c.name.ToLower().Equals(_type.ToString().ToLower()))
            {
                _priority = c.speed;
            }
        }
        return _priority;
    }
}

using UnityEngine;

public class OpponentAI : Boxer
{
    public delegate void Attack(AttackType _type);
    public static event Attack onAttack;

    public delegate void PointsEarned(int points);
    public static event PointsEarned onEarnedPoints;

    public delegate void GetCard();
    public static event GetCard onCardSelection;

    private void OnEnable()
    {
        onAttack += AttackAction;
        onEarnedPoints += UpdatePoints;
        onCardSelection += AvailableCards;
    }
    private void OnDisable()
    {
        onAttack -= AttackAction;
        onEarnedPoints -= UpdatePoints;
        onCardSelection -= AvailableCards;
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
        CardNamesScriptable.Card _card = null;
        foreach (CardNamesScriptable.Card c in cardType.cards)
        {
            if (c.name.ToLower().Equals(_attack.ToString()))
            {
                _card = c;
                break;
            }
        }
        switch (_attack)
        {
            case AttackType.powerpunch:
                Debug.Log("-----POWER PUNCH CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.powerpunch);
                break;
            case AttackType.combo:
                Debug.Log("-----COMBO CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.combo);
                break;
            case AttackType.uppercut:
                Debug.Log("-----UPPER CUT CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.uppercut);
                break;
            case AttackType.overhand:
                Debug.Log("-----OVERHAND CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.overhand);
                break;
            case AttackType.hook:
                Debug.Log("-----HOOK CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.hook);
                break;
            case AttackType.body:
                Debug.Log("-----BODY CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.body);
                break;
            case AttackType.jab:
                Debug.Log("-----JAB CALLED-----");
                UpdatePoints(_card.rewardAmount);
                CallAnimation(AttackType.jab);
                break;
        }
    }

    void UpdatePoints(int p)
    {
        pointsEarned += p;
        GameHUD.OnUpdatingPoints(BoxerType.Ai, p);
    }
    void CallAnimation(AttackType anim)
    {

    }

    public static void GetCardForAi()
    {
        onCardSelection?.Invoke();
    }
    void AvailableCards()
    {
        foreach (GameObject g in CardsManager.GetAvailabeCards())
        {
            Card_Info c = g.GetComponent<Card_Info>();
            if (!c.selected)
            {
                c.SetAttackType(GameHUD.GetAICardTargetPosition(), BoxerType.Ai);
                CardsManager.SetOpponentSelectedCard(c._type);
                break;
            }
        }
        GameplayManager.SetAttack(true);

    }
    #endregion
}

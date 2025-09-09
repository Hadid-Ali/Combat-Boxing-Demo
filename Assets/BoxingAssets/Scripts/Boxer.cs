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

    private void Update()
    {
        //before that call camera animation set to center
        //ui will be disabled
        //attack started
            Attack(); // after some delay call attack
    }
    void Attack()
    {
        if (GameplayManager.GetAttackCall())
        {
            Invoke("ShiftCameraToFocusCombat", 1);
            GameplayManager.SetAttack(false);
        }
            
    }

    void ShiftCameraToFocusCombat()
    {
        GameplayManager.ShiftCamera(GameplayManager.GetCombatCamera());
        GameHUD.DisableBottomUI(false);
        Invoke("CallForAttack", 1);
        CancelInvoke("ShiftCameraToFocusCombat");
    }

    void CallForAttack()
    {
        GameHUD.AvailableRounds();
        Player.OnAttackAction(CardsManager.OnSelectedAttack());
        OpponentAI.OnAttackAction(CardsManager.OnOpponentSelectedAttack());
        CancelInvoke("CallForAttack");
    }
}

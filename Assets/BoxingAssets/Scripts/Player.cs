using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Player : Boxer
{

    public delegate void PowerUpAttack();
    public static event PowerUpAttack onPowerup;

    public delegate void PowerPunchAttack();
    public static event PowerPunchAttack onPowerPunch;

    public delegate void ComboAttack();
    public static event ComboAttack onCombo;

    public delegate void UpperCutAttack();
    public static event UpperCutAttack onUppercut;

    public delegate void OverHandAttack();
    public static event OverHandAttack onOverhand;

    public delegate void HookAttack();
    public static event HookAttack onHook;

    public delegate void BodyAttack();
    public static event BodyAttack onBody;

    public delegate void JabAttack();
    public static event JabAttack onJab;
    private void OnEnable()
    {
        onPowerup += PowerUp;
        onPowerPunch += PowerPunch;
        onCombo += Combo;
        onUppercut += UpperCut;
        onOverhand += OverHand;
        onHook += Hook;
        onBody += Body;
        onJab += Jab;
    }
    private void OnDisable()
    {
        onPowerup -= PowerUp;
        onPowerPunch -= PowerPunch;
        onCombo -= Combo;
        onUppercut -= UpperCut;
        onOverhand -= OverHand;
        onHook -= Hook;
        onBody -= Body;
        onJab -= Jab;
    }
    #region Events Invoke
    public static void OnPowerUpAttack()
    {
        onPowerup?.Invoke();
    }
    public static void OnPowerPunchAttack()
    {
        onPowerPunch?.Invoke();
    }
    public static void OnComboAttack()
    {
        onCombo?.Invoke();
    }
    public static void OnUppercutAttack()
    {
        onUppercut?.Invoke();
    }
    public static void OnOverhandAttack()
    {
        onOverhand?.Invoke();
    }
    public static void OnHookAttack()
    {
        onHook?.Invoke();
    }
    public static void OnBodyAttack()
    {
        onBody?.Invoke();
    }
    public static void OnJabAttack()
    {
        onJab?.Invoke();
    }
    #endregion
}

using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum Turn
    {
        player,
        ai
    }
    [SerializeField] Turn activeTurn;

    public delegate void TurnDelegate(Turn turn);
    public static event TurnDelegate onSwitchTurn;

    private void OnEnable()
    {
        onSwitchTurn += SwitchTurns;
    }

    private void OnDisable()
    {
        onSwitchTurn -= SwitchTurns;
    }

    public static void OnSwitchTurn(Turn activeTurn)
    {
        onSwitchTurn?.Invoke(activeTurn);
    }
    void SwitchTurns(Turn activeTurn)
    {
        switch (activeTurn)
        {
            case Turn.player:
                break;
            case Turn.ai:
                break;
        }
    }

    void TurnIncrement_Decrement(Turn activeTurn, bool increment, string attackType)
    {
        switch (activeTurn)
        {
            case Turn.player:
                break;
            case Turn.ai:
                break;
        }
    }
}

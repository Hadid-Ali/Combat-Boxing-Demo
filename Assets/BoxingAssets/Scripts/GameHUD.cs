using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] Image cardSelectionTimer;
    [SerializeField] Color[] barColor;
    public delegate void StartCardSelectionTimer(float val);
    public static event StartCardSelectionTimer onCardTimerStart;
    bool isFull => (cardSelectionTimer.fillAmount == 1);
    bool isEmpty => (cardSelectionTimer.fillAmount == 0);

    public delegate void ChangeTimerBarColor(int val);
    public static event ChangeTimerBarColor onChangingTimerBarColor;
    private void OnEnable()
    {
        onCardTimerStart += FillBar;
        onChangingTimerBarColor += ChangeBarColor;
    }
    private void OnDisable()
    {
        onCardTimerStart -= FillBar;
        onChangingTimerBarColor -= ChangeBarColor;

    }
    public static void ChooseCardTimer(float val)
    {
        onCardTimerStart?.Invoke(val);
    }
    void FillBar(float val)
    {
        cardSelectionTimer.fillAmount -= val;
    }

    public static void TurnWiseChangeBarColor(int index)
    {
        onChangingTimerBarColor?.Invoke(index);
    }
    void ChangeBarColor(int i)
    {
        cardSelectionTimer.color = barColor[i];
    }
}

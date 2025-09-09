using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] RectTransform playerCardTargetPos;
    [SerializeField] RectTransform aiCardTargetPos;
    [SerializeField] GameObject bottomUi;
    [SerializeField] Image cardSelectionTimer;
    [SerializeField] Color[] barColor;
    [SerializeField] TextMeshProUGUI roundsText;
    [SerializeField] TextMeshProUGUI playerPoints;
    [SerializeField] TextMeshProUGUI aiPoints;
    [SerializeField] int totalRounds;
    [SerializeField] int playedRound;
    [SerializeField] float targetValueForBG;
    [SerializeField] float animationSpeedForBottomUi;
    public delegate void StartCardSelectionTimer(float val);
    public static event StartCardSelectionTimer onCardTimerStart;

    public delegate void Rounds();
    public static event Rounds onRoundsAvailablity;

    public delegate void UpdatePoints(Boxer.BoxerType _type, int points);
    public static event UpdatePoints onPointsUpdate;

    public delegate void BottomUIActivity(bool active);
    public static event BottomUIActivity onUiActivity;

    public delegate RectTransform GetPlayerSelectedCardTarget();
    public static event GetPlayerSelectedCardTarget onPlayerCardTargetPos;

    public delegate RectTransform GetAISelectedCardTarget();
    public static event GetAISelectedCardTarget onAICardTargetPos;
    bool isFull => (cardSelectionTimer.fillAmount == 1);
    bool isEmpty => (cardSelectionTimer.fillAmount == 0);

    private void Update()
    {
        ChooseCardTimer(0.01f);
    }
    private void OnEnable()
    {
        onCardTimerStart += FillBar;
        onRoundsAvailablity += CheckRounds;
        onPointsUpdate += UpdatePointUI;
        onUiActivity += DisableUI;
        onPlayerCardTargetPos += GetPlayerCardTargetPos;
        onAICardTargetPos += GetAICardTargetPos;
    }
    private void OnDisable()
    {
        onCardTimerStart -= FillBar;
        onRoundsAvailablity -= CheckRounds;
        onPointsUpdate += UpdatePointUI;
        onUiActivity -= DisableUI;
        onPlayerCardTargetPos += GetPlayerCardTargetPos;
        onAICardTargetPos += GetAICardTargetPos;
    }
    public static void ChooseCardTimer(float val)
    {
        onCardTimerStart?.Invoke(val);
    }
    void FillBar(float val)
    {
        val = val * Time.deltaTime;
        cardSelectionTimer.fillAmount -= val;
    }

    public static void AvailableRounds()
    {
        onRoundsAvailablity?.Invoke();
    }
    void CheckRounds()
    {

        playedRound++;
        if(playedRound >= totalRounds)
            playedRound = totalRounds;
        roundsText.text = playedRound + "/"+totalRounds.ToString();

        Debug.LogError("CheckRounds " + playedRound);

    }

    public static void OnUpdatingPoints(Boxer.BoxerType _type, int points)
    {
        onPointsUpdate?.Invoke(_type, points);
    }
    void UpdatePointUI(Boxer.BoxerType _type, int points)
    {
        switch (_type)
        {
            case Boxer.BoxerType.player:
                playerPoints.text = points.ToString();
                break;
            case Boxer.BoxerType.Ai:
                aiPoints.text = points.ToString();
                break;
        }
    }

    public static void DisableBottomUI(bool val)
    {
        onUiActivity.Invoke(val);
    }
    void DisableUI(bool ui)
    {
        bottomUi.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, targetValueForBG), animationSpeedForBottomUi).SetEase(Ease.Linear).OnComplete(() =>
        {
            bottomUi.SetActive(ui);
            bottomUi.transform.DOPause();
        });
    }

    public static RectTransform GetPlayerCardTargetPosition()
    {
        return onPlayerCardTargetPos.Invoke();
    }
    public static RectTransform GetAICardTargetPosition()
    {
        return onAICardTargetPos.Invoke();
    }
    RectTransform GetPlayerCardTargetPos()
    {
        return playerCardTargetPos;
    }

    RectTransform GetAICardTargetPos()
    {
        return aiCardTargetPos;
    }
}

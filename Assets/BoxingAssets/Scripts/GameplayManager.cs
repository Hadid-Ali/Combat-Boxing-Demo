using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Boxer;


public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public CardsManager cardsManager;
    public CameraManager cameraManager;
    public RoundTracker roundTracker;

    [SerializeField] bool startAttack;
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;    

    public delegate bool CheckAttackCall();
    public static event CheckAttackCall onStartAttack;

    public delegate void SetStartAttack(bool val);
    public static event SetStartAttack onValueSetAttack;

    public delegate string FetchFirstAttacker();
    public static event FetchFirstAttacker onRevealFirstAttacker;

    public delegate void SetFirstAttacker(string val);
    public static event SetFirstAttacker onSetFirstAttackerValue;

    [SerializeField] string boxerWhoWillAttackFirst;

    [SerializeField] private Card_Info player1SelectedCard;
    [SerializeField] private Card_Info player2SelectedCard;
    [SerializeField] private Card_Info highPriorityCard;

    private PhotonView _photonView;
    private List<Player> joinedPlayers = new List<Player>();


    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void RegisterPlayer(Player player)
    {
        joinedPlayers.Add(player);
    }

    public Card_Info GetHighPriorityCard()
    {
        return highPriorityCard;
    }

    public void SetPlayer1SelectedCard(Card_Info card)
    {
        player1SelectedCard = card;
    }

    public Card_Info GetPlayer1SelectedCard()
    {
        return player1SelectedCard;
    }

    public void SetPlayer2SelectedCard(Card_Info card)
    {
        player2SelectedCard = card;
    }

    public Card_Info GetPlayer2SelectedCard()
    {
        return player2SelectedCard;
    }

    public void ClearSelectedCards()
    {
        player1SelectedCard = null;
        player2SelectedCard = null;
        highPriorityCard = null;
    }

    public void StartCardPriorityEvaluation()
    {
        if (player1SelectedCard == null || player2SelectedCard == null)
        {
            Debug.Log("Cannot evaluate card priority: One or both players have not selected a card.");
            return;
        }

        if(PhotonNetwork.IsMasterClient)
            EvaluateCardPriority();
    }

    public void EvaluateCardPriority()
    {
        var cardP1 = player1SelectedCard;
        var cardP2 = player2SelectedCard;

        // Safety checks
        if (cardP1 == null || cardP2 == null)
        {
            Debug.Log("Both players must have selected cards before evaluating priority.");
            return;
        }

        if (cardP1.priority > cardP2.priority)
        {
            highPriorityCard = cardP1;
            OnCardPriorityResolved(1, cardP1, cardP2);
        }
        else if (cardP2.priority > cardP1.priority)
        {
            highPriorityCard = cardP2;
            OnCardPriorityResolved(2, cardP2, cardP1);
        }
        else
        {
            highPriorityCard = cardP1;
            OnCardPriorityResolved(0, cardP1, cardP2);
        }
    }

    // Called after priority evaluation
    private void OnCardPriorityResolved(int winnerId, Card_Info winningCard, Card_Info losingCard)
    {
        roundTracker.RegisterRoundWinner(winnerId);
        cameraManager.SwitchByWinner(winnerId, () =>
        {
            StartAttackSequence(winnerId);
        });
    }


    public void ResetRoundInternal()
    {        
        _photonView.RPC(nameof(ResetCamAnOtherData), RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void ResetCamAnOtherData()
    {
        cameraManager.SwitchToMain(() =>
        {
            Debug.Log("Resetting round after attack sequence.");
            double fireTime = PhotonNetwork.Time + 1.0;
            StartRoundDelay(fireTime);
        });
    }


    private void StartRoundDelay(double delay)
    {
        Debug.Log("Starting round delay until: " + delay);
        StartCoroutine(RoundDelayCoroutine(delay));
    }

    private IEnumerator RoundDelayCoroutine(double delay)
    {
        while (PhotonNetwork.Time < delay)
            yield return null;

        // Master handles card destruction
        if (PhotonNetwork.IsMasterClient)
            DestroySelectedCards();

        // Clear references locally and re-enable selection
        ResetRound();
    }

    private void DestroySelectedCards()
    {
        if (player1SelectedCard != null)
            PhotonNetwork.Destroy(player1SelectedCard.gameObject);

        if (player2SelectedCard != null)
            PhotonNetwork.Destroy(player2SelectedCard.gameObject);
    }

    public void ResetRound()
    {
        // Clear local references
        player1SelectedCard = null;
        player2SelectedCard = null;

        // Enable UI for next round
        if (cardsManager != null)
            cardsManager.EnableCardSelection(true);


        if (PhotonNetwork.IsMasterClient && roundTracker.CurrentRound >= roundTracker.maxRounds)
        {
            StartCoroutine(WinnerDeclare());
        }
    }


    private IEnumerator WinnerDeclare()
    {
        yield return new WaitForSeconds(2f);
        _photonView.RPC(nameof(RPC_MatchFinished), RpcTarget.All);
    }


    [PunRPC]
    private void RPC_MatchFinished()
    {
        Debug.Log("Match Finished. Displaying Win Panel.");
        roundTracker.ShowWinPanel();
    }

    private void OnEnable()
    {        
        onStartAttack += GetAttackValue;
        onValueSetAttack += SetAttackValue;
        onRevealFirstAttacker += GetFirstAttacker;
        onSetFirstAttackerValue += ReSetFirstAttacker;
        GameEvents.BoxingDemoGameFlowEvents.RoundComplete.Register(HandleRoundEnd);
        GameEvents.BoxingDemoGameFlowEvents.MatchFinished.Register(HandleMatchFinished);
    }

    private void OnDisable()
    {        
        onStartAttack -= GetAttackValue;
        onValueSetAttack -= SetAttackValue;
        onRevealFirstAttacker -= GetFirstAttacker;
        onSetFirstAttackerValue -= ReSetFirstAttacker;
        GameEvents.BoxingDemoGameFlowEvents.RoundComplete.UnRegister(HandleRoundEnd);
        GameEvents.BoxingDemoGameFlowEvents.MatchFinished.UnRegister(HandleMatchFinished);
    }

    private void HandleRoundEnd(int winnerID, int round)
    {
        Debug.Log($"Round {round} winner: Player {winnerID}");
        // Play round win animation, reset fighters, etc.
    }

    private void HandleMatchFinished(int winnerID)
    {
        if (winnerID == -1)
            Debug.Log("Match is a DRAW!");
        else
            Debug.Log($"Match Winner: Player {winnerID}");
    }

    public static void SetAttack(bool val)
    {
        onValueSetAttack?.Invoke(val);
    }

    private void SetAttackValue(bool val)
    {
        startAttack = val;
    }

    public static bool GetAttackCall()
    {
        return onStartAttack.Invoke();
    }

    private bool GetAttackValue()
    {
        return startAttack;
    }


    public void StartAttackSequence(int winnerID)
    {
        Debug.Log("Starting Attack Sequence with High Priority Card: " + (int)highPriorityCard._type);
        _photonView.RPC(nameof(InvokeAttack), RpcTarget.All, (int)highPriorityCard._type, winnerID);
    }


    [PunRPC]
    public void InvokeAttack(int _type, int winnerID)
    {
        Debug.Log("Player Invoking Attack: " + (AttackType)_type + this.gameObject.name + "Winner ID" + winnerID);
        Player.OnAttackAction((AttackType)_type, winnerID);
    }

    public static string FirstAttacker()
    {
        return onRevealFirstAttacker.Invoke();
    }

    private string GetFirstAttacker()
    {
        return boxerWhoWillAttackFirst;
    }

    public static void ResetFirstAttackerValue(string val)
    {
        onSetFirstAttackerValue?.Invoke(val);
    }

    private void ReSetFirstAttacker(string val)
    {
        boxerWhoWillAttackFirst = val;
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    [SerializeField] GameObject mainCamera;
    [SerializeField] Transform playerCameraTransform;
    [SerializeField] Transform opponentCameraTransform;
    [SerializeField] Transform attackFocusedCamera;
    [SerializeField] CardsManager cardsManager;

    public delegate void ChangeCameraPosition(Transform pos);
    public static event ChangeCameraPosition onChangingCameraPosition;
    [SerializeField] bool startAttack;

    public delegate bool CheckAttackCall();
    public static event CheckAttackCall onStartAttack;

    public delegate void SetStartAttack(bool val);
    public static event SetStartAttack onValueSetAttack;

    public delegate Transform GetAttackCamera();
    public static event GetAttackCamera onAttackCamera;


    public delegate string FetchFirstAttacker();
    public static event FetchFirstAttacker onRevealFirstAttacker;

    public delegate void SetFirstAttacker(string val);
    public static event SetFirstAttacker onSetFirstAttackerValue;

    [SerializeField] string boxerWhoWillAttackFirst;

    [SerializeField] private Card_Info player1SelectedCard;
    [SerializeField] private Card_Info player2SelectedCard;


    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
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

        // Compare priorities (assuming higher number = higher priority)
        if (cardP1.priority > cardP2.priority)
        {
            OnCardPriorityResolved(1, cardP1, cardP2);
        }
        else if (cardP2.priority > cardP1.priority)
        {
            OnCardPriorityResolved(2, cardP2, cardP1);
        }
        else
        {
            OnCardPriorityResolved(0, cardP1, cardP2);
        }
    }

    // Called after priority evaluation
    private void OnCardPriorityResolved(int winnerId, Card_Info winningCard, Card_Info losingCard)
    {
        // Trigger winner logic / animations
        if (winnerId == 1)
            Debug.Log("Player 1 wins the round.");
        else if (winnerId == 2)
            Debug.Log("Player 2 wins the round.");
        else
            Debug.Log("The round is a tie.");

        // Start delay timer on all clients
        _photonView.RPC(nameof(RPC_StartRoundDelay), RpcTarget.AllBuffered, 2f); // 2 seconds delay
    }

    [PunRPC]
    private void RPC_StartRoundDelay(float delay)
    {
        Debug.Log($"Starting round delay of {delay} seconds.");
        StartCoroutine(RoundDelayCoroutine(delay));
    }

    private IEnumerator RoundDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Round delay over. Resetting round.");

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
        Debug.Log("Resetting round...");

        // Clear local references
        player1SelectedCard = null;
        player2SelectedCard = null;

        // Enable UI for next round
        if (cardsManager != null)
            cardsManager.EnableCardSelection(true);

        Debug.Log("New round started — players can select cards.");
    }


    private void OnEnable()
    {
        onChangingCameraPosition += ChangeCamTransform;
        onStartAttack += GetAttackValue;
        onValueSetAttack += SetAttackValue;
        onAttackCamera += GetCameraTargetForCombat;
        onRevealFirstAttacker += GetFirstAttacker;
        onSetFirstAttackerValue += ReSetFirstAttacker;
    }

    private void OnDisable()
    {
        onChangingCameraPosition -= ChangeCamTransform;
        onStartAttack -= GetAttackValue;
        onValueSetAttack -= SetAttackValue;
        onAttackCamera -= GetCameraTargetForCombat;
        onRevealFirstAttacker -= GetFirstAttacker;
        onSetFirstAttackerValue -= ReSetFirstAttacker;
    }
    
    public static void SetAttack(bool val)
    {
        onValueSetAttack?.Invoke(val);
    }
    void SetAttackValue(bool val)
    {
        startAttack = val;
    }
    public static bool GetAttackCall()
    {
        return onStartAttack.Invoke();
    }
    bool GetAttackValue()
    {
        return startAttack;
    }
    private void Update()
    {
        RaycastToDetectPlayerPointer();
        Attack();
    }
    public static void ChangingCameraTransform(Transform pos)
    {
        onChangingCameraPosition?.Invoke(pos);
    }
    void ChangeCamTransform(Transform pos)
    {
        mainCamera.transform.position = pos.position;
        mainCamera.transform.eulerAngles = pos.eulerAngles;
    }
    [SerializeField] float rayDistance;
    [SerializeField]LayerMask layerMask;
    Vector2 screenPoint = Vector2.zero;

    void RaycastToDetectPlayerPointer()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    screenPoint = Input.mousePosition;
        //    if (IsPointerOverUI(screenPoint))
        //    {
        //        Card_Info c = null;
        //        foreach(RaycastResult r in results)
        //        {
        //            if(r.gameObject.GetComponent<Card_Info>())
        //            {
        //                c = r.gameObject.GetComponent<Card_Info>();
        //                c.gameObject.name = c.m_TextMeshPro.text;
        //                c.CardSelected(GameHUD.GetPlayerCardTargetPosition(), Boxer.BoxerType.player);
        //                Invoke("ShiftOpponentCameraOnSelection", 1);
        //                //Debug.Log("UI element was hit" + c.m_TextMeshPro.text);
        //                break;
        //            }
        //        }
                
        //        return; 
        //    }
           
        //}
    }
    List<RaycastResult> results = new List<RaycastResult>();

    bool IsPointerOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        EventSystem.current.RaycastAll(eventData, results);
     
        return results.Count > 0;
    }
    void ShiftOpponentCameraOnSelection()
    {
        CameraManager.SwitchToOpponentPosition();
        Invoke("GetCardForAI", 2);
        CancelInvoke("ShiftOpponentCameraOnSelection");
    }
    void GetCardForAI()
    {
        OpponentAI.GetCardForAi();
        CancelInvoke("GetCardForAI");
    }


    public static Transform GetCombatCamera()
    {
        return onAttackCamera.Invoke();
    }
    Transform GetCameraTargetForCombat()
    {
        return attackFocusedCamera.transform;
    }
    void Attack()
    {
        if (startAttack)
        {
            if(Player.playerAttackPriority < OpponentAI.aiAttackPriority)
                boxerWhoWillAttackFirst = "player";
            else
                boxerWhoWillAttackFirst = "Ai";

            Invoke("ShiftToCombat", 1);
            startAttack = false;
        }

    }

    void ShiftToCombat()
    {
        CameraManager.SwitchToFightingPosition();
        GameHUD.DisableBottomUI(false);
        Invoke("CallForAttack", 1);
        CancelInvoke("ShiftToCombat");
    }

    void CallForAttack()
    {
        GameHUD.AvailableRounds();
        if(boxerWhoWillAttackFirst.ToLower().Equals(Boxer.BoxerType.player.ToString().ToLower()))
            Player.OnAttackAction(CardsManager.OnSelectedAttack());
        else
            OpponentAI.OnAttackAction(CardsManager.OnOpponentSelectedAttack());
        CancelInvoke("CallForAttack");
    }

    public static string FirstAttacker()
    {
        return onRevealFirstAttacker.Invoke();
    }
    string GetFirstAttacker()
    {
        return boxerWhoWillAttackFirst;
    }
    public static void ResetFirstAttackerValue(string val)
    {
        onSetFirstAttackerValue?.Invoke(val);
    }
    void ReSetFirstAttacker(string val)
    {
        boxerWhoWillAttackFirst = val;
    }
}

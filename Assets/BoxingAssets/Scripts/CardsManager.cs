using DG.Tweening;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Boxer;

public class CardsManager : MonoBehaviour
{
    [SerializeField] private CardNamesScriptable cardDictionary;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private Transform selectedCardParentP1;
    [SerializeField] private Transform selectedCardParentP2;
    [SerializeField] private GameObject blockerOBJ;

    [SerializeField] List<Card_Info> cardsInstantiated;
    [SerializeField] List<CardNamesScriptable.Boxing_Card> powers;

    [SerializeField] AttackType selectedAttack;
    [SerializeField] AttackType opponentsAttack;

    [SerializeField] bool testing;
    [SerializeField] string attackIndex;
    [SerializeField] int totalCardsNeeded;
    [SerializeField] int index = 0;

    #region Delegates

    public delegate void CloneCards();
    public static event CloneCards onCloneCard;

    public delegate AttackType GetSelectedAttack();
    public static event GetSelectedAttack onAttackSelected;

    public delegate AttackType GetOpponentSelectedAttack();
    public static event GetOpponentSelectedAttack onOpponentAttackSelected;

    public delegate void SetOpponentSelectedAttack(AttackType val);
    public static event SetOpponentSelectedAttack onOpponentAttackSelection;

    public delegate void SetSelectedAttack(AttackType val);
    public static event SetSelectedAttack onAttackSet;

    public delegate List<Card_Info> GetCards();
    public static event GetCards onGettingAvailableCards;

    public delegate float SetAttackPriority(AttackType _type);
    public static event SetAttackPriority onPriorityCheck;

    public delegate void RegenerateCardList();
    public static event RegenerateCardList onRegeneratingCards;

    private PhotonView _photonView;

    #endregion

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        totalCardsNeeded = cardDictionary.cards.Count;
        powers.AddRange(cardDictionary.cards);
        InstantiatingCards();
    }

    private void OnEnable()
    {
        onCloneCard += InstantiateCards;
        onAttackSelected += GetAttackSelected;
        onAttackSet += SetAttackValue;
        onOpponentAttackSelected += GetAttackSelectedForOpponent;
        onGettingAvailableCards += GetCardsInstantiated;
        onOpponentAttackSelection += SetOpponentsAttack;
        onPriorityCheck += GetAttackPriority;
    }

    private void OnDisable()
    {
        onCloneCard -= InstantiateCards;
        onAttackSelected -= GetAttackSelected;
        onAttackSet -= SetAttackValue;
        onOpponentAttackSelected -= GetAttackSelectedForOpponent;
        onGettingAvailableCards += GetCardsInstantiated;
        onOpponentAttackSelection -= SetOpponentsAttack;
        onPriorityCheck -= GetAttackPriority;
    }

    private void InstantiatingCards()
    {
        if (PhotonNetwork.IsMasterClient)
            onCloneCard?.Invoke();
    }

    private void InstantiateCards()
    {
        Debug.Log("Instantiating Cards " + index);

        for (int i = 0; i < cardDictionary.cards.Count; i++)
        {
            var data = cardDictionary.cards[i];
            GameObject g = PhotonNetwork.Instantiate("Network/Cards/CardInfo", cardParent.position,
                Quaternion.identity);

            // Sync spawn info to all clients
            _photonView.RPC(nameof(RPC_SpawnCard),
                RpcTarget.AllBuffered,
                data.id, g.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    private void RPC_SpawnCard(string id, int viewID)
    {
        var card = cardDictionary.cards.Find(c => c.id == id);

        // This runs on ALL clients (called by master)
        GameObject cardObj = PhotonView.Find(viewID).gameObject;
        if (cardObj == null) return;

        Card_Info cardInfo = cardObj.GetComponent<Card_Info>();
        cardsInstantiated.Add(cardInfo);
        cardInfo.transform.SetParent(cardParent);

        cardInfo.m_TextMeshPro.text = card.name;
        cardInfo.cardId = id;
        cardInfo.transform.localScale = Vector3.one;
        cardInfo.icon.sprite = card.cardSprite;
        cardInfo.priority = card.priority;
        cardInfo._type = card.attackType;

        Button btn = cardObj.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnCardSelected(cardInfo));
    }

    public void OnCardSelected(Card_Info cardInfo)
    {
        if (cardsInstantiated.Count == 0)
        {
            Debug.Log("No cards instantiated!");
            return;
        }

        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;

        // Make sure the card has a PhotonView
        PhotonView cardView = PhotonView.Find(cardInfo.PhotonId());

        if (cardView == null)
        {
            Debug.LogError("Card has no PhotonView attached!");
            return;
        }

        blockerOBJ.SetActive(true);

        GameEvents.BoxingDemoGameFlowEvents.CardSelected.Raise();

        // Send the selection info to everyone (movement happens in RPC)
        _photonView.RPC(
            nameof(RPC_CardSelectedMeta),
            RpcTarget.AllBuffered,
            playerId,
            cardInfo.cardId,
            cardView.ViewID
        );
    }

    [PunRPC]
    private void RPC_CardSelectedMeta(int playerId, string cardId, int viewID)
    {
        PhotonView cardView = PhotonView.Find(viewID);
        if (cardView == null)
        {
            Debug.LogWarning($"No card found with ViewID {viewID}");
            return;
        }

        Card_Info card = cardView.GetComponent<Card_Info>();
        if (card == null) return;

        Button btn = card.GetComponent<Button>();
        if (btn != null)
            btn.interactable = false;

        Transform targetParent = (playerId == 1)
            ? selectedCardParentP1
            : selectedCardParentP2;

        RectTransform cardRect = card.GetComponent<RectTransform>();
        RectTransform targetRect = targetParent.GetComponent<RectTransform>();

        Vector3 worldPos = cardRect.position;
        cardRect.SetParent(targetRect, worldPositionStays: true);

        cardRect.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.OutBack);
        cardRect.DORotate(Vector3.zero, 0.4f).SetEase(Ease.OutBack);
        cardRect.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);

        if (playerId == 1)
            GameplayManager.instance.SetPlayer1SelectedCard(card);
        else
            GameplayManager.instance.SetPlayer2SelectedCard(card);

        Debug.Log($"Player {playerId} selected card priority {card.priority}");
        GameplayManager.instance.StartCardPriorityEvaluation();
    }

    public void EnableCardSelection(bool enable)
    {
        //foreach (var card in cardsInstantiated)
        //{
        //    Button btn = card.GetComponent<Button>();
        //    if (btn != null)
        //        btn.interactable = enable;
        //}

        if (blockerOBJ != null)
            blockerOBJ.SetActive(!enable);
    }

    private void ResetIndex()
    {
        index = 0;
        powers.AddRange(cardDictionary.cards);
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
        //Debug.LogError("selected opponent");
    }

    public static List<Card_Info> GetAvailabeCards()
    {
        return onGettingAvailableCards.Invoke();
    }

    public List<Card_Info> GetCardsInstantiated()
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
        foreach (CardNamesScriptable.Boxing_Card c in cardDictionary.cards)
        {
            if (c.name.ToLower().Equals(_type.ToString().ToLower()))
            {
                _priority = c.speed;
            }
        }
        return _priority;
    }

    public static void RegenerateCards()
    {
        onRegeneratingCards?.Invoke();
    }
}

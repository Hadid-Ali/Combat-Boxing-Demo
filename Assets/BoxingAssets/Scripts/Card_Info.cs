using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card_Info : MonoBehaviour
{
    public TextMeshProUGUI m_TextMeshPro;
    public Image icon;
    public string cardId;
    public int priority;
    public bool selected = false;
    public Boxer.AttackType _type;
    [SerializeField] float animationSpeed;


    private PhotonView m_PhotonView;

    private void Start()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public int PhotonId()
    {
        if (m_PhotonView != null)
            return m_PhotonView.ViewID;
        else
            return -1;
    }

    public void CardSelected(RectTransform target, Boxer.BoxerType pType)
    {
        SetAttackType(target, pType);
        CardsManager.OnSetAttack(_type);
        selected = true;
    }

    public void SetAttackType(RectTransform target, Boxer.BoxerType pType)
    {
        string attckName = m_TextMeshPro.text;
        switch (attckName)
        {
            case "POWERPUNCH":
                _type = Boxer.AttackType.powerpunch;
                break;
            case "COMBO":
                _type = Boxer.AttackType.combo;
                break;
            case "UPPERCUT":
                _type = Boxer.AttackType.uppercut;
                break;
            case "OVERHAND":
                _type = Boxer.AttackType.overhand;
                break;
            case "HOOK":
                _type = Boxer.AttackType.hook;
                break;
            case "BODY":
                _type = Boxer.AttackType.body;
                break;
            case "JAB":
                _type = Boxer.AttackType.jab;
                break;
        }
        CardSelectionAnimation(target, pType);
    }

    void CardSelectionAnimation(RectTransform _target, Boxer.BoxerType pType)
    {
        this.transform.SetParent(_target.parent);
        RectTransform t = this.GetComponent<RectTransform>();

        t.DOAnchorPos(_target.anchoredPosition, animationSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {          
            m_TextMeshPro.enabled = true;
            if(icon)
                icon.gameObject.SetActive(true);
            t.DOPause();
        });
    }
   
}

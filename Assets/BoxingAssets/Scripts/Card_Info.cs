using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Card_Info : MonoBehaviour
{
    public TextMeshProUGUI m_TextMeshPro;
    public Image icon;
    public bool selected = false;
    public Boxer.AttackType _type;
    [SerializeField] float animationSpeed;
    public void CardSelected(RectTransform target, Boxer.BoxerType pType)
    {
        SetAttackType(target, pType);
        Debug.LogError("card selected ");
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
        //Vector3 worldPos = this.transform.position;
        this.transform.SetParent(_target.parent);
        //Vector3 localPos = _target.InverseTransformPoint(worldPos);
        //this.transform.localPosition = worldPos;
        RectTransform t = this.GetComponent<RectTransform>();

       
        t.DOAnchorPos(_target.anchoredPosition, animationSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            //if (pType.Equals(Boxer.BoxerType.player))
            //{
            //    t.anchorMin = new Vector2(0f, 1f);
            //    t.anchorMax = new Vector2(0f, 1f);
            //}
            //else
            //{
            //    t.anchorMin = new Vector2(1f, 1f);
            //    t.anchorMax = new Vector2(1f, 1f);
            //}
            t.DOPause();
        });
    }

   
    //on car selection -- which 
}

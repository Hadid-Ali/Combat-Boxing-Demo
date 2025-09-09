using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] GameObject mainCamera;
    [SerializeField] float cameraMoveSpeed;
    [SerializeField] Transform playerCameraTransform;
    [SerializeField] Transform opponentCameraTransform;
    [SerializeField] Transform attackFocusedCamera;
    private Tween suckTween;

    public delegate void ChangeCameraPosition(Transform pos);
    public static event ChangeCameraPosition onChangingCameraPosition;
    [SerializeField] bool startAttack;

    public delegate bool CheckAttackCall();
    public static event CheckAttackCall onStartAttack;

    public delegate void SetStartAttack(bool val);
    public static event SetStartAttack onValueSetAttack;

    public delegate Transform GetAttackCamera();
    public static event GetAttackCamera onAttackCamera;

    public delegate void ShiftCameraForActivePlayer(Transform _target);
    public static event ShiftCameraForActivePlayer onShiftingCamera;
    private void OnEnable()
    {
        onChangingCameraPosition += ChangeCamTransform;
        onStartAttack += GetAttackValue;
        onValueSetAttack += SetAttackValue;
        onShiftingCamera += CameraShifting;
        onAttackCamera += GetCameraTargetForCombat;
    }

    private void OnDisable()
    {
        onChangingCameraPosition -= ChangeCamTransform;
        onStartAttack -= GetAttackValue;
        onValueSetAttack -= SetAttackValue;
        onShiftingCamera -= CameraShifting;
        onAttackCamera -= GetCameraTargetForCombat;
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
        if (Input.GetMouseButtonDown(0))
        {
            screenPoint = Input.mousePosition;
            if (IsPointerOverUI(screenPoint))
            {
                Card_Info c = null;
                foreach(RaycastResult r in results)
                {
                    if(r.gameObject.GetComponent<Card_Info>())
                    {
                        c = r.gameObject.GetComponent<Card_Info>();
                        c.gameObject.name = c.m_TextMeshPro.text;
                        c.CardSelected(GameHUD.GetPlayerCardTargetPosition(), Boxer.BoxerType.player);
                        Invoke("ShiftOpponentCameraOnSelection", 1);
                        //Debug.Log("UI element was hit" + c.m_TextMeshPro.text);
                        break;
                    }
                }
                
                return; 
            }
           
        }
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
        ShiftCamera(opponentCameraTransform);
        Invoke("GetCardForAI", 2);
        CancelInvoke("ShiftOpponentCameraOnSelection");
    }
    void GetCardForAI()
    {
        OpponentAI.GetCardForAi();
        CancelInvoke("GetCardForAI");
    }

    public static void ShiftCamera(Transform target)
    {
        onShiftingCamera?.Invoke(target);
    }
    void CameraShifting(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target Point not assigned!");
            return;
        }

        if (suckTween != null && suckTween.IsActive())
            suckTween.Kill();

        Sequence seq = DOTween.Sequence();

        seq.Join(mainCamera.transform.DOMove(target.position, cameraMoveSpeed).SetEase(Ease.OutCirc));

        seq.Join(mainCamera.transform.DORotate(target.eulerAngles, cameraMoveSpeed).SetEase(Ease.OutCirc));

        suckTween = seq;
   
    }

    public static Transform GetCombatCamera()
    {
        return onAttackCamera.Invoke();
    }
    Transform GetCameraTargetForCombat()
    {
        return attackFocusedCamera.transform;
    }
}

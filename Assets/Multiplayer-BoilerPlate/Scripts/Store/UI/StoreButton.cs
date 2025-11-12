using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreButton : MonoBehaviour
{
    public StorePageName storeName;
    
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unSelectedColor;

#if UNITY_EDITOR
    private void OnValidate()
    {
        string val = storeName.ToString();        
        SetTitle(val);
    }
#endif
    private void Awake()
    {
        button.onClick.AddListener(LaunchEvent);
        GameEvents.StoreEvents.OnStoreButtonClicked.Register(OnStoreButtonClicked);
    }
        private void OnDestroy()
    {
        button.onClick.RemoveListener(LaunchEvent);
        GameEvents.StoreEvents.OnStoreButtonClicked.UnRegister(OnStoreButtonClicked);
    }

    private void OnStoreButtonClicked(StorePageName obj)
    {
        image.color = obj == storeName ? selectedColor : unSelectedColor;
    }
    private void LaunchEvent()
    {
        GameEvents.StoreEvents.OnStoreButtonClicked.Raise(storeName);  
    }  
    public void SetTitle(string val) => title.SetText(val);
    
    
}

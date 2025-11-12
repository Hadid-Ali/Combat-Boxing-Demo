
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoreSelectionScreen : MonoBehaviour
{
    [SerializeField] private StoreManager storeManager;
    [SerializeField] private Button button;
    [SerializeField] private GameObject container;
    private void  OnEnable()
    {
        StartCoroutine(wait());
        button.onClick.AddListener(()=>container.gameObject.SetActive(false));
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.3f);
        storeManager.InitializeStore();
    }
}

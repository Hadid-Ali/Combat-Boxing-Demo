using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour
{
    public delegate void InstantiateHitEffectDelegate(GameObject prefab, Transform parent, List<GameObject> list);
    public static event InstantiateHitEffectDelegate onInstantiateEffects;
    private void OnEnable()
    {
        onInstantiateEffects += InstantiateHitEffect;
    }
    private void OnDisable()
    {
        onInstantiateEffects -= InstantiateHitEffect;
    }

    public static void InstantiateHitEffects(GameObject prefab, Transform parent, List<GameObject> list)
    {
        onInstantiateEffects?.Invoke(prefab, parent, list);
    }
    void InstantiateHitEffect(GameObject prefab, Transform parent, List<GameObject> list)
    {
        GameObject right_g = Instantiate(prefab, parent, false);
        if (!list.Contains(right_g))
            list.Add(right_g);
        right_g.transform.SetParent(parent);
        right_g.transform.localPosition = Vector3.zero;
        right_g.transform.localEulerAngles = Vector3.zero;
    }
}

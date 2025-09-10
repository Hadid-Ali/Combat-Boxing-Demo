using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour
{
    [SerializeField] List<GameObject> effects;
    [SerializeField] Transform effectAIParent;
    [SerializeField] Transform effectPlayerParent;
    [SerializeField]List<GameObject> instantiatedEffects;

    private void Start()
    {
        InstantiateHitEffect(effectAIParent);
        InstantiateHitEffect(effectPlayerParent);
    }
    void InstantiateHitEffect(Transform parent)
    {
        foreach (GameObject effect in instantiatedEffects)
        {
            GameObject g = Instantiate(effect, parent, false);
            instantiatedEffects.Add(g);
            g.transform.SetParent(parent);
        }
    }
}

using UnityEngine;
using System.Collections;

public class Trainer : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTriggerName = "Talk";
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 8f;

    void Start()
    {
        StartCoroutine(PlayRandomAnimations());
    }

    private IEnumerator PlayRandomAnimations()
    {
        while (true)
        {
            float randomWait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(randomWait);

            TriggerAnimation();
        }
    }

    private void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTriggerName);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

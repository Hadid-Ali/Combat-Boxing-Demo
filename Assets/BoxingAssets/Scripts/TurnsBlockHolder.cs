using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnsBlockHolder : MonoBehaviour
{
    [SerializeField] GameObject blockObj;
    [SerializeField] List<Image> blocks;
    [SerializeField] Transform blockParent;

    public delegate void TurnsIncrementDelegate();
    public static event TurnsIncrementDelegate onTurnIncrement;
    private void OnEnable()
    {
        onTurnIncrement += InstantiateBlock;
    }
    private void OnDisable()
    {
        onTurnIncrement -= InstantiateBlock;
    }
    public static void CloneBlockOnTurnIncrement()
    {
        onTurnIncrement?.Invoke();
    }
    void InstantiateBlock()
    {
        GameObject g = Instantiate(blockObj, blockParent.position, Quaternion.identity);
        g.transform.SetParent(blockParent);
    }
}

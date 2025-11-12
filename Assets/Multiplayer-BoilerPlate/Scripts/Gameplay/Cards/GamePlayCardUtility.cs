using UnityEngine;

public class GamePlayCardUtility : MonoBehaviour
{
    [SerializeField] private Material mat;
    private Texture _startTex;

    private void Start()
    {
        _startTex = mat.mainTexture;
        mat.mainTexture = CardsRegistery.Instance.GetCardSprite(GameData.RuntimeData.SELECTED_CARD_BACK).texture;
    }

    private void OnDestroy()
    {
        mat.mainTexture = _startTex;
    }
}

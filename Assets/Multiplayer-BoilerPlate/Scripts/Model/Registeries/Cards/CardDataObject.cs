using UnityEngine;

[CreateAssetMenu(fileName = "Card Object",menuName = "Game Data/Card Data/Create")]
public class CardDataObject : ScriptableObject
{
    [field: SerializeField] public CardType type = CardType.TYPE_HEARTS;
    [field: SerializeField] public Cardvalue value = Cardvalue.value_2;
	
    [field: SerializeField] public Sprite CardImage;
}

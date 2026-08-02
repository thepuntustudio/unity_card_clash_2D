using UnityEngine;

public enum CardType { Attack, Block, Heal, PowerStrike }

[CreateAssetMenu(fileName = "NewCard", menuName = "CardClash/Card Data")]
public class CardData : ScriptableObject
{
    public Sprite cardIcon;
    public Color cardBorderColor = Color.white;
    public string cardName;
    public CardType type;
    public int value;       // damage, shield, or heal amount
    [TextArea] public string flavorText; // handy later for Phase 6
}
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "CardClash/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHP;
    public int attackDamage;
    public Sprite enemySprite;
    public Vector2 displaySize = new Vector2(400f, 400f); // width/height in UI units — this replaces manually resizing 3 separate GameObjects
    [TextArea] public string introLine;
    public bool isBoss = false;
    [Range(0f, 1f)] public float bossTelegraphChance = 0.4f; // bosses telegraph more often

    public AudioClip hurtSFX;
    public AudioClip deathSFX;
    public AudioClip appearSFX;
}
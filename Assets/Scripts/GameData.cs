using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public string currentLocationKey;
    public bool hasSelectedCharacter = false; // see below

    public string playerName = "Hero";
    public Sprite selectedCharacterSprite;
    public bool isFemaleKnight;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    } 
}
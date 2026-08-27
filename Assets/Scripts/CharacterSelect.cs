using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterSelect : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Sprite maleKnightSprite;
    public Sprite femaleKnightSprite;
    public Button startButton;
    public TMP_Text warningText; // "Please select a character" — hidden by default
    public AudioSource sfxSource;  

    public CharacterButtonState maleButtonState;
    public CharacterButtonState femaleButtonState;

    private Sprite chosenSprite;
    private bool hasChosenCharacter = false;

    void Start()
{
    if (warningText != null) warningText.gameObject.SetActive(false);

    if (GameData.Instance != null && GameData.Instance.hasSelectedCharacter)
    {
        nameInput.text = GameData.Instance.playerName;
        chosenSprite = GameData.Instance.selectedCharacterSprite;
        hasChosenCharacter = true;

        bool wasFemale = GameData.Instance.isFemaleKnight;
        maleButtonState.SetSelected(!wasFemale);
        femaleButtonState.SetSelected(wasFemale);
    }
}

    public void SelectMale()
{
    chosenSprite = maleKnightSprite;
    hasChosenCharacter = true;
    maleButtonState.SetSelected(true);
    femaleButtonState.SetSelected(false);
}

    public void SelectFemale()
{
    chosenSprite = femaleKnightSprite;
    hasChosenCharacter = true;
    femaleButtonState.SetSelected(true);
    maleButtonState.SetSelected(false);
}
    

    public void OnStartJourney()
    {
        if (!hasChosenCharacter)
        {
            ShowWarning("Please choose a character first!");
            return;
        }
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            ShowWarning("Please enter your name!");
            return;
        }

        HideWarning();
        GameData.Instance.playerName = nameInput.text.Trim();
        GameData.Instance.selectedCharacterSprite = chosenSprite;
        GameData.Instance.isFemaleKnight = (chosenSprite == femaleKnightSprite);
        GameData.Instance.hasSelectedCharacter = true; // <- the missing piece

        SceneManager.LoadScene("2_CampaignMap");
    }

void ShowWarning(string message)
{
    if (warningText == null) return;
    warningText.text = message;
    warningText.gameObject.SetActive(true);
}

void HideWarning()
{
    if (warningText != null) warningText.gameObject.SetActive(false);
}

public void OnBack()
    {
        SceneManager.LoadScene("0_MainMenu");
    }

}
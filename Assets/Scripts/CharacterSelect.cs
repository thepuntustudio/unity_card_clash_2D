using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Sprite maleKnightSprite;
    public Sprite femaleKnightSprite;
    public Image maleSelectHighlight;   // simple border/glow Image, toggled active
    public Image femaleSelectHighlight;
    public Button startButton;
    public TMP_Text warningText; // "Please select a character" — hidden by default

    private Sprite chosenSprite;
    private bool hasChosenCharacter = false;

    void Start()
    {
        if (warningText != null) warningText.gameObject.SetActive(false);

        // Pre-fill if the player already chose a character this session (e.g. came back via Back button)
        if (GameData.Instance != null && GameData.Instance.hasSelectedCharacter)
        {
            nameInput.text = GameData.Instance.playerName;
            chosenSprite = GameData.Instance.selectedCharacterSprite;
            hasChosenCharacter = true;

            // Re-show the correct highlight based on which was picked
            bool wasFemale = GameData.Instance.isFemaleKnight;
            maleSelectHighlight.gameObject.SetActive(!wasFemale);
            femaleSelectHighlight.gameObject.SetActive(wasFemale);
        }
    }

    public void SelectMale()
    {
        chosenSprite = maleKnightSprite;
        hasChosenCharacter = true;
        maleSelectHighlight.gameObject.SetActive(true);
        femaleSelectHighlight.gameObject.SetActive(false);
    }

    public void SelectFemale()
    {
        chosenSprite = femaleKnightSprite;
        hasChosenCharacter = true;
        femaleSelectHighlight.gameObject.SetActive(true);
        maleSelectHighlight.gameObject.SetActive(false);
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
}
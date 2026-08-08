using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class CinematicSlide
{
    public Sprite image;
    [TextArea] public string text;
}

public class CinematicController : MonoBehaviour
{
    public Image backgroundImage;
    public TMP_Text storyText;
    public CinematicSlide[] slides;
    public float charDelay = 0.03f;
    public float holdAfterTyping = 1.5f;
    public string nextSceneName = "BattleScene1";

    private int currentSlide = 0;
    private bool isTyping = false;
    private Coroutine typingRoutine;

    void Start()
    {
        StartCoroutine(PlayCinematic());
    }

    IEnumerator PlayCinematic()
    {
        for (currentSlide = 0; currentSlide < slides.Length; currentSlide++)
        {
            backgroundImage.sprite = slides[currentSlide].image;
            typingRoutine = StartCoroutine(TypeText(slides[currentSlide].text));
            yield return typingRoutine;
            yield return new WaitForSeconds(holdAfterTyping);
        }
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        storyText.text = "";
        foreach (char c in fullText)
        {
            storyText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
        isTyping = false;
    }

    // Optional: let the player tap/click to skip ahead through the typing or slide
    public void OnScreenTapped()
    {
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            storyText.text = slides[currentSlide].text; // snap to full text
            isTyping = false;
        }
    }
}
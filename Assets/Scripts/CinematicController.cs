using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CinematicController : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;
    public TMP_Text storyText;

    [Header("Cinematic Slides")]
    public CinematicSlide[] slides;

    [Header("Typing Settings")]
    public float charDelay = 0.03f;
    public float autoAdvanceDelay = 1.5f;

    [Header("Scene")]
    public string nextSceneName = "CampaignMap";

    private int currentSlide = 0;

    private bool isTyping = false;
    private bool textCompleted = false;
    private bool isAdvancing = false;

    private Coroutine typingRoutine;
    private Coroutine autoAdvanceRoutine;


    void Start()
    {
        if (slides == null || slides.Length == 0)
        {
            Debug.LogError("CinematicController: No slides assigned!");
            return;
        }

        ShowSlide(currentSlide);
    }


    void ShowSlide(int slideIndex)
    {
        if (slideIndex >= slides.Length)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // Set background image
        backgroundImage.sprite = slides[slideIndex].image;

        // Reset states
        isTyping = true;
        textCompleted = false;
        isAdvancing = false;

        // Start typing
        typingRoutine = StartCoroutine(TypeText(slides[slideIndex].text));
    }


    IEnumerator TypeText(string fullText)
    {
        storyText.text = "";

        foreach (char c in fullText)
        {
            storyText.text += c;

            yield return new WaitForSeconds(charDelay);
        }

        // Text finished naturally
        isTyping = false;
        textCompleted = true;

        // Start automatic advance timer
        StartAutoAdvance();
    }


    void StartAutoAdvance()
    {
        // Make sure there isn't already a timer running
        if (autoAdvanceRoutine != null)
        {
            StopCoroutine(autoAdvanceRoutine);
        }

        autoAdvanceRoutine = StartCoroutine(AutoAdvance());
    }


    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);

        if (!isAdvancing)
        {
            NextSlide();
        }
    }


    // Connect this function to your UI Button's OnClick()
    public void OnScreenTapped()
    {
        // ---------------------------------------
        // TAP WHILE TEXT IS TYPING
        // ---------------------------------------
        if (isTyping)
        {
            // Stop typewriter
            if (typingRoutine != null)
            {
                StopCoroutine(typingRoutine);
            }

            // Show complete text
            storyText.text = slides[currentSlide].text;

            // Update state
            isTyping = false;
            textCompleted = true;

            // Start the 1.5 second auto advance timer
            StartAutoAdvance();

            return;
        }


        // ---------------------------------------
        // TAP AFTER TEXT IS COMPLETE
        // ---------------------------------------
        if (textCompleted)
        {
            // Cancel automatic timer
            if (autoAdvanceRoutine != null)
            {
                StopCoroutine(autoAdvanceRoutine);
                autoAdvanceRoutine = null;
            }

            // Immediately go to next slide
            NextSlide();
        }
    }


    void NextSlide()
    {
        // Prevent double advancing
        if (isAdvancing)
            return;

        isAdvancing = true;

        // Stop any running timer
        if (autoAdvanceRoutine != null)
        {
            StopCoroutine(autoAdvanceRoutine);
            autoAdvanceRoutine = null;
        }

        currentSlide++;

        // Check if all slides are finished
        if (currentSlide >= slides.Length)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // Show next slide
        ShowSlide(currentSlide);
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;
    private AudioSource sfxSource;

    void Start()
    {
        //check for GameManage instance if not found check for SceneHandler instance
        if (FindFirstObjectByType<GameManager>() != null)
        {
            sfxSource = FindFirstObjectByType<GameManager>().sfxSource;
        }
        else if (FindFirstObjectByType<SceneHandler>() != null)
        {
            sfxSource = FindFirstObjectByType<SceneHandler>().sfxSource;
        }
        //sfxSource = FindFirstObjectByType<GameManager>().GetComponent<AudioSource>();
        // If you have multiple AudioSources on GameManager, instead expose a public
        // reference there and grab it explicitly rather than relying on GetComponent order.
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null) sfxSource.PlayOneShot(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip != null) sfxSource.PlayOneShot(clickClip);
    }
}
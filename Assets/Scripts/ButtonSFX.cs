using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip;
    public AudioClip clickClip;
    private AudioSource sfxSource;

    void Start()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            sfxSource = gm.sfxSource;
        }
        else if (FindFirstObjectByType<SceneHandler>() != null)
        {
            SceneHandler sh = FindFirstObjectByType<SceneHandler>();
            if (sh != null) sfxSource = sh.sfxSource;
        }
        else
        {
            CharacterSelect cs = FindFirstObjectByType<CharacterSelect>();
            if (cs != null) sfxSource = cs.sfxSource;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sfxSource != null && hoverClip != null) sfxSource.PlayOneShot(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
    }
}
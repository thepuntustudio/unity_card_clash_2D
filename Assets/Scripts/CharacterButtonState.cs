using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterButtonState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetImage; // the button's own background Image (frame)
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.92f, 0.7f); // soft gold tint
    public Color selectedColor = new Color(1f, 0.8f, 0.3f); // stronger gold

    public float normalScale = 1f;
    public float hoverScale = 1.08f;
    public float selectedScale = 1.05f;

    private bool isSelected = false;
    private bool isHovering = false;

    void Start()
    {
        UpdateVisual();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        UpdateVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (isSelected)
        {
            targetImage.color = selectedColor;
            transform.localScale = Vector3.one * selectedScale;
        }
        else if (isHovering)
        {
            targetImage.color = hoverColor;
            transform.localScale = Vector3.one * hoverScale;
        }
        else
        {
            targetImage.color = normalColor;
            transform.localScale = Vector3.one * normalScale;
        }
    }
}
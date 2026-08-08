using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 40f; // now in UI units/sec, consistent regardless of canvas scale
    public float fadeDuration = 1f;
    private TextMeshProUGUI text;
    private RectTransform rt;
    private float elapsed = 0f;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        rt = GetComponent<RectTransform>();
    }

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = color;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        rt.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
        Color c = text.color;
        c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
        text.color = c;
        if (elapsed >= fadeDuration) Destroy(gameObject);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class KingHealthBar : MonoBehaviour
{
    public Image fillBar;
    public King king;

    private RectTransform fillRect;
    private float maxWidth;

    void Start()
    {
        king = FindAnyObjectByType<King>();
        fillRect = fillBar.GetComponent<RectTransform>();
        maxWidth = fillRect.rect.width;
        if (king == null)
            Debug.LogWarning("KingHealthBar: No King found!");
    }

    void Update()
    {
        if (fillRect == null) return;

        float ratio = king != null ? Mathf.Clamp01(king.health / king.maxHealth) : 0f;

        // Scale the fill bar width based on health
        fillRect.localScale = new Vector3(ratio, 1f, 1f);
        // Keep it anchored to the left by adjusting pivot
        fillRect.pivot = new Vector2(0f, 0.5f);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TroopCard : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Button button;

    private TroopData data;
    private Action<TroopData> onClickCallback;

    public void Setup(TroopData troopData, Action<TroopData> onClick)
    {
        data = troopData;
        onClickCallback = onClick;

        if (nameText != null) nameText.text = troopData.name;
        if (costText != null) costText.text = $"{troopData.goldCost}g";
        if (iconImage != null && troopData.icon != null)
            iconImage.sprite = troopData.icon;

        // Disable raycast on text children so they don't block button clicks
        foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>())
            tmp.raycastTarget = false;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log($"Clicked: {troopData.name}");
            onClickCallback?.Invoke(data);
        });
    }
}
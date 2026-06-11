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

        nameText.text = troopData.name;
        costText.text = $"{troopData.goldCost}g";

        if (troopData.icon != null)
            iconImage.sprite = troopData.icon;

        button.onClick.AddListener(() => onClickCallback?.Invoke(data));
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreparationUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI goldText;
    public Transform troopCardContainer;
    public GameObject troopCardPrefab;
    public GameObject attackButtonObject;
    public GameObject undoButtonObject;

    [Header("Troop Definitions")]
    public List<TroopData> availableTroops;

    private Button attackButton;
    private Button undoButton;

    void Start()
    {
        Debug.Log("PreparationUI Start called");

        if (attackButtonObject == null) { Debug.LogError("attackButtonObject is null!"); return; }
        if (undoButtonObject == null)   { Debug.LogError("undoButtonObject is null!"); return; }

        attackButton = attackButtonObject.GetComponent<Button>();
        undoButton   = undoButtonObject.GetComponent<Button>();

        if (attackButton == null) { Debug.LogError("AttackButton has no Button component!"); return; }
        if (undoButton == null)   { Debug.LogError("UndoButton has no Button component!"); return; }

        attackButton.onClick.AddListener(OnAttackPressed);
        undoButton.onClick.AddListener(OnUndoPressed);

        Debug.Log($"Building {availableTroops.Count} troop cards");
        BuildTroopCards();
        UpdateGoldDisplay();
    }

    void BuildTroopCards()
    {
        if (troopCardPrefab == null) { Debug.LogError("troopCardPrefab is null!"); return; }
        if (troopCardContainer == null) { Debug.LogError("troopCardContainer is null!"); return; }

        foreach (var data in availableTroops)
        {
            if (data.prefab == null) { Debug.LogWarning($"Troop '{data.name}' has no prefab!"); continue; }

            GameObject card = Instantiate(troopCardPrefab, troopCardContainer);
            TroopCard cardScript = card.GetComponent<TroopCard>();

            if (cardScript == null) { Debug.LogError("Spawned card has no TroopCard script!"); continue; }

            cardScript.Setup(data, OnTroopCardPressed);
        }
    }

    void OnTroopCardPressed(TroopData data)
    {
        bool added = GameManager.Instance.TryAddTroop(data.prefab, data.goldCost);
        if (added) UpdateGoldDisplay();
        else Debug.Log("Not enough gold!");
    }

    void OnAttackPressed()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartExecution();
    }

    void OnUndoPressed()
    {
        var queue = GameManager.Instance.GetQueue();
        if (queue.Count == 0) return;

        var lastPrefab = queue[queue.Count - 1];
        int cost = availableTroops.Find(t => t.prefab == lastPrefab).goldCost;
        GameManager.Instance.RemoveLastTroop(cost);
        UpdateGoldDisplay();
    }

    void UpdateGoldDisplay()
    {
        if (goldText != null && GameManager.Instance != null)
            goldText.text = $"Gold: {GameManager.Instance.currentGold}";
    }
}

[System.Serializable]
public class TroopData
{
    public string name;
    public int goldCost;
    public Sprite icon;
    public GameObject prefab;
}
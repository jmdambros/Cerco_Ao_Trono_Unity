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
    public GameObject attackButtonObject;   // drag AttackButton here
    public GameObject undoButtonObject;     // drag UndoButton here

    [Header("Troop Definitions")]
    public List<TroopData> availableTroops;

    private Button attackButton;
    private Button undoButton;

    void Start()
    {
        // Get Button components automatically from the GameObjects
        attackButton = attackButtonObject.GetComponent<Button>();
        undoButton = undoButtonObject.GetComponent<Button>();

        attackButton.onClick.AddListener(OnAttackPressed);
        undoButton.onClick.AddListener(OnUndoPressed);

        BuildTroopCards();
        UpdateGoldDisplay();
    }

    void Update()
    {
        UpdateGoldDisplay();
    }

    void BuildTroopCards()
    {
        foreach (var data in availableTroops)
        {
            GameObject card = Instantiate(troopCardPrefab, troopCardContainer);
            TroopCard cardScript = card.GetComponent<TroopCard>();
            if (cardScript != null)
                cardScript.Setup(data, OnTroopCardPressed);
        }
    }

    void OnTroopCardPressed(TroopData data)
    {
        bool added = GameManager.Instance.TryAddTroop(data.prefab, data.goldCost);
        if (added)
            UpdateGoldDisplay();
        else
            Debug.Log("Not enough gold!");
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
        int cost = lastPrefab.GetComponent<Troop>().goldCost;
        GameManager.Instance.RemoveLastTroop(cost);
        UpdateGoldDisplay();
    }

    void UpdateGoldDisplay()
    {
        if (goldText != null)
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
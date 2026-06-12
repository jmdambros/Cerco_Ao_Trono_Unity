using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Budget")]
    public int startingGold = 1000;
    public int currentGold;

    [Header("Spawn Point")]
    public Transform troopSpawnPoint;

    [Header("State")]
    public GamePhase currentPhase = GamePhase.Preparation;

    private List<GameObject> troopQueue = new List<GameObject>();
    private Troop activeTroop = null;
    private int troopIndex = 0;
    private King king;

    public enum GamePhase { Preparation, Execution, Victory, Defeat }

    void Awake()
    {
        Instance = this;
        currentGold = startingGold;
    }

    void Start()
    {
        // Find king at start
        king = FindAnyObjectByType<King>();
        if (king == null)
            Debug.LogWarning("No King found in scene! Place a King prefab.");
        else
            Debug.Log("King found: " + king.name);
    }

    public bool TryAddTroop(GameObject troopPrefab, int cost)
    {
        if (currentPhase != GamePhase.Preparation) return false;
        if (currentGold < cost) return false;

        currentGold -= cost;
        troopQueue.Add(troopPrefab);
        Debug.Log($"Added {troopPrefab.name} to queue. Gold left: {currentGold}");
        return true;
    }

    public void StartExecution()
    {
        if (troopQueue.Count == 0)
        {
            Debug.LogWarning("No troops selected!");
            return;
        }

        currentPhase = GamePhase.Execution;
        troopIndex = 0;
        SpawnNextTroop();
    }

    void SpawnNextTroop()
    {
        if (troopIndex >= troopQueue.Count)
        {
            StartCoroutine(CheckForDefeat());
            return;
        }

        GameObject prefab = troopQueue[troopIndex];
        GameObject obj = Instantiate(prefab, troopSpawnPoint.position, Quaternion.identity);
        activeTroop = obj.GetComponent<Troop>();
        activeTroop.Activate();
        troopIndex++;
        StartCoroutine(WaitForTroopToFinish());
    }

    IEnumerator WaitForTroopToFinish()
    {
        yield return new WaitUntil(() => activeTroop == null || activeTroop.HasLaunched);
        yield return new WaitForSeconds(2f);
        if (currentPhase == GamePhase.Execution)
            SpawnNextTroop();
    }

    IEnumerator CheckForDefeat()
    {
        yield return new WaitForSeconds(3f);
        if (currentPhase == GamePhase.Execution)
        {
            currentPhase = GamePhase.Defeat;
            Debug.Log("DEFEAT - The King survives!");
        }
    }

    // Called directly by King.Die()
    public void CheckForVictory()
    {
        currentPhase = GamePhase.Victory;
        Debug.Log("VICTORY - The King is dethroned!");
        // TODO: show victory screen
    }

    public void RemoveLastTroop(int refundAmount)
    {
        if (troopQueue.Count == 0) return;
        troopQueue.RemoveAt(troopQueue.Count - 1);
        currentGold += refundAmount;
    }

    public int GetQueueCount() => troopQueue.Count;
    public List<GameObject> GetQueue() => troopQueue;
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Attach this to an empty GameObject called "GameManager" in your scene.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Budget")]
    public int startingGold = 1000;
    public int currentGold;

    [Header("Spawn Point")]
    public Transform troopSpawnPoint;   // empty GameObject on the left, where troops appear

    [Header("State")]
    public GamePhase currentPhase = GamePhase.Preparation;

    // The ordered queue of troop prefabs the player has selected
    private List<GameObject> troopQueue = new List<GameObject>();
    private Troop activeTroop = null;
    private int troopIndex = 0;

    public enum GamePhase { Preparation, Execution, Victory, Defeat }

    void Awake()
    {
        Instance = this;
        currentGold = startingGold;
    }

    // Called by UI buttons when player clicks a troop card
    public bool TryAddTroop(GameObject troopPrefab, int cost)
    {
        if (currentPhase != GamePhase.Preparation) return false;
        if (currentGold < cost) return false;

        currentGold -= cost;
        troopQueue.Add(troopPrefab);

        Debug.Log($"Added {troopPrefab.name} to queue. Gold left: {currentGold}");
        return true;
    }

    // Called by UI when player clicks "ATTACK" button
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
        activeTroop.Activate();     // enables input on this troop

        troopIndex++;
        StartCoroutine(WaitForTroopToFinish());
    }

    IEnumerator WaitForTroopToFinish()
    {
        // Wait until the troop has launched
        yield return new WaitUntil(() => activeTroop == null || activeTroop.HasLaunched);

        // Then wait a couple seconds for physics to settle before next troop
        yield return new WaitForSeconds(2f);

        // Check win condition before spawning next
        if (CheckForVictory()) yield break;

        SpawnNextTroop();
    }

    IEnumerator CheckForDefeat()
    {
        yield return new WaitForSeconds(3f);    // let last troop physics settle
        if (currentPhase == GamePhase.Execution)
        {
            if (!CheckForVictory())
            {
                currentPhase = GamePhase.Defeat;
                Debug.Log("DEFEAT - The King survives!");
                // TODO: show defeat screen
            }
        }
    }

    bool CheckForVictory()
    {
        // Victory = no object tagged "King" remains in the scene
        GameObject king = GameObject.FindWithTag("King");
        if (king == null)
        {
            currentPhase = GamePhase.Victory;
            Debug.Log("VICTORY - The King is dethroned!");
            // TODO: show victory screen
            return true;
        }
        return false;
    }

    // Called by PreparationUI to remove last added troop (undo)
    public void RemoveLastTroop(int refundAmount)
    {
        if (troopQueue.Count == 0) return;
        troopQueue.RemoveAt(troopQueue.Count - 1);
        currentGold += refundAmount;
    }

    public int GetQueueCount() => troopQueue.Count;
    public List<GameObject> GetQueue() => troopQueue;
}
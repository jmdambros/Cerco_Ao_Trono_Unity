using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Stars")]
    public GameObject star1;    // assign 3 star GameObjects in Inspector
    public GameObject star2;
    public GameObject star3;
    public TextMeshProUGUI starsLabel; // optional e.g. "2 / 3 stars"

    void Start()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    public void ShowVictory(int stars)
    {
        victoryPanel.SetActive(true);
        UpdateStars(stars);
    }

    // kept for safety in case anything calls the old signature
    public void ShowVictory() => ShowVictory(1);

    public void ShowDefeat()
    {
        defeatPanel.SetActive(true);
    }

    private void UpdateStars(int stars)
    {
        if (star1 != null) star1.SetActive(stars >= 1);
        if (star2 != null) star2.SetActive(stars >= 2);
        if (star3 != null) star3.SetActive(stars >= 3);

        if (starsLabel != null)
            starsLabel.text = $"{stars} / 3";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // Called by Play button
    public void OnPlayPressed()
    {
        SceneManager.LoadScene("MapaFases");
    }

    // Called by Credits button
    public void OnCreditsPressed()
    {
        // For now just log - we can add a credits panel later
        Debug.Log("Credits pressed!");
        // TODO: show credits panel
    }
}
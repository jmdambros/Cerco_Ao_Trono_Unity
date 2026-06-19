using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapaFasesUI : MonoBehaviour
{
    // Called by each level button on the map
    public void LoadFase(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Called by back button
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
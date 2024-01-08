using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    [Header("De scene instellingen")]
    [Tooltip("De namen van de scene's")] public string[] SceneStrings;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneStrings[0]);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

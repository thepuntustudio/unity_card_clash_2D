using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public AudioSource sfxSource;   
     
    // Load a scene by its build index
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Exit the game
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

        // Works in a built game
        Application.Quit();

        // Stops Play Mode in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
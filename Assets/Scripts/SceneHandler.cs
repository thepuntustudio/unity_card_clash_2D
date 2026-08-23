using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public AudioSource sfxSource;   
    public GameObject ExitPanel;

    void Start()
    {
        if(ExitPanel.activeSelf)
        ExitPanel.SetActive(false);
    }

     
    // Load a scene by its build index
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }


    public void OnExitClicked()
    {
        Debug.Log("Exit button cliced");
        if(!ExitPanel.activeSelf)
        ExitPanel.SetActive(true);
    }

    public void OnConfirmExitNo()
    {
        if (ExitPanel.activeSelf)
        ExitPanel.SetActive(false);
    }

    // Exit the game
    public void OnConfirmExitYes()
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
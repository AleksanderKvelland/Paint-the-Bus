using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public string gameScene;

    public void PlayGame()
    {
        SceneManager.LoadScene(gameScene);

    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");   
        Application.Quit();       
    }
}

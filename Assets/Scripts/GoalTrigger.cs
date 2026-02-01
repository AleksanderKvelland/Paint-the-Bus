using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject finishScreen;
    
    private bool hasTriggered = false;

    void Start()
    {
        if (finishScreen != null)
        {
            finishScreen.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        // Check if the collider is the bus
        if (other.CompareTag("Bus") || other.name.Contains("Bus"))
        {
            TriggerFinish();
        }
    }

    private void TriggerFinish()
    {
        hasTriggered = true;
        Debug.Log("Goal reached! Triggering finish screen.");

        if (finishScreen != null)
        {
            ShowFinishScreen();
        }
        else
        {
            Debug.LogWarning("No finish screen assigned! Pausing game instead.");
            Time.timeScale = 0f;
        }
    }

    private void ShowFinishScreen()
    {
        Debug.Log("Displaying finish screen and pausing game.");
        finishScreen.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

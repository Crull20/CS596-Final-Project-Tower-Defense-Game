using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        gameObject.SetActive(false); // Disable pause menu
        Time.timeScale = 1f; // Set game speed to normal
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Set game speed to normal before reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitMainMenu()
    {
        // TODO: implement title screen if time permits
    }
}

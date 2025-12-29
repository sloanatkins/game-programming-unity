using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{
    // This function will be called by the Play button.
    public void PlayGame()
    {
        // Load the scene named "Bird Scene".
        Debug.Log("PlayGame button clicked!");
        SceneManager.LoadScene("Bird Scene");
    }
}

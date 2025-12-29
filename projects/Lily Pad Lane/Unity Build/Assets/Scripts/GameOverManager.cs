using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Animator gameOverAnimator;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (gameOverAnimator == null)
        {
            gameOverAnimator = GetComponent<Animator>();
        }
    }

    public void ShowGameOver()
    {
        if (gameOverAnimator != null)
        {
            gameOverAnimator.SetTrigger("FadeIn");
        }

        if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true); // show it directly
            }

    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

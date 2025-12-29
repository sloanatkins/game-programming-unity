using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;
    public AudioSource pointSFX;

    public void addScore(int scoreToAdd){
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        pointSFX.Play(); // Ensure pointSFX is properly assigned in the Inspector
    }

    public void restartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver() {
        gameOverScreen.SetActive(true);
    }
}

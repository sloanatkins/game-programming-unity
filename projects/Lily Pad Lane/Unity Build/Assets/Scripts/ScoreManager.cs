using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text scoreText;
    public TMP_Text topScoreText;

    private int currentScore = 0;
    private int topScore = 0;
    private int farthestRowReached = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Load personal top score for the currently logged-in user
        string currentUser = PlayerPrefs.GetString("currentUser", "Guest");
        topScore = PlayerPrefs.GetInt($"TopScore_{currentUser}", 0);
        UpdateUI();
    }

    public void CheckForScoreIncrease(float frogYPosition)
    {
        float rowHeight = 2.0429f;
        int currentRow = Mathf.RoundToInt(frogYPosition / rowHeight);

        if (currentRow > farthestRowReached)
        {
            currentScore += 1;
            farthestRowReached = currentRow;

            if (scoreText != null)
                scoreText.text = currentScore.ToString();
        }
    }

    public void ResetScore()
    {
        float rowHeight = 2.0429f;
        currentScore = 0;

        float startY = FrogController.instance.transform.position.y;
        farthestRowReached = Mathf.FloorToInt(startY / rowHeight);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();

        if (topScoreText != null)
            topScoreText.text = "High Score: " + topScore;
    }

    public void SaveCurrentScoreToLeaderboard()
    {
        int newScore = currentScore;
        string username = PlayerPrefs.GetString("currentUser", "Guest");

        // Update user's personal best
        int personalBest = PlayerPrefs.GetInt($"TopScore_{username}", 0);
        if (newScore > personalBest)
        {
            PlayerPrefs.SetInt($"TopScore_{username}", newScore);
            topScore = newScore;
        }
        else
        {
            newScore = personalBest;
        }

        // Load leaderboard
        int[] scores = new int[5];
        string[] names = new string[5];

        for (int i = 0; i < 5; i++)
        {
            scores[i] = PlayerPrefs.GetInt($"Score_{i}", 0);
            names[i] = PlayerPrefs.GetString($"Username_{i}", "-");
        }

        bool found = false;
        for (int i = 0; i < 5; i++)
        {
            if (names[i] == username)
            {
                found = true;
                if (newScore > scores[i])
                {
                    scores[i] = newScore;
                }
                else
                {
                    newScore = scores[i]; // keep highest
                }
            }
        }

        if (!found)
        {
            if (newScore > scores[4])
            {
                names[4] = username;
                scores[4] = newScore;
            }
            else
            {
                return; // Score not high enough to enter leaderboard
            }
        }

        //NOW: Sort leaderboard descending by score
        for (int i = 0; i < 5 - 1; i++)
        {
            for (int j = i + 1; j < 5; j++)
            {
                if (scores[j] > scores[i])
                {
                    int tempScore = scores[i];
                    scores[i] = scores[j];
                    scores[j] = tempScore;

                    string tempName = names[i];
                    names[i] = names[j];
                    names[j] = tempName;
                }
            }
        }

        // Save leaderboard
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt($"Score_{i}", scores[i]);
            PlayerPrefs.SetString($"Username_{i}", names[i]);
        }

        PlayerPrefs.Save();
        UpdateUI();
    }



}

using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string username;
        public int score;
    }

    public LeaderboardEntry[] leaderboardEntries;

    public TMP_Text headerUsernameText;
    public TMP_Text headerScoreText;

    public TMP_Text[] usernameTexts; // 5 slots
    public TMP_Text[] scoreTexts;    // 5 slots

    void Start()
    {
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        headerUsernameText.text = "Username";
        headerScoreText.text = "Score";

        for (int i = 0; i < usernameTexts.Length; i++)
        {
            string loadedUsername = PlayerPrefs.GetString($"Username_{i}", "-");
            int loadedScore = PlayerPrefs.GetInt($"Score_{i}", 0);

            usernameTexts[i].text = loadedUsername;
            scoreTexts[i].text = loadedScore.ToString();
        }
    }

}

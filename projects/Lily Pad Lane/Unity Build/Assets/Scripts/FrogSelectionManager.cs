using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FrogSelectionManager : MonoBehaviour
{
    public GameObject starterFrog, midFrog, topFrog; // frog visuals
    public Button starterButton, midButton, topButton;
    public TMP_Text starterText, midText, topText;

    private string currentUser;



    void Start()
    {
        currentUser = PlayerPrefs.GetString("currentUser", "Guest");
        UpdateFrogUI();

        // Force select starter frog if nothing saved yet
        if (!PlayerPrefs.HasKey($"selectedFrog_{currentUser}"))
        {
            PlayerPrefs.SetString($"selectedFrog_{currentUser}", "starter");
            PlayerPrefs.Save();
        }

    }

    public void SelectFrog(string frog)
    {
        bool isUnlocked = frog == "starter" || PlayerPrefs.GetInt($"frog_{frog}_unlocked", 0) == 1;

        if (!isUnlocked)
        {
            TryPurchase(frog);
            return;
        }

        PlayerPrefs.SetString($"selectedFrog_{currentUser}", frog);
        PlayerPrefs.Save();

        UpdateFrogUI();
    }

    void TryPurchase(string frog)
    {
        int cost = frog == "mid" ? 100 : 300;
        int coins = PlayerPrefs.GetInt($"{currentUser}_coins", 0);

        if (coins >= cost)
        {
            PlayerPrefs.SetInt($"coins_{currentUser}", coins - cost);
            PlayerPrefs.SetInt($"frog_{frog}_unlocked", 1);
            PlayerPrefs.SetString($"selectedFrog_{currentUser}", frog);
            PlayerPrefs.Save();

            CoinManager.instance.AnimateCoinRefresh(coins - cost);

            UpdateFrogUI();
        }
        else
        {
            // show a warning
            Debug.Log("❌ Insufficient coins to unlock this frog.");
            ShowInsufficientFundsWarning(); // Implement below
        }
    }


    void UpdateFrogUI()
    {

        string selected = PlayerPrefs.GetString($"selectedFrog_{currentUser}", "starter");

        bool midUnlocked = PlayerPrefs.GetInt("frog_mid_unlocked", 0) == 1;
        bool topUnlocked = PlayerPrefs.GetInt("frog_top_unlocked", 0) == 1;

        // Starter frog is always unlocked
        starterText.text = selected == "starter" ? "Selected" : "Select";
        midText.text = midUnlocked ? (selected == "mid" ? "Selected" : "Select") : "Buy: 100";
        topText.text = topUnlocked ? (selected == "top" ? "Selected" : "Select") : "Buy: 300";

        // Highlight selected button
        HighlightButton(starterButton, selected == "starter");
        HighlightButton(midButton, selected == "mid");
        HighlightButton(topButton, selected == "top");


    }


    void HighlightButton(Button btn, bool isSelected)
    {
        ColorBlock colors = btn.colors;
        colors.normalColor = isSelected ? new Color(0.3f, 0.3f, 0.3f) : Color.white;
        btn.colors = colors;
    }

    [SerializeField] TMP_Text warningText;

    void ShowInsufficientFundsWarning()
    {
        if (warningText != null)
        {
            warningText.text = "Not enough coins!";
            warningText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideWarning)); // if already invoked
            Invoke(nameof(HideWarning), 2f);   // hide after 2 seconds
        }
    }

    void HideWarning()
    {
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

}

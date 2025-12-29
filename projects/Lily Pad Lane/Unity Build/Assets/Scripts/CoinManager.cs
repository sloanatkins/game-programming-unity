using UnityEngine;
using System.Collections;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public int coins = 0;
    public TMP_Text coinText;

    private string currentUser;
    public TMP_Text settingsCoinText;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentUser = PlayerPrefs.GetString("currentUser", ""); // Pull logged in user

        if (currentUser == "")
        {
            Debug.LogError("No user logged in! Cannot load coins.");
        }
        else
        {
              RefreshCoins();

        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinText();
        SaveCoins(); // Save after every update
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }

        if (settingsCoinText != null)
        {
            settingsCoinText.text = coins.ToString();
        }
    }


    void SaveCoins()
    {
        if (currentUser != "")
        {
            PlayerPrefs.SetInt(currentUser + "_coins", coins); // Save to user-specific key
            PlayerPrefs.Save();
        }
    }

    public void LoadCoins()
    {
        if (currentUser != "")
        {
            coins = PlayerPrefs.GetInt(currentUser + "_coins", 0); // Load from user-specific key
            UpdateCoinText();
        }
    }

    private Coroutine coinAnimationCoroutine;

    public void AnimateCoinRefresh(int targetCoins)
    {
        if (coinAnimationCoroutine != null)
        {
            StopCoroutine(coinAnimationCoroutine);
        }
        coinAnimationCoroutine = StartCoroutine(AnimateCoinsCoroutine(targetCoins));
    }

    private IEnumerator AnimateCoinsCoroutine(int targetCoins)
    {
        int startCoins = coins;
        float duration = 0.5f; // Total time to animate (half a second)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            coins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, t)); // Smooth interpolation
            UpdateCoinText(); // Update the UI every frame
            yield return null; // Wait one frame
        }

        coins = targetCoins; // Snap exactly to target at the end
        UpdateCoinText();
    }


    public void RefreshCoins()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("currentUser", "")))
        {
            int savedCoins = PlayerPrefs.GetInt(PlayerPrefs.GetString("currentUser") + "_coins", 0);
            AnimateCoinRefresh(savedCoins);
        }
    }

    public void CollectCoin(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt(currentUser + "_coins", coins);
        PlayerPrefs.Save();
        AnimateCoinRefresh(coins); // Animate ONLY after real pickup
    }

}

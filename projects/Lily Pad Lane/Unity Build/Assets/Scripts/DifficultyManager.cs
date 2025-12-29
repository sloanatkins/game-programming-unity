using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;

    public enum DifficultyLevel { Easy, Medium, Hard }
    public static DifficultyLevel currentDifficulty = DifficultyLevel.Easy;

    void Start()
    {
        // Load saved difficulty (defaults to "Easy" if not set)
        string savedDifficulty = PlayerPrefs.GetString("difficulty", "Easy");

        switch (savedDifficulty)
        {
            case "Medium":
                difficultyDropdown.value = 1;
                currentDifficulty = DifficultyLevel.Medium;
                break;
            case "Hard":
                difficultyDropdown.value = 2;
                currentDifficulty = DifficultyLevel.Hard;
                break;
            default:
                difficultyDropdown.value = 0;
                currentDifficulty = DifficultyLevel.Easy;
                break;
        }

        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
    }


    void OnDifficultyChanged(int index)
    {
        currentDifficulty = (DifficultyLevel)index;

        // Save to PlayerPrefs
        PlayerPrefs.SetString("difficulty", currentDifficulty.ToString());
        PlayerPrefs.Save();

        Debug.Log("Difficulty set to: " + currentDifficulty);
    }

}
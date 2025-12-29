using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public SettingsPageManager settingsPageManager;

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public Animator settingsAnimator;

    public void OpenSettings()
    {
        settingsAnimator.SetTrigger("OpenSettings");
        settingsPanel.SetActive(true);
        settingsPageManager.ResetPages();
    }

    public void CloseSettings()
    {
        settingsAnimator.SetTrigger("CloseSettings");
    }
}

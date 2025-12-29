using UnityEngine;

public class SettingsPageManager : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;

    private int currentPage = 1;

    void Start()
    {
        ResetPages(); // Ensure it's reset if active at start
    }

    public void ResetPages()
    {
        page1.SetActive(true);
        page2.SetActive(false);
        currentPage = 1;
    }

    public void GoRight()
    {
        if (currentPage == 1)
        {
            page1.SetActive(false);
            page2.SetActive(true);
            currentPage = 2;
        }
    }

    public void GoLeft()
    {
        if (currentPage == 2)
        {
            page2.SetActive(false);
            page1.SetActive(true);
            currentPage = 1;
        }
    }
}

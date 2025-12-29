using UnityEngine;
using UnityEngine.SceneManagement;

public class SignOutManager : MonoBehaviour
{
    public void SignOut()
    {
        // Clear the currentUser
        PlayerPrefs.DeleteKey("currentUser");
        PlayerPrefs.Save();

        // Load back to the Login Scene
        SceneManager.LoadScene("LoginScene");
    }
}

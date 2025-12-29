using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorText;

    private void Start()
    {
        errorText.gameObject.SetActive(false);
    }

    public void Login()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        // Clear the fields immediately after reading their values
        usernameInput.text = "";
        passwordInput.text = "";

        errorText.gameObject.SetActive(false);

        if (!PlayerPrefs.HasKey(username))
        {
            ShowError("Username not found!");
            return;
        }

        string savedPassword = PlayerPrefs.GetString(username);

        if (password != savedPassword)
        {
            ShowError("Password is incorrect!");
            return;
        }

        if (password == savedPassword)
        {
            Debug.Log("Login successful!");
            PlayerPrefs.SetString("currentUser", username);
            PlayerPrefs.Save();

            // Clear fields on success only
            usernameInput.text = "";
            passwordInput.text = "";

            SceneManager.LoadScene("MainMenu");
        }
    }


    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }


}

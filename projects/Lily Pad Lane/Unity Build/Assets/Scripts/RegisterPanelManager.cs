using UnityEngine;
using TMPro;

public class RegisterPanelManager : MonoBehaviour
{
    public Animator registerAnimator;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TextMeshProUGUI errorText;

    private void Start()
    {
        errorText.gameObject.SetActive(false);
    }

    public void OpenRegisterPanel()
    {
        ClearFields();
        registerAnimator.SetTrigger("OpenRegister");
        confirmPasswordInput.gameObject.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        registerAnimator.SetTrigger("CloseRegister");
    }

    public void Register()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;
        string confirm = confirmPasswordInput.text;

        errorText.gameObject.SetActive(false);

        // First Click: Hide password, reveal confirm field
        if (!confirmPasswordInput.gameObject.activeSelf)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please fill out all fields.");
                return;
            }

            passwordInput.gameObject.SetActive(false); // Hide password field
            confirmPasswordInput.gameObject.SetActive(true); // Show confirm password field
            return;
        }

        // Now, Second Click: Actually confirm and create account
        if (string.IsNullOrEmpty(confirm))
        {
            ShowError("Please confirm your password.");
            return;
        }

        if (password != confirm)
        {
            ShowError("Passwords do not match!");
            return;
        }

        if (PlayerPrefs.HasKey(username))
        {
            ShowError("Username already exists!");
            return;
        }

        // Passwords match, username is unique
        PlayerPrefs.SetString(username, password); // Save the real password
        PlayerPrefs.Save();

        ShowError("Account created successfully!");

        Invoke(nameof(CloseRegisterPanel), 1.5f);
    }




    private void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    private void ClearFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";

        errorText.gameObject.SetActive(false);

        // Make sure input fields reset to their default visibility
        passwordInput.gameObject.SetActive(true); // show password again
        confirmPasswordInput.gameObject.SetActive(false); // hide confirm password
    }

}

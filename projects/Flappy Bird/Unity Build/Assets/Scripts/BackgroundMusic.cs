using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource bgMusic;  // Assign your background music AudioSource in the Inspector
    private bool isMuted = false;

    void Start()
    {
        if (bgMusic != null)
        {
            bgMusic.Play();  // Start playing music at the beginning
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Toggle mute/unmute on pressing 'M'
        {
            isMuted = !isMuted;
            bgMusic.mute = isMuted;
        }
    }
}

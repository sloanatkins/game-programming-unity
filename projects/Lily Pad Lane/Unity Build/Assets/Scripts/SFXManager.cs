using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public Toggle musicToggle;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip jumpClip, coinClip, splashClip, carHitClip, clickClip, backgroundMusic;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Restore saved music state
        bool isMusicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        musicSource.mute = !isMusicOn;

        if (musicToggle != null)
        {
            musicToggle.isOn = isMusicOn;
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            PlayBackgroundMusic();
        }
        else
        {
            musicSource.Stop(); // Only music — NOT sfx
        }

        // Re-sync toggle
        if (musicToggle != null)
        {
            bool saved = PlayerPrefs.GetInt("Music", 1) == 1;
            musicToggle.isOn = saved;
            musicSource.mute = !saved;
        }
    }

    void OnMusicToggleChanged(bool isOn)
    {
        musicSource.mute = !isOn;
        PlayerPrefs.SetInt("Music", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayJump() => PlaySFX(jumpClip);
    public void PlayCoin() => PlaySFX(coinClip);
    public void PlaySplash() => PlaySFX(splashClip);
    public void PlayCarHit() => PlaySFX(carHitClip);
    public void PlayClick() => PlaySFX(clickClip);

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying && !musicSource.mute)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

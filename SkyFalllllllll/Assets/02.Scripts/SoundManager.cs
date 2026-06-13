using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM")]
    public AudioClip lobbyBGM;
    public AudioClip gameBGM;

    [Header("효과음")]
    public AudioClip gameOverSound;
    public AudioClip buttonSound;

    [Header("볼륨 설정")]
    public float bgmVolume = 0.5f;
    public float sfxVolume = 1.0f;
    public float bgmGameOverVolume = 0.1f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        // 현재 씬에 맞는 BGM 재생
        PlayBGMForCurrentScene();
    }

    // 현재 씬에 맞는 BGM 재생
    public void PlayBGMForCurrentScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (sceneName == "LobbyScene")
            PlayBGM(lobbyBGM);
        else
            PlayBGM(gameBGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip) return;  // 같은 BGM이면 재시작 안 함

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void DimBGM()
    {
        bgmSource.volume = bgmGameOverVolume;
    }

    public void RestoreBGM()
    {
        bgmSource.volume = bgmVolume;
    }

    public void PlayGameOverSound()
    {
        DimBGM();
        PlaySFX(gameOverSound);
    }

    public void PlayButtonSound()
    {
        PlaySFX(buttonSound);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
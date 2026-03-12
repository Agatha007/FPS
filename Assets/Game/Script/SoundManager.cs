using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    AudioSource bgmSource;
    AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            bgmSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // BGM 재생
    public void PlayBGM(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>(name);

        if (clip == null)
        {
            Debug.LogWarning("BGM 없음 : " + name);
            return;
        }

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // 효과음 재생
    public void PlaySFX(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>(name);

        if (clip == null)
        {
            Debug.LogWarning("SFX 없음 : " + name);
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}

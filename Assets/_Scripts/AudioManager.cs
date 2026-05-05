using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource; 

    [Header("Audio Clips")]
    [SerializeField] private AudioClip mainMenuBGM; 
    [SerializeField] private AudioClip loseSFX;

    private float defaultBGMVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (bgmSource != null) defaultBGMVolume = bgmSource.volume;
    }

    public void PlayMainMenuBGM()
    {
        if (bgmSource != null && mainMenuBGM != null)
        {
            if (bgmSource.clip != mainMenuBGM || !bgmSource.isPlaying)
            {
                bgmSource.clip = mainMenuBGM;
                bgmSource.volume = defaultBGMVolume;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
    }

    public void StopBGM(bool fadeOut = true, float fadeDuration = 1.5f)
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOutCoroutine(fadeDuration));
            }
            else
            {
                bgmSource.Stop();
            }
        }
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
        bgmSource.Stop();

        bgmSource.volume = startVolume;
    }

    public void PlayLoseSFX()
    {
        if (sfxSource != null && loseSFX != null)
        {
            sfxSource.PlayOneShot(loseSFX);
        }
    }
}

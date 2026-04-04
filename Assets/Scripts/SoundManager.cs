using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private float bgmVolume = 0.5f;
    [SerializeField] private float seVolume = 1.0f;
    [SerializeField] private Key toggleKey = Key.M;

    private AudioSource bgmSource;
    private AudioSource seSource;
    private bool bgmEnabled;

    public static SoundManager Instance { get; private set; }

    public static void EnsureExists()
    {
        if (Instance != null) return;

        var go = new GameObject("SoundManager");
        go.AddComponent<SoundManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (bgmClip == null)
            bgmClip = Resources.Load<AudioClip>("セツナイツバサ");
        if (jumpClip == null)
            jumpClip = Resources.Load<AudioClip>("jump");

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        seSource = gameObject.AddComponent<AudioSource>();
        seSource.playOnAwake = false;
        seSource.volume = seVolume;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard[toggleKey].wasPressedThisFrame)
        {
            ToggleBGM();
        }
    }

    private void ToggleBGM()
    {
        bgmEnabled = !bgmEnabled;

        if (bgmEnabled && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
        else
        {
            bgmSource.Stop();
            bgmEnabled = false;
        }
    }

    public void PlayJumpSound()
    {
        if (jumpClip != null)
        {
            seSource.PlayOneShot(jumpClip, seVolume);
        }
    }
}

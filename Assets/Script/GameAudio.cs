using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [Header("Clips")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip doorCloseClip;
    [SerializeField] private AudioClip passwordKeyClip;
    [SerializeField] private AudioClip passwordErrorClip;
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip victoryMusicClip;
    [SerializeField] private AudioClip defeatMusicClip;

    [Header("Volumes")]
    [SerializeField, Range(0f, 1f)] private float footstepsVolume = 0.45f;
    [SerializeField, Range(0f, 1f)] private float doorVolume = 0.75f;
    [SerializeField, Range(0f, 1f)] private float passwordVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float uiVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;

    [Header("Footsteps")]
    [SerializeField, Min(0.05f)] private float footstepInterval = 0.42f;
    [SerializeField, Range(0f, 0.3f)] private float footstepPitchVariation = 0.06f;

    private AudioSource musicSource;
    private AudioSource uiSource;
    private AudioSource footstepsSource;
    private float nextFootstepTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        musicSource = CreateSource("Music Source");
        uiSource = CreateSource("UI Source");
        footstepsSource = CreateSource("Footsteps Source");
    }

    private void Start()
    {
        RegisterExistingButtons();
        PlayBackgroundMusic();
    }

    public void SetFootstepsPlaying(bool shouldPlay)
    {
        if (!shouldPlay)
        {
            nextFootstepTime = 0f;
            return;
        }

        if (footstepClip == null || Time.time < nextFootstepTime)
            return;

        footstepsSource.pitch = 1f + Random.Range(
            -footstepPitchVariation,
            footstepPitchVariation
        );

        footstepsSource.PlayOneShot(footstepClip, footstepsVolume);
        nextFootstepTime = Time.time + footstepInterval;
    }

    public void PlayDoorOpen(Vector3 position)
    {
        PlayWorldClip(doorOpenClip, position, doorVolume);
    }

    public void PlayDoorClose(Vector3 position)
    {
        PlayWorldClip(doorCloseClip, position, doorVolume);
    }

    public void PlayPasswordKey()
    {
        PlayUiClip(passwordKeyClip, passwordVolume);
    }

    public void PlayPasswordError()
    {
        PlayUiClip(passwordErrorClip, passwordVolume);
    }

    public void PlayUiClick()
    {
        PlayUiClip(uiClickClip, uiVolume);
    }

    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusicClip, true);
    }

    public void PlayVictoryMusic()
    {
        PlayMusic(victoryMusicClip, false);
    }

    public void PlayDefeatMusic()
    {
        PlayMusic(defeatMusicClip, false);
    }

    public void RegisterButton(Button button)
    {
        if (button == null || button.GetComponent<UiClickSound>() != null)
            return;

        button.gameObject.AddComponent<UiClickSound>();
    }

    private AudioSource CreateSource(string sourceName)
    {
        GameObject sourceObject = new GameObject(sourceName);
        sourceObject.transform.SetParent(transform, false);

        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;

        return source;
    }

    private void PlayUiClip(AudioClip clip, float volume)
    {
        if (clip != null)
            uiSource.PlayOneShot(clip, volume);
    }

    private void PlayWorldClip(AudioClip clip, Vector3 position, float volume)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    private void PlayMusic(AudioClip clip, bool loop)
    {
        if (clip == null)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.pitch = 1f;
        musicSource.Play();
    }

    private void RegisterExistingButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
            RegisterButton(button);
    }
}

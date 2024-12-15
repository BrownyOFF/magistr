using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    private List<AudioClip> audioClips = new List<AudioClip>();

    private int currentTrackIndex = 0;

    public Button previousButton;
    public Button nextButton;
    public Button playButton;
    public Toggle toggleShuffle;
    public Toggle toggleLoop;
    public Slider volumeSlider;
    public TextMeshProUGUI trackNameText;
    public TMP_Dropdown tracksDropdown;
    
    private bool isShuffle = false;
    private bool isLoop = false;

    void Start()
    {
        // Завантажуємо аудіофайли
        AudioClip[] clipsCyberpunk = Resources.LoadAll<AudioClip>("Music/cyberpunk");
        audioClips.AddRange(clipsCyberpunk);
        AudioClip[] fantasyCyberpunk = Resources.LoadAll<AudioClip>("Music/fantasy");
        audioClips.AddRange(fantasyCyberpunk);

        tracksDropdown.onValueChanged.AddListener(OnPlaylistChanged);
        
        OnPlaylistChanged(tracksDropdown.value);
        // Прив'язуємо слухачі
        previousButton.onClick.AddListener(PlayPreviousTrack);
        nextButton.onClick.AddListener(PlayNextTrack);
        playButton.onClick.AddListener(TogglePlayPause);

        toggleShuffle.onValueChanged.AddListener(OnShuffleToggleChanged);
        toggleLoop.onValueChanged.AddListener(OnLoopToggleChanged);

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Встановлюємо початкову гучність
        audioSource.volume = volumeSlider.value;

        // Запускаємо перевірку завершення треку
        StartCoroutine(CheckIfTrackEnded());
    }
    private void OnPlaylistChanged(int index)
    {
        audioSource.Stop();
        // Залежно від вибраного індексу, змінюємо список треків
        switch (index)
        {
            case 0:
                audioClips = new List<AudioClip>(Resources.LoadAll<AudioClip>("Music/cyberpunk"));
                break;
            case 1:
                audioClips = new List<AudioClip>(Resources.LoadAll<AudioClip>("Music/fantasy"));
                break;
            // Додайте інші випадки, якщо є інші плейлісти
        }

        // Відтворюємо перший трек з нового списку
        UpdateTrackName("Select a track");
    }

    public void PlayTrack(int index)
    {
        if (index >= 0 && index < audioClips.Count)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
            UpdateTrackName(audioClips[index].name); 
        }
    }

    public void PlayNextTrack()
    {
        if (isShuffle)
        {
            currentTrackIndex = Random.Range(0, audioClips.Count);
        }
        else
        {
            currentTrackIndex = (currentTrackIndex + 1) % audioClips.Count;
        }
        PlayTrack(currentTrackIndex);
    }

    public void PlayPreviousTrack()
    {
        if (isShuffle)
        {
            currentTrackIndex = Random.Range(0, audioClips.Count);
        }
        else
        {
            currentTrackIndex = (currentTrackIndex - 1 + audioClips.Count) % audioClips.Count;
        }
        PlayTrack(currentTrackIndex);
    }

    private void TogglePlayPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            if (audioSource.clip == null) // Якщо трек не обраний
            {
                PlayTrack(currentTrackIndex); // Запустити поточний трек
            }
            else
            {
                audioSource.Play();
            }
        }
    }

    private void OnShuffleToggleChanged(bool value)
    {
        isShuffle = value;
    }

    private void OnLoopToggleChanged(bool value)
    {
        isLoop = value;
    }

    private void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
    }

    private IEnumerator CheckIfTrackEnded()
    {
        while (true)
        {
            // Перевірка, чи трек завершився, і чи програвач не на паузі
            if (audioSource.clip != null && !audioSource.isPlaying && audioSource.time == 0)
            {
                if (isLoop)
                {
                    PlayTrack(currentTrackIndex);
                }
                else
                {
                    PlayNextTrack();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void UpdateTrackName(string trackName)
    {
        trackNameText.text = "Now Playing: " + trackName;
    }
}

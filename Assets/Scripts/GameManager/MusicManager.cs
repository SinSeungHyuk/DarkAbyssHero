using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private AudioMixerSnapshot musicOffSnapshot;
    [SerializeField] private AudioMixerSnapshot musicLowSnapshot;
    [SerializeField] private AudioMixerSnapshot musicOnFullSnapshot;

    [SerializeField] private AudioMixerGroup musicMaster;

    private AudioSource musicAudioSource = null; // Music 오디오 그룹
    private AudioClip currentAudioClip = null; // 재생중인 클립
    private Coroutine fadeOutMusic;
    private Coroutine fadeInMusic;

    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        musicAudioSource = GetComponent<AudioSource>();

        // 해당 스냅샷으로 n초 동안 보간하여 전환 (볼륨조절 선형보간)
        musicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
            musicVolume = PlayerPrefs.GetInt("musicVolume");

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        // 세팅된 볼륨 저장하고 종료
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime,
        float fadeInTime = Settings.musicFadeInTime)
    {
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    public void IncreaseVolume()
    {
        int maxVolume = 20;

        if (musicVolume >= maxVolume) return;

        ++musicVolume;
        SetMusicVolume(musicVolume);
    }
    public void DecreaseVolume()
    {
        if (musicVolume == 0) return;

        --musicVolume;
        SetMusicVolume(musicVolume);
    }

    public void SetMusicLowSnapShot()
    {
        musicLowSnapshot.TransitionTo(0f);
    }
    public void SetMusicFullSnapShot()
    {
        musicOnFullSnapshot.TransitionTo(0f);
    }

    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        if (fadeOutMusic != null) StopCoroutine(fadeOutMusic);
        if (fadeInMusic != null) StopCoroutine(fadeInMusic);

        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusic = StartCoroutine(FadeOutMusic(fadeOutTime));
            yield return fadeInMusic = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }
    }

    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    private void SetMusicVolume(int musicVolume)
    {
        float mute = -80f;

        if (musicVolume == 0)
            musicMaster.audioMixer.SetFloat("musicVolume", mute);
        else
            musicMaster.audioMixer.SetFloat("musicVolume",
                UtilitieHelper.LinearToDecibels(musicVolume));
    }
}

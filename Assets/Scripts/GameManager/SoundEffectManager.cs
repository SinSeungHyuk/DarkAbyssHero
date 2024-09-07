using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectManager : Singleton<SoundEffectManager> 
{
    [SerializeField] private AudioMixerGroup soundsMaster;

    public int soundsVolume = 8;


    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
            soundsVolume = PlayerPrefs.GetInt("soundsVolume"); // 저장되어있는 볼륨 가져오기

        SetSoundsVolume(soundsVolume);
    }
    private void OnDisable()
    {
        // 세팅된 볼륨 저장하고 종료
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // 오브젝트 풀에 등록된 사운드 게임오브젝트 활성화하여 소리 재생
        SoundEffect sound = ObjectPoolManager.Instance.
            Get("soundEffect", Vector3.zero, Quaternion.identity).GetComponent<SoundEffect>();

        sound.SetSound(soundEffect); // 오디오 클립,볼륨 설정
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    public void IncreaseVolume()
    {
        int maxVolume = 20;

        if (soundsVolume >= maxVolume) return;

        ++soundsVolume;
        SetSoundsVolume(soundsVolume);
    }
    public void DecreaseVolume()
    {
        if (soundsVolume == 0) return;

        --soundsVolume;
        SetSoundsVolume(soundsVolume);
    }

    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {   // 사운드 길이만큼 시간이 지나면 사운드 오브젝트 비활성화
        yield return new WaitForSeconds(soundDuration);
        ObjectPoolManager.Instance.Release(sound.gameObject, "soundEffect");
    }

    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            soundsMaster.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {   // 사운드 볼륨을 데시벨로 전환
            soundsMaster.audioMixer.SetFloat(
                "soundsVolume", UtilitieHelper.LinearToDecibels(soundsVolume));
        }
    }
}

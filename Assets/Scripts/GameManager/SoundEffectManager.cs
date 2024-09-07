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
            soundsVolume = PlayerPrefs.GetInt("soundsVolume"); // ����Ǿ��ִ� ���� ��������

        SetSoundsVolume(soundsVolume);
    }
    private void OnDisable()
    {
        // ���õ� ���� �����ϰ� ����
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // ������Ʈ Ǯ�� ��ϵ� ���� ���ӿ�����Ʈ Ȱ��ȭ�Ͽ� �Ҹ� ���
        SoundEffect sound = ObjectPoolManager.Instance.
            Get("soundEffect", Vector3.zero, Quaternion.identity).GetComponent<SoundEffect>();

        sound.SetSound(soundEffect); // ����� Ŭ��,���� ����
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
    {   // ���� ���̸�ŭ �ð��� ������ ���� ������Ʈ ��Ȱ��ȭ
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
        {   // ���� ������ ���ú��� ��ȯ
            soundsMaster.audioMixer.SetFloat(
                "soundsVolume", UtilitieHelper.LinearToDecibels(soundsVolume));
        }
    }
}

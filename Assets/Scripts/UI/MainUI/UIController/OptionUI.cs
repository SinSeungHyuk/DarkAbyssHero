using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI soundsText;


    void Start()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeUI());
    }

    private IEnumerator InitializeUI()
    {
        // 한 프레임 그냥 넘기기. 사운드 세팅 기다리기
        yield return null;

        musicText.text = MusicManager.Instance.musicVolume.ToString();
        soundsText.text = SoundEffectManager.Instance.soundsVolume.ToString();
    }

    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseVolume();
        musicText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseVolume();
        musicText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaseVolume();
        soundsText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }
    public void DecreaseSoundsVolume()
    {
        SoundEffectManager.Instance.DecreaseVolume();
        soundsText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    public void BtnOK()
    {
        gameObject.SetActive(false);
    }
}

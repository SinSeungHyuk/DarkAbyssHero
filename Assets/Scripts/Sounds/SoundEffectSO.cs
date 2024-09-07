
using UnityEngine;


[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    [Header("Sound Effect Details")]
    public string soundEffectName;
    public GameObject soundPrefab;
    public AudioClip soundEffectClip;
    [Range(0.1f, 1.5f)]
    public float pitchVariationMin = 0.8f; // 사운드 피치 랜덤최소
    [Range(0.1f, 1.5f)]
    public float pitchVariationMax = 1.2f; // 사운드 피치 랜덤최대
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f; 


}

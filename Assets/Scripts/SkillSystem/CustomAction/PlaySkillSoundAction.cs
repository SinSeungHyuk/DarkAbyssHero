using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlaySkillSoundAction : CustomAction
{
    // 재생할 사운드
    [SerializeField] private SoundEffectSO soundEffect;


    public override void Run(object data)
    {
        // 재생하는 타이밍은 Run으로 고정 (이때 아니면 재생하기 적합한 타이밍이 딱히 없음)
        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }


    public override object Clone()
    {
        return new PlaySkillSoundAction()
        {
            soundEffect = soundEffect
        };
    }
}
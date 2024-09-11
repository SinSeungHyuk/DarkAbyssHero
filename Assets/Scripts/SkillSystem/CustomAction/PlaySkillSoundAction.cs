using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlaySkillSoundAction : CustomAction
{
    // ����� ����
    [SerializeField] private SoundEffectSO soundEffect;


    public override void Run(object data)
    {
        // ����ϴ� Ÿ�̹��� Run���� ���� (�̶� �ƴϸ� ����ϱ� ������ Ÿ�̹��� ���� ����)
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
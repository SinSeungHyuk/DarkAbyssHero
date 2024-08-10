using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct EffectData
{
    // ���� Data�� Effect�� �� Level Data������ ���� ����
    // ���� ���, level�� 3�� ���, Effect�� 3 Level Data��� �ǹ�
    public int level;


    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    // Effect�� ���� ȿ���� ����ϴ� Module
    // EffectAction�� ���� ���ݰ��� ���� Effect�� ȿ���� ������
    public EffectAction action;

    [UnderlineTitle("Setting")]
    // Effect�� �Ϸ��� ����
    public EffectRunningFinishOption runningFinishOption;
    // Effect ���ӽð�
    public float duration;
    // Effect�� ������ Ƚ��
    public int applyCount;
    // Effect�� ������ �ֱ�
    // ù �ѹ��� ȿ���� �ٷ� ����� ���̱� ������, �ѹ� ����� �ĺ��� ApplyCycle�� ���� �����
    // ���� ��, ApplyCycle�� 1�ʶ��, �ٷ� �ѹ� ����� �� 1�ʸ��� ����ǰ� ��. 
    [Min(0f)]
    public float applyCycle;

    // Effect�� �پ��� ������ �ֱ����� Module
    // ex. Particle Spawn, Sound ���, Camera Shake ��
    [UnderlineTitle("Custom Action")]
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActions;
}

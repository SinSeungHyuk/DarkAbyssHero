using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


// �������� ���ٰ����� ���� ��� static Ŭ����
public static class UtilitieHelper
{
    // Ȯ�� ����ϱ�  ===========================================================
    public static bool isSuccess(int percent)
    {
        int chance = Random.Range(1, 101);
        return percent >= chance; // �����ϸ� true
    }
    public static bool isSuccess(float percent)
    {
        int chance = Random.Range(1, 101);
        return percent >= chance; // �����ϸ� true
    }

    // ������ ����ϱ� =====================================================================
    public static float CalcPower(float attack, float health, float healthRegen, float healthOnKill, float critChance, float critDamage)
    {
        // �⺻ ���ݷ� ���
        float basePower = attack * 10;

        // ü�� ���� ���
        float healthPower = health * 0.5f + healthRegen * 20f + healthOnKill * 10f;

        // ġ��Ÿ ���� ���
        float critPower = (critChance / 100f) * (critDamage / 100f) * attack * 50f;

        // ���� ������ ���
        float totalCombatPower = basePower + healthPower + critPower;

        return totalCombatPower;
    }

    // ��޺� ���� ���� =================================================================
    public static Color32 GetGradeColor(GradeType type)
    {
        Color32 color = Color.white;

        switch (type)
        {
            case GradeType.Normal:
                color = Color.white;
                break;
            case GradeType.Rare:
                color = Settings.rare;
                break;
            case GradeType.Epic:
                color = Settings.epic;
                break;
            case GradeType.Legend:
                color = Settings.legend;
                break;
            default:
                break;
        };

        return color;
    }

    // ���� ���� �������� ���ú��� ��ȯ ====================================================================
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }
}

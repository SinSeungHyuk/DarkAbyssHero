using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


// 전역으로 접근가능한 각종 계산 static 클래스
public static class UtilitieHelper
{
    // 확률 계산하기  ===========================================================
    public static bool isSuccess(int percent)
    {
        int chance = Random.Range(1, 101);
        return percent >= chance; // 성공하면 true
    }
    public static bool isSuccess(float percent)
    {
        int chance = Random.Range(1, 101);
        return percent >= chance; // 성공하면 true
    }

    // 전투력 계산하기 =====================================================================
    public static float CalcPower(float attack, float health, float healthRegen, float healthOnKill, float critChance, float critDamage)
    {
        // 기본 공격력 계산
        float basePower = attack * 10;

        // 체력 관련 계산
        float healthPower = health * 0.5f + healthRegen * 20f + healthOnKill * 10f;

        // 치명타 관련 계산
        float critPower = (critChance / 100f) * (critDamage / 100f) * attack * 50f;

        // 종합 전투력 계산
        float totalCombatPower = basePower + healthPower + critPower;

        return totalCombatPower;
    }

    // 등급별 색상 리턴 =================================================================
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

    // 선형 볼륨 스케일을 데시벨로 변환 ====================================================================
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }
}

using System.Collections;
using System.Collections.Generic;
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

    // 등급별 업글재료 리턴 ================================================================
    public static int GetGradeCurrency(GradeType type)
    {
        int currency = 0;

        switch (type)
        {
            case GradeType.Normal:
                currency = 25;
                break;
            case GradeType.Rare:
                currency = 50;
                break;
            case GradeType.Epic:
                currency = 125;
                break;
            case GradeType.Legend:
                currency = 250;
                break;
            default:
                break;
        };

        return currency;
    }

    // 선형 볼륨 스케일을 데시벨로 변환 ====================================================================
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }
}

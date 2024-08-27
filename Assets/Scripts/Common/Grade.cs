using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Grade
{
    public GradeType Type { get; private set; }
    public string GradeName { get; private set; }
    public Color32 GradeColor { get; private set; }
    public int GradeCurrency { get; private set; }

    public Grade(GradeType type)
    {
        Type = type;
        (GradeName, GradeColor, GradeCurrency) = GetGradeInfo(type);
    }


    // struct는 모든 변수가 할당되기 전까지 this에 접근을 못함 (함수호출 불가능)
    // 따라서 static 함수로 인스턴스 없어도 사용가능한 함수를 호출
    private static (string, Color32, int) GetGradeInfo(GradeType type)
    {
        return type switch
        {
            GradeType.Normal => ("Normal", Color.white,50),
            GradeType.Rare => ("Rare", Settings.rare,100),
            GradeType.Epic => ("Epic", Settings.epic,250),
            GradeType.Legend => ("Legend", Settings.legend,500),
            _ => throw new ArgumentException("Invalid GradeType", nameof(type))
        };
    }
}
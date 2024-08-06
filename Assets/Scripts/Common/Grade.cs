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

    public Grade(GradeType type)
    {
        Type = type;
        (GradeName, GradeColor) = GetGradeInfo(type);
    }


    // struct�� ��� ������ �Ҵ�Ǳ� ������ this�� ������ ���� (�Լ�ȣ�� �Ұ���)
    // ���� static �Լ��� �ν��Ͻ� ��� ��밡���� �Լ��� ȣ��
    private static (string, Color32) GetGradeInfo(GradeType type)
    {
        return type switch
        {
            GradeType.Normal => ("Normal", Color.white),
            GradeType.Rare => ("Rare", Settings.rare),
            GradeType.Epic => ("Epic", Settings.epic),
            GradeType.Legend => ("Legend", Settings.legend),
            _ => throw new ArgumentException("Invalid GradeType", nameof(type))
        };
    }
}
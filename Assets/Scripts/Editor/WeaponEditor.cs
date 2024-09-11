using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : IdentifiedObjectEditor
{
    private SerializedProperty gradeTypeProperty;
    private SerializedProperty weaponDatasProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        weaponDatasProperty = serializedObject.FindProperty("weaponDatas");
        gradeTypeProperty = serializedObject.FindProperty("gradeType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        float prevLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 220f;

        CustomEditorUtility.DrawEnumToolbar(gradeTypeProperty);

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();

        DrawWeaponDatas();

        EditorGUIUtility.labelWidth = prevLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWeaponDatas()
    {
        // FindPropertyRelative("Array.size") : ����Ƽ���� �ڵ����� �迭�� ũ�⸦ ã����
        EditorGUILayout.PropertyField(weaponDatasProperty.FindPropertyRelative("Array.size"));

        // ������ �׸� ������Ƽ�� ���� �迭�� ����� �����ǰ� �� �����ŭ ������ �Ӽ� �׸���
        for (int i = 0; i < weaponDatasProperty.arraySize; i++)
        {
            SerializedProperty element = weaponDatasProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.PropertyField(element.FindPropertyRelative("stat"));
            EditorGUILayout.PropertyField(element.FindPropertyRelative("defaultValue"));
            EditorGUILayout.PropertyField(element.FindPropertyRelative("bonusStatPerLevel"));
            EditorGUILayout.EndVertical();
        }
    }
}

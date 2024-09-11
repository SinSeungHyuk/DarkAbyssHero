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
        // FindPropertyRelative("Array.size") : 유니티에서 자동으로 배열의 크기를 찾아줌
        EditorGUILayout.PropertyField(weaponDatasProperty.FindPropertyRelative("Array.size"));

        // 위에서 그린 프로퍼티에 의해 배열의 사이즈가 결정되고 그 사이즈만큼 무기의 속성 그리기
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

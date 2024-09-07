using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stage))]
public class StageEditor : IdentifiedObjectEditor
{
    private SerializedProperty stagePrefabProperty;
    private SerializedProperty stageLevelProperty;
    private SerializedProperty stageMusicProperty;
    private SerializedProperty monsterParametersProperty;


    protected override void OnEnable()
    {
        base.OnEnable();

        stagePrefabProperty = serializedObject.FindProperty("stagePrefab");
        stageMusicProperty = serializedObject.FindProperty("stageMusic");
        stageLevelProperty = serializedObject.FindProperty("stageLevel");
        monsterParametersProperty = serializedObject.FindProperty("monsterParameters");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update(); // 직렬화 변수 업데이트 시작


        EditorGUILayout.PropertyField(stagePrefabProperty);
        EditorGUILayout.PropertyField(stageMusicProperty);
        EditorGUILayout.PropertyField(stageLevelProperty);
        EditorGUILayout.PropertyField(monsterParametersProperty);


        serializedObject.ApplyModifiedProperties(); // 변경된 내용 저장
    }
}
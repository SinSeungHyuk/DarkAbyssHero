using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Effect))]
public class EffectEditor : IdentifiedObjectEditor
{
    private SerializedProperty effectTypeProperty;
    private SerializedProperty effectDatasProperty;


    protected override void OnEnable()
    {
        base.OnEnable();

        effectTypeProperty = serializedObject.FindProperty("effectType");
        effectDatasProperty = serializedObject.FindProperty("effectDatas");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // Lebel(=Inpector창에 표시되는 변수의 이름)의 길이를 늘림;
        float prevLevelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 175f;

        // effectType Enum을 Toolbar 형태로 그려줌
        CustomEditorUtility.DrawEnumToolbar(effectTypeProperty);

        DrawEffectDatas();

        // Label의 길이를 원래대로 되돌림
        EditorGUIUtility.labelWidth = prevLevelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawEffectDatas()
    {
        // Effect의 Data가 아무것도 존재하지 않으면 1개를 자동적으로 만들어줌
        if (effectDatasProperty.arraySize == 0)
        {
            // 배열 길이를 늘려서 새로운 Element를 생성
            effectDatasProperty.arraySize++;
            // 추가한 Data의 Level을 1로 설정
            effectDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        // 타이틀 그려주기
        if (!DrawFoldoutTitle("Data"))
            return;


        // effectDatas를 돌면서 GUI를 그려줌
        for (int i = 0; i < effectDatasProperty.arraySize; i++)
        {
            var property = effectDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                if (property.isExpanded)
                {
                    // 들여쓰기
                    EditorGUI.indentLevel += 1;

                    var levelProperty = property.FindPropertyRelative("level");
              

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("action"));

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("runningFinishOption"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("duration"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCount"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCycle"));

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("customActions"));

                    // 들여쓰기 종료
                    EditorGUI.indentLevel -= 1;
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}

using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Effect))]
public class EffectEditor : IdentifiedObjectEditor
{
    private SerializedProperty effectTypeProperty;
    private SerializedProperty maxLevelProperty;
    private SerializedProperty effectDatasProperty;


    protected override void OnEnable()
    {
        base.OnEnable();

        effectTypeProperty = serializedObject.FindProperty("effectType");
        maxLevelProperty = serializedObject.FindProperty("maxLevel");
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

        // Property를 수정하지 못하게 GUI Enable의 false로 바꿈
        //GUI.enabled = false;
        // 마지막 EffectData(= 가장 높은 Level의 Data)를 가져옴
        //var lastEffectData = effectDatasProperty.GetArrayElementAtIndex(effectDatasProperty.arraySize - 1);
        // maxLevel을 마지막 Data의 Level로 고정
        //maxLevelProperty.intValue = lastEffectData.FindPropertyRelative("level").intValue;
        // maxLevel Property를 그려줌
        EditorGUILayout.PropertyField(maxLevelProperty);
        //GUI.enabled = true;


        // effectDatas를 돌면서 GUI를 그려줌
        for (int i = 0; i < effectDatasProperty.arraySize; i++)
        {
            var property = effectDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data의 Level과 Data 삭제를 위한 X Button을 그려주는 Foldout Title을 그려줌
                // 단, 첫번째 Data(= index 0) 지우면 안되기 때문에 X Button을 그려주지 않음
                // X Button을 눌러서 Data가 지워지면 true를 return함
                if (DrawRemovableLevelFoldout(effectDatasProperty, property, i, i != 0))
                {
                    // Data가 삭제되었으며 더 이상 GUI를 그리지 않고 바로 빠져나감
                    // 다음 Frame에 처음부터 다시 그리기 위함
                    EditorGUILayout.EndVertical();
                    break;
                }

                if (property.isExpanded)
                {
                    // 들여쓰기
                    EditorGUI.indentLevel += 1;

                    var levelProperty = property.FindPropertyRelative("level");
                    // Level Property를 그려주면서 Level 값이 수정되면 Level을 기준으로 EffectDatas를 오름차순으로 정렬해줌
                    DrawAutoSortLevelProperty(effectDatasProperty, levelProperty, i, i != 0);

                   

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

        // EffectDatas에 새로운 Data를 추가하는 Button
        if (GUILayout.Button("Add New Level"))
        {
            // 데이터를 추가할때마다 이펙트의 maxLevel 증가
            maxLevelProperty.intValue++;

            // 배열 길이를 늘려서 새로운 Element를 생성
            var lastArraySize = effectDatasProperty.arraySize++;
            // 이전 Element Property를 가져옴
            var prevElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            // 새 Element Property를 가져옴
            var newElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize);
            // 새 Element의 Level은 이전 Element Level + 1
            var newElementLevel = prevElementProperty.FindPropertyRelative("level").intValue + 1;
            newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;

            // 새 Element의 Soft Copy된 Action을 Deep Copy함
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));

            // 새 Element의 Soft Copy된 CustomAction을 Deep Copy함
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActions"));
        }
    }
}

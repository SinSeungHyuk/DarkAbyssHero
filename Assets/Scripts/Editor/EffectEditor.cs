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

        // Lebel(=Inpectorâ�� ǥ�õǴ� ������ �̸�)�� ���̸� �ø�;
        float prevLevelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 175f;

        // effectType Enum�� Toolbar ���·� �׷���
        CustomEditorUtility.DrawEnumToolbar(effectTypeProperty);

        DrawEffectDatas();

        // Label�� ���̸� ������� �ǵ���
        EditorGUIUtility.labelWidth = prevLevelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawEffectDatas()
    {
        // Effect�� Data�� �ƹ��͵� �������� ������ 1���� �ڵ������� �������
        if (effectDatasProperty.arraySize == 0)
        {
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            effectDatasProperty.arraySize++;
            // �߰��� Data�� Level�� 1�� ����
            effectDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        // Ÿ��Ʋ �׷��ֱ�
        if (!DrawFoldoutTitle("Data"))
            return;


        // effectDatas�� ���鼭 GUI�� �׷���
        for (int i = 0; i < effectDatasProperty.arraySize; i++)
        {
            var property = effectDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                if (property.isExpanded)
                {
                    // �鿩����
                    EditorGUI.indentLevel += 1;

                    var levelProperty = property.FindPropertyRelative("level");
              

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("action"));

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("runningFinishOption"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("duration"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCount"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCycle"));

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("customActions"));

                    // �鿩���� ����
                    EditorGUI.indentLevel -= 1;
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}

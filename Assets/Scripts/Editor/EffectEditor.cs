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

        // Property�� �������� ���ϰ� GUI Enable�� false�� �ٲ�
        //GUI.enabled = false;
        // ������ EffectData(= ���� ���� Level�� Data)�� ������
        //var lastEffectData = effectDatasProperty.GetArrayElementAtIndex(effectDatasProperty.arraySize - 1);
        // maxLevel�� ������ Data�� Level�� ����
        //maxLevelProperty.intValue = lastEffectData.FindPropertyRelative("level").intValue;
        // maxLevel Property�� �׷���
        EditorGUILayout.PropertyField(maxLevelProperty);
        //GUI.enabled = true;


        // effectDatas�� ���鼭 GUI�� �׷���
        for (int i = 0; i < effectDatasProperty.arraySize; i++)
        {
            var property = effectDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title�� �׷���
                // ��, ù��° Data(= index 0) ����� �ȵǱ� ������ X Button�� �׷����� ����
                // X Button�� ������ Data�� �������� true�� return��
                if (DrawRemovableLevelFoldout(effectDatasProperty, property, i, i != 0))
                {
                    // Data�� �����Ǿ����� �� �̻� GUI�� �׸��� �ʰ� �ٷ� ��������
                    // ���� Frame�� ó������ �ٽ� �׸��� ����
                    EditorGUILayout.EndVertical();
                    break;
                }

                if (property.isExpanded)
                {
                    // �鿩����
                    EditorGUI.indentLevel += 1;

                    var levelProperty = property.FindPropertyRelative("level");
                    // Level Property�� �׷��ָ鼭 Level ���� �����Ǹ� Level�� �������� EffectDatas�� ������������ ��������
                    DrawAutoSortLevelProperty(effectDatasProperty, levelProperty, i, i != 0);

                   

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

        // EffectDatas�� ���ο� Data�� �߰��ϴ� Button
        if (GUILayout.Button("Add New Level"))
        {
            // �����͸� �߰��Ҷ����� ����Ʈ�� maxLevel ����
            maxLevelProperty.intValue++;

            // �迭 ���̸� �÷��� ���ο� Element�� ����
            var lastArraySize = effectDatasProperty.arraySize++;
            // ���� Element Property�� ������
            var prevElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            // �� Element Property�� ������
            var newElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize);
            // �� Element�� Level�� ���� Element Level + 1
            var newElementLevel = prevElementProperty.FindPropertyRelative("level").intValue + 1;
            newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;

            // �� Element�� Soft Copy�� Action�� Deep Copy��
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));

            // �� Element�� Soft Copy�� CustomAction�� Deep Copy��
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActions"));
        }
    }
}

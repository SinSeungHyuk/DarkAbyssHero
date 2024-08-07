using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : IdentifiedObjectEditor
{
    private SerializedProperty applyTypeProperty;
    private SerializedProperty gradeTypeProperty;
    private SerializedProperty skillPriorityProperty;

    private SerializedProperty maxLevelProperty;
    private SerializedProperty defaultLevelProperty;
    private SerializedProperty skillDatasProperty;


    protected override void OnEnable()
    {
        base.OnEnable();

        applyTypeProperty = serializedObject.FindProperty("applyType");
        gradeTypeProperty = serializedObject.FindProperty("gradeType");
        skillPriorityProperty = serializedObject.FindProperty("skillPriority");

        maxLevelProperty = serializedObject.FindProperty("maxLevel");
        defaultLevelProperty = serializedObject.FindProperty("defaultLevel");
        skillDatasProperty = serializedObject.FindProperty("skillDatas");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        float prevLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 220f;

        DrawSettings();
        DrawSkillDatas();

        EditorGUIUtility.labelWidth = prevLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSettings()
    {
        CustomEditorUtility.DrawEnumToolbar(applyTypeProperty);
        CustomEditorUtility.DrawEnumToolbar(gradeTypeProperty);
        EditorGUILayout.PropertyField(skillPriorityProperty);

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();
    }

    private void DrawSkillDatas()
    {
        // Skill�� Data�� �ƹ��͵� �������� ������ 1���� �ڵ������� �������
        if (skillDatasProperty.arraySize == 0)
        {
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            skillDatasProperty.arraySize++;
            // �߰��� Data�� Level�� 1�� ����
            skillDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        if (!DrawFoldoutTitle("Data"))
            return;


        // maxLevel, defaultLevel Property�� �׷���
        EditorGUILayout.PropertyField(maxLevelProperty);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(defaultLevelProperty);
        GUI.enabled = true;


        // ��ų������ ���� �ϳ��ϳ� �׷��ֱ�
        for (int i = 0; i < skillDatasProperty.arraySize; i++)
        {
            var skillDataProperty = skillDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title�� �׷���
                // ��, ù��° Data(= index 0) ����� �ȵǱ� ������ X Button�� �׷����� ����
                // X Button�� ������ Data�� �������� true�� return��
                if (DrawRemovableLevelFoldout(skillDatasProperty, skillDataProperty, i, i != 0))
                {
                    // Data�� �����Ǿ����� �� �̻� GUI�� �׸��� �ʰ� �ٷ� ��������
                    // ���� Frame�� ó������ �ٽ� �׸��� ����
                    EditorGUILayout.EndVertical();
                    break;
                }
            }

            var actionProperty = skillDataProperty.FindPropertyRelative("action");
            EditorGUILayout.PropertyField(actionProperty);

            var runningFinishOptionProperty = skillDataProperty.FindPropertyRelative("runningFinishOption");
            CustomEditorUtility.DrawEnumToolbar(runningFinishOptionProperty);

            EditorGUILayout.Space();
            CustomEditorUtility.DrawUnderline();
            EditorGUILayout.Space();

            // Settings
            var durationProperty = skillDataProperty.FindPropertyRelative("duration");
            EditorGUILayout.PropertyField(durationProperty);
            var applyCountProperty = skillDataProperty.FindPropertyRelative("applyCount");
            EditorGUILayout.PropertyField(applyCountProperty);
            var applyCycleProperty = skillDataProperty.FindPropertyRelative("applyCycle");
            EditorGUILayout.PropertyField(applyCycleProperty);
            var cooldownProperty = skillDataProperty.FindPropertyRelative("cooldown");
            EditorGUILayout.PropertyField(cooldownProperty);            
            var distanceProperty = skillDataProperty.FindPropertyRelative("distance");
            EditorGUILayout.PropertyField(distanceProperty);

            // Cast
            var isUseCastProperty = skillDataProperty.FindPropertyRelative("isUseCast");
            EditorGUILayout.PropertyField(isUseCastProperty);
            var castTimeProperty = skillDataProperty.FindPropertyRelative("castTime");
            EditorGUILayout.PropertyField(castTimeProperty);


            // EffectSelector[] �迭 ������Ƽ �׸���
            var effectSelectorsProperty = skillDataProperty.FindPropertyRelative("effectSelectors");
            EditorGUILayout.PropertyField(effectSelectorsProperty);
            // EffectSelector �迭 ������Ƽ�� arraySize ��ŭ ��ȸ�ϸ鼭 ����Ʈ�� �ִ뷹�� ��������
            for (int j = 0; j < effectSelectorsProperty.arraySize; j++)
            {
                // �迭 ������Ƽ�� ��ȸ�ϸ鼭 �� EffectSelector �ϳ��� ��������
                var effectSelectorProperty = effectSelectorsProperty.GetArrayElementAtIndex(j);
                // EffectSelector ������Ƽ�� level ������Ƽ ��������
                var levelProperty = effectSelectorProperty.FindPropertyRelative("level");
                // EffectSelector ������Ƽ�� effect ������Ƽ ��������
                var effect = effectSelectorProperty.FindPropertyRelative("effect").objectReferenceValue as Effect;
                var maxLevel = effect != null ? effect.MaxLevel : 0;
                var minLevel = maxLevel == 0 ? 0 : 1;
                // levelProperty�� int���� �ڵ����� ������ EffectSelector ������Ƽ�� ������ �����ֱ�
                levelProperty.intValue = Mathf.Clamp(levelProperty.intValue, minLevel, maxLevel);
            }


            // �ִϸ����� �Ķ���� enum ������Ƽ (enum���ٷ� �׸��� �� ©��)
            var castAnimatorParameterProperty = skillDataProperty.FindPropertyRelative("castAnimatorParameter");
            var actionAnimatorParameterProperty = skillDataProperty.FindPropertyRelative("actionAnimatorParameter");
            EditorGUILayout.PropertyField(castAnimatorParameterProperty);
            EditorGUILayout.PropertyField(actionAnimatorParameterProperty);

            // Custom Action
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Action", EditorStyles.boldLabel);
            CustomEditorUtility.DrawUnderline();

            var customActionsOnCastProperty = skillDataProperty.FindPropertyRelative("customActionsOnCast");
            var customActionsOnActionProperty = skillDataProperty.FindPropertyRelative("customActionsOnAction");
            EditorGUILayout.PropertyField(customActionsOnCastProperty);
            EditorGUILayout.PropertyField(customActionsOnActionProperty);


            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add New Level"))
        {
            // Level Change
            var lastArraySize = skillDatasProperty.arraySize++;
            var prevElementalProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            var newElementProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize);
            var newElementLevel = prevElementalProperty.FindPropertyRelative("level").intValue + 1;
            newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;
            newElementProperty.isExpanded = true;


            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));

            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnCast"));
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnAction"));
        }
    }
}
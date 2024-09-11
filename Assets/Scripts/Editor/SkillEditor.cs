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
    private SerializedProperty skillSoundProperty;

    private SerializedProperty skillDataProperty;


    protected override void OnEnable()
    {
        base.OnEnable();

        applyTypeProperty = serializedObject.FindProperty("applyType");
        gradeTypeProperty = serializedObject.FindProperty("gradeType");
        skillPriorityProperty = serializedObject.FindProperty("skillPriority");
        skillSoundProperty = serializedObject.FindProperty("skillSound");

        skillDataProperty = serializedObject.FindProperty("skillDatas");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        float prevLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 220f;

        DrawSettings();
        DrawSkillData();

        EditorGUIUtility.labelWidth = prevLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSettings()
    {
        // ��ų�� Ÿ��, ��� enum�� ���ٷ� �׷��ִ� �Լ� ȣ��
        CustomEditorUtility.DrawEnumToolbar(applyTypeProperty);
        CustomEditorUtility.DrawEnumToolbar(gradeTypeProperty);
        EditorGUILayout.PropertyField(skillPriorityProperty);
        EditorGUILayout.PropertyField(skillSoundProperty);

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();
    }

    private void DrawSkillData()
    {
        // ��ų�� �����͸� �׸��� �Լ�

        var actionProperty = skillDataProperty.FindPropertyRelative("action");
        EditorGUILayout.PropertyField(actionProperty);

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();

        var runningFinishOptionProperty = skillDataProperty.FindPropertyRelative("runningFinishOption");
        CustomEditorUtility.DrawEnumToolbar(runningFinishOptionProperty);

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
            var minLevel = 1;
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
    }
}
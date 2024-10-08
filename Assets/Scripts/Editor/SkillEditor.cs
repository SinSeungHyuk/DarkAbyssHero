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
        // 스킬의 타입, 등급 enum을 툴바로 그려주는 함수 호출
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
        // 스킬의 데이터를 그리는 함수

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


        // EffectSelector[] 배열 프로퍼티 그리기
        var effectSelectorsProperty = skillDataProperty.FindPropertyRelative("effectSelectors");
        EditorGUILayout.PropertyField(effectSelectorsProperty);
        // EffectSelector 배열 프로퍼티의 arraySize 만큼 순회하면서 이펙트의 최대레벨 가져오기
        for (int j = 0; j < effectSelectorsProperty.arraySize; j++)
        {
            // 배열 프로퍼티를 순회하면서 각 EffectSelector 하나씩 가져오기
            var effectSelectorProperty = effectSelectorsProperty.GetArrayElementAtIndex(j);
            // EffectSelector 프로퍼티의 level 프로퍼티 가져오기
            var levelProperty = effectSelectorProperty.FindPropertyRelative("level");
            // EffectSelector 프로퍼티의 effect 프로퍼티 가져오기
            var effect = effectSelectorProperty.FindPropertyRelative("effect").objectReferenceValue as Effect;
            var maxLevel = effect != null ? effect.MaxLevel : 0;
            var minLevel = 1;
            // levelProperty의 int값을 자동으로 가져온 EffectSelector 프로퍼티의 레벨로 맞춰주기
            levelProperty.intValue = Mathf.Clamp(levelProperty.intValue, minLevel, maxLevel);
        }


        // 애니메이터 파라미터 enum 프로퍼티 (enum툴바로 그리면 길어서 짤림)
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
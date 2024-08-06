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

    private SerializedProperty maxLevelProperty;
    private SerializedProperty defaultLevelProperty;
    private SerializedProperty skillDatasProperty;

    // Toolbar Button들의 이름
    private readonly string[] customActionsToolbarList = new[] { "Cast", "Action" };
    // Skill Data마다 선택한 Toolbar Button의 Index 값
    private Dictionary<int, int> customActionToolbarIndexesByLevel = new();


    protected override void OnEnable()
    {
        base.OnEnable();

        applyTypeProperty = serializedObject.FindProperty("applyType");
        gradeTypeProperty = serializedObject.FindProperty("gradeType");

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

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();
    }

    private void DrawSkillDatas()
    {
        // Skill의 Data가 아무것도 존재하지 않으면 1개를 자동적으로 만들어줌
        if (skillDatasProperty.arraySize == 0)
        {
            // 배열 길이를 늘려서 새로운 Element를 생성
            skillDatasProperty.arraySize++;
            // 추가한 Data의 Level을 1로 설정
            skillDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        if (!DrawFoldoutTitle("Data"))
            return;


        // Property를 수정하지 못하게 GUI Enable의 false로 바꿈
        var lastIndex = skillDatasProperty.arraySize - 1;
        // 마지막 SkillData(= 가장 높은 Level의 Data)를 가져옴
        var lastSkillData = skillDatasProperty.GetArrayElementAtIndex(lastIndex);
        // maxLevel을 마지막 Data의 Level로 고정
        maxLevelProperty.intValue = lastSkillData.FindPropertyRelative("level").intValue;

        // maxLevel, defaultLevel Property를 그려줌
        EditorGUILayout.PropertyField(maxLevelProperty);
        EditorGUILayout.PropertyField(defaultLevelProperty);

        // 스킬데이터 원소 하나하나 그려주기
        for (int i = 0; i < skillDatasProperty.arraySize; i++)
        {
            var skillDataProperty = skillDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data의 Level과 Data 삭제를 위한 X Button을 그려주는 Foldout Title을 그려줌
                // 단, 첫번째 Data(= index 0) 지우면 안되기 때문에 X Button을 그려주지 않음
                // X Button을 눌러서 Data가 지워지면 true를 return함
                if (DrawRemovableLevelFoldout(skillDatasProperty, skillDataProperty, i, i != 0))
                {
                    // Data가 삭제되었으며 더 이상 GUI를 그리지 않고 바로 빠져나감
                    // 다음 Frame에 처음부터 다시 그리기 위함
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
                var minLevel = maxLevel == 0 ? 0 : 1;
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
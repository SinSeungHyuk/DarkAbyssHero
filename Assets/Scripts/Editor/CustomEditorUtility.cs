using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public static class CustomEditorUtility
{
    private readonly static GUIStyle titleStyle;

    static CustomEditorUtility()
    {
        // 유니티 내부에 정의되어있는 ShurikenModuleTitle Style을 Base로 함
        titleStyle = new GUIStyle("ShurikenModuleTitle")
        {
            // 유니티 Default Label의 font를 가져옴
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            // title을 그릴 상하좌우 공간에 여유를 줌
            border = new RectOffset(15, 7, 4, 4),
            // 높이는 26
            fixedHeight = 26f,
            // 내부 Text의 위치를 조절함
            contentOffset = new Vector2(20f, -2f)
        };
    }

    // 접이식 타이틀 그리기
    public static bool DrawFoldoutTitle(string title, bool isExpanded, float space = 15f)
    {
        // space만큼 윗 줄을 띄움
        EditorGUILayout.Space(space);

        // titleStyle의 정보를 가지고 Inspector상에서 옳바른 위치를 가져옴
        var rect = GUILayoutUtility.GetRect(16f, titleStyle.fixedHeight, titleStyle);
        // TitleStyle을 적용시킨 Box를 그려줌
        GUI.Box(rect, title, titleStyle);

        // 현재 Editor의 Event를 가져옴
        // Editor Event는 마우스 입력, GUI 새로 그리기(Repaint), 키보드 입력 등 Editor 상에서 일어나는 일임
        var currentEvent = Event.current;
        // Toggle Button의 위치와 크기를 정함
        // 위치는 방금 그린 박스의 좌표에서 살짝 오른쪽 아래, 즉 Button이 좌, 가운데 정렬이 된 형태가 됨.
        var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);

        // Event가 Repaint(=GUI를 그린다 혹은 다시 그린다)면 단순히 foldout button을 보여줌
        if (currentEvent.type == EventType.Repaint)
            EditorStyles.foldout.Draw(toggleRect, false, false, isExpanded, false);
        // Event가 MouseDown이고 mousePosition이 rect와 겹쳐있으면(=Mouse Pointer가 위에서 그려준 Box안에 있음) Click 판정
        else if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
        {
            isExpanded = !isExpanded;
            // Use 함수를 사용하지 않으면 아직 Event가 처리되지 않은 것으로 판단되어 같은 위치에 있는 다른 GUI도 같이 동작될 수 있음.
            // event 처리를 했으면 항상 Use를 통해 event에 대한 처리를 했음을 Unity에 알려주는게 좋음
            currentEvent.Use();
        }

        return isExpanded;
    }

    // FoldoutTitle을 그림과 동시에 인자로 받은 Dictionary에 Expand 상황을 저장까지 해줌
    // 여러개의 타이틀이 있을경우를 대비해서 딕셔너리 사용
    public static bool DrawFoldoutTitle(IDictionary<string, bool> isFoldoutExpandedesByTitle, string title, float space = 15f)
    {
        if (!isFoldoutExpandedesByTitle.ContainsKey(title))
            isFoldoutExpandedesByTitle[title] = true;

        isFoldoutExpandedesByTitle[title] = DrawFoldoutTitle(title, isFoldoutExpandedesByTitle[title], space);
        return isFoldoutExpandedesByTitle[title];
    }

    // 줄 하나 그리기
    public static void DrawUnderline(float height = 1f)
    {
        // 마지막으로 그린 GUI의 위치와 크기 정보를 가진 Rect 구조체를 가져옴
        var lastRect = GUILayoutUtility.GetLastRect();
        // Rect 구조체를 indent(=들여쓰기)가 적용된 값으로 변환함
        lastRect = EditorGUI.IndentedRect(lastRect);
        // rect의 y값을 이전 GUI의 높이만큼 내림(=즉, y값은 이전 GUI 바로 아래에 위치하게 됨)
        lastRect.y += lastRect.height;
        lastRect.height = height;
        // rect 값을 이용해서 지정된 위치에 height크기의 Box를 그림
        // height가 1이라면 이전 GUI 바로 아래에 크기가 1인 Box, 즉 Line이 그려지게됨
        EditorGUI.DrawRect(lastRect, Color.gray);
    }

    // enum 변수 툴바로 그리기
    public static void DrawEnumToolbar(SerializedProperty enumProperty)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(enumProperty.displayName);
        // enum 프로퍼티를 툴바로 그리기 (각 인덱스별 DisplayName으로)
        enumProperty.enumValueIndex = GUILayout.Toolbar(enumProperty.enumValueIndex, enumProperty.enumDisplayNames);
        EditorGUILayout.EndHorizontal();
    }

    // 깊은 복사를 위한 함수 => 스킬,이펙트를 레벨별로 추가하기 위해선 이전 데이터를 깊은복사해야함
    public static void DeepCopySerializeReference(SerializedProperty property)
    {
        // managedReferenceValue는 SerializeReference Attribute를 적용한 변수
        if (property.managedReferenceValue == null)
            return;

        // ICloneable의 Clone 함수로 쉽게 깊은복사 가능
        property.managedReferenceValue = (property.managedReferenceValue as ICloneable).Clone();
    }

    public static void DeepCopySerializeReferenceArray(SerializedProperty property, string fieldName = "")
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            // Array에서 Element를 가져옴
            var elementProperty = property.GetArrayElementAtIndex(i);
            // Element가 일반 class나 struct라서 Element 내부에 SerializeReference 변수가 있을 수 있으므로,
            // fieldName이 Empty가 아니라면 Elenemt에서 fieldName 변수 정보를 찾아옴
            if (!string.IsNullOrEmpty(fieldName))
                elementProperty = elementProperty.FindPropertyRelative(fieldName);

            if (elementProperty.managedReferenceValue == null)
                continue;

            // 찾아온 정보를 이용해서 property의 manageredRefenceValue에서 Clone 함수를 실행시킴
            elementProperty.managedReferenceValue = (elementProperty.managedReferenceValue as ICloneable).Clone();
        }
    }
}

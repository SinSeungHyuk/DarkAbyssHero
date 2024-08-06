using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnderlineTitleAttribute))]
public class UnderlineTitleDrawer : DecoratorDrawer
{
    // UnderlineTitleAttribute 어트리뷰트를 그리는 클래스도 있어야 적용됨

    public override void OnGUI(Rect position)
    {
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;

        position = EditorGUI.IndentedRect(position);
        position.height = EditorGUIUtility.singleLineHeight;
        position.y += attributeAsUnderlineTitle.Space;

        // position에 title을 Bold Style로 그림
        GUI.Label(position, attributeAsUnderlineTitle.Title, EditorStyles.boldLabel);

        // 한줄 이동
        position.y += EditorGUIUtility.singleLineHeight;
        // 두께는 1
        position.height = 1f;
        // 회색 선을 그림
        EditorGUI.DrawRect(position, Color.gray);
    }

    // GUI의 총 높이를 반환하는 함수
    public override float GetHeight()
    {
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;
        // 기본 GUI 높이 + (기본 GUI 간격 * 2) + 설정한 Attribute Space 
        return attributeAsUnderlineTitle.Space + EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2);
    }
}

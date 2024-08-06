using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnderlineTitleAttribute))]
public class UnderlineTitleDrawer : DecoratorDrawer
{
    // UnderlineTitleAttribute ��Ʈ����Ʈ�� �׸��� Ŭ������ �־�� �����

    public override void OnGUI(Rect position)
    {
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;

        position = EditorGUI.IndentedRect(position);
        position.height = EditorGUIUtility.singleLineHeight;
        position.y += attributeAsUnderlineTitle.Space;

        // position�� title�� Bold Style�� �׸�
        GUI.Label(position, attributeAsUnderlineTitle.Title, EditorStyles.boldLabel);

        // ���� �̵�
        position.y += EditorGUIUtility.singleLineHeight;
        // �β��� 1
        position.height = 1f;
        // ȸ�� ���� �׸�
        EditorGUI.DrawRect(position, Color.gray);
    }

    // GUI�� �� ���̸� ��ȯ�ϴ� �Լ�
    public override float GetHeight()
    {
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;
        // �⺻ GUI ���� + (�⺻ GUI ���� * 2) + ������ Attribute Space 
        return attributeAsUnderlineTitle.Space + EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2);
    }
}

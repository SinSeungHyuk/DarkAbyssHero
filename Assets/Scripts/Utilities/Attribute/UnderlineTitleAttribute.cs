using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PropertyAttribute 를 상속받아 만든 커스텀 어트리뷰트
// 클래스 이름 뒤에붙은 Attribute를 떼고 사용할 수 있다.
public class UnderlineTitleAttribute : PropertyAttribute
{
    // Title Text
    public string Title { get; private set; }
    // 윗쪽 GUI와 띄워줄 공간
    public int Space { get; private set; }

    public UnderlineTitleAttribute(string title, int space = 12)
    {
        Title = title;
        Space = space;
    }
}

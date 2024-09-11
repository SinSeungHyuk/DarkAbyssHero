using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    // 몬스터와 플레이어가 공통으로 가지게 될 속성
    // 인터페이스를 사용할 경우 컴포넌트에 접근하진 못하게됨 
    // -> 따라서 공통요소로 추상클래스를 상속받는 것을 선택
}

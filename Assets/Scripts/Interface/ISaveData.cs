using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 세이브해야하는 클래스에 상속. 구조체 타입으로만 세이브
public interface ISaveData<T>
{
    T ToSaveData();
    void FromSaveData(T saveData);
}

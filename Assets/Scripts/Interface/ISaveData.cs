using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���̺��ؾ��ϴ� Ŭ������ ���. ����ü Ÿ�����θ� ���̺�
public interface ISaveData<T>
{
    T ToSaveData();
    void FromSaveData(T saveData);
}

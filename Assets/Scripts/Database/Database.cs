using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Database")]
public class Database : ScriptableObject
{
    // IdentifiedObject Ÿ���� ������(SO)�� �� �����ͺ��̽�
    [SerializeField] private List<IdentifiedObject> datas = new();

    public IReadOnlyList<IdentifiedObject> Datas => datas;
    public int Count => datas.Count;


    private void SetID(IdentifiedObject target, int id)
    {
        // �Ű������� ���� target�� id�� �Ű����� id�� ����

        // id �ʵ�� public,static ������ �ƴϿ�����
        FieldInfo field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        // FieldInfo�� SetValue�� Ÿ���� id�ʵ带 �Ű����� id�� ����
        field.SetValue(target, id);

        // SetDirty�� ����Ƽ�� �������� �˷��־�� ������ �����
        // AssetDatabase.SaveAssets�� ȣ���ؾ��ϴµ�, �̴� ���߿� �ٸ� Ŭ�������� ���ȣ������
#if UNITY_EDITOR
        EditorUtility.SetDirty(target);
#endif
    }

    // index ������ IdentifiedObjects�� id�� �缳����
    private void ReorderDatas()
    {
        var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
        for (int i = 0; i < datas.Count; i++)
        {
            field.SetValue(datas[i], i);

            // ���������� �ٸ� Ŭ�������� SaveAssets�� ȣ���ϹǷ� ���⿣ ��������
#if UNITY_EDITOR
            EditorUtility.SetDirty(datas[i]);
#endif
        }
    }

    // ������ �����쿡 ������ ������ �߰��Լ�. SetID�� �߰��� �������� ID�� ����
    public void Add(IdentifiedObject newData)
    {
        datas.Add(newData);
        SetID(newData, datas.Count - 1); // ID�� 0������ ����
    }
    // ������ �����쿡 ������ ������ �����Լ�
    public void Remove(IdentifiedObject data)
    {
        datas.Remove(data);
        ReorderDatas();
    }

    // Data�� CodeName�� �������� ������������ ������
    public void SortByCodeName()
    {
        datas.Sort((x, y) => x.CodeName.CompareTo(y.CodeName));
        ReorderDatas();
    }

    public IdentifiedObject GetDataByID(int id) => datas[id];

    public bool Contains(IdentifiedObject item) => datas.Contains(item);
}

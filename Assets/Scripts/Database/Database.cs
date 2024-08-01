using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Database")]
public class Database : ScriptableObject
{
    // IdentifiedObject 타입의 데이터(SO)가 들어갈 데이터베이스
    [SerializeField] private List<IdentifiedObject> datas = new();

    public IReadOnlyList<IdentifiedObject> Datas => datas;
    public int Count => datas.Count;


    private void SetID(IdentifiedObject target, int id)
    {
        // 매개변수로 받은 target의 id를 매개변수 id로 설정

        // id 필드는 public,static 변수가 아니여야함
        FieldInfo field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        // FieldInfo의 SetValue로 타겟의 id필드를 매개변수 id로 설정
        field.SetValue(target, id);

        // SetDirty로 유니티에 수정됨을 알려주어야 실제로 저장됨
        // AssetDatabase.SaveAssets도 호출해야하는데, 이는 나중에 다른 클래스에서 대신호출해줌
#if UNITY_EDITOR
        EditorUtility.SetDirty(target);
#endif
    }

    // index 순서로 IdentifiedObjects의 id를 재설정함
    private void ReorderDatas()
    {
        var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
        for (int i = 0; i < datas.Count; i++)
        {
            field.SetValue(datas[i], i);

            // 마찬가지로 다른 클래스에서 SaveAssets를 호출하므로 여기엔 적지않음
#if UNITY_EDITOR
            EditorUtility.SetDirty(datas[i]);
#endif
        }
    }

    // 에디터 윈도우에 연결할 데이터 추가함수. SetID로 추가한 데이터의 ID값 설정
    public void Add(IdentifiedObject newData)
    {
        datas.Add(newData);
        SetID(newData, datas.Count - 1); // ID는 0번부터 시작
    }
    // 에디터 윈도우에 연결할 데이터 삭제함수
    public void Remove(IdentifiedObject data)
    {
        datas.Remove(data);
        ReorderDatas();
    }

    // Data를 CodeName을 기준으로 오름차순으로 정렬함
    public void SortByCodeName()
    {
        datas.Sort((x, y) => x.CodeName.CompareTo(y.CodeName));
        ReorderDatas();
    }

    public IdentifiedObject GetDataByID(int id) => datas[id];

    public bool Contains(IdentifiedObject item) => datas.Contains(item);
}

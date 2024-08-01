using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class IdentifiedObject : ScriptableObject, ICloneable
{
    // 이 데이터의 카테고리 (몬스터,스킬,무기 등)
    [SerializeField] private Category[] categories;

    [SerializeField] private Sprite icon;
    [SerializeField] private int id = -1;
    [SerializeField] private string codeName;
    [SerializeField] private string displayName;
    [SerializeField] private string description;

    public Sprite Icon => icon;
    public int ID => id;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public virtual string Description => description;

    // ICloneable 인터페이스의 Clone 함수 구현 => 객체를 복사해서 생성
    public virtual object Clone() => Instantiate(this);

    public bool HasCategory(Category category)
        => categories.Any(x => x.ID == category.ID);

    public bool HasCategory(string category)
        => categories.Any(x => x == category);
}

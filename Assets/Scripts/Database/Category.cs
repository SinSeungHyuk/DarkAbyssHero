using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Category : IdentifiedObject
{
    // 각 데이터의 카테고리를 담을 SO

    public override bool Equals(object other)
        => base.Equals(other);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Category lhs, string rhs)
    {
        if (lhs is null)
            return rhs is null;
        return lhs.CodeName == rhs;
    }

    public static bool operator !=(Category lhs, string rhs)
        => !(lhs == rhs);
}

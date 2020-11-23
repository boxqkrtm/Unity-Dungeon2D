using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{
    public List<UnitDB> pool;

    public string GetUnitName(UnitInfo unitInfo)
    {
        return pool[unitInfo.UnitCode].uname;
    }

    public List<DropCell> GetDropTable(UnitInfo unitInfo)
    {
        return pool[unitInfo.UnitCode].dropTable;
    }

    public Vector2Int GetDropGoldRange(UnitInfo unitInfo)
    {
        return pool[unitInfo.UnitCode].dropGold;
    }
}

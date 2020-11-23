using UnityEngine;
public class UnitInfo
{
    int unitCode;
    int hp;
    int sp;
    int exp;
    int lv;
    static float statUpgradeFactor = 0.6f;
    UnitPool unitPool;
    public UnitInfo(UnitPool unitPool, int unitCode, int lv)
    {
        this.unitPool = unitPool;
        this.unitCode = unitCode;
        this.lv = lv;
        this.hp = MaxHp; 
        this.sp = MaxSp;
        this.exp = 0;
    }
    public int UnitCode { get => unitCode; set => unitCode = value; }
    public int Hp { get => hp; set => hp = value; }
    public int Sp { get => sp; set => sp = value; }
    public int Exp { get => exp; set => exp = value; }
    public int Lv { get => lv; set => lv = value; }
    public int MaxHp { get => unitPool.pool[unitCode].hp + (int)(unitPool.pool[unitCode].hp * this.lv * statUpgradeFactor); }
    public int MaxSp { get => unitPool.pool[unitCode].sp + (int)(unitPool.pool[unitCode].sp * this.lv * statUpgradeFactor); }
    public int MaxExp { get => unitPool.pool[unitCode].growthRate * this.lv; }
    public int GetAtk(int equipStat = 0) => StatForLevel(unitPool.pool[unitCode].atk, equipStat);
    public int GetDef(int equipStat = 0) => StatForLevel(unitPool.pool[unitCode].def, equipStat);
    public int GetSatk(int equipStat = 0) => StatForLevel(unitPool.pool[unitCode].satk, equipStat);
    public int GetSdef(int equipStat = 0) => StatForLevel(unitPool.pool[unitCode].sdef, equipStat);
    public int StatForLevel(int stat, int equipStat)
    {
        return Mathf.RoundToInt((float)stat + (float)(stat+equipStat)*Lv*statUpgradeFactor);
    }
    public void levelUp()
    {
        lv += 1;
    }
}

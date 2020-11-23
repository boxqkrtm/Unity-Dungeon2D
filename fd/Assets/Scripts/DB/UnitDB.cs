using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

[System.Serializable]
public class DropCell
{
    public Item item;
    public int itemCode;
    [Range(0.0f, 1.0f)]
    public float dropRate;//0~1 rate
}

[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitDB : ScriptableObject
{
    public string uname;
    public int hp;
    public int sp;
    public int atk;
    public int def;
    public int satk;
    public int sdef;
    public int growthRate;
    public SkillType skillType;
    public float SkillCooldown;
    public float skillDuration;
    public int skillStamina;
    public float skillPower;
    public string skillName;
    public AIType aiType;
    [MinMaxSlider(0, 20)]
    public Vector2Int spawnFloor;
    public List<DropCell> dropTable;
    [MinMaxSlider(0, 5000)]
    public Vector2Int dropGold;
    public bool needGoldMultiplierByFloor;
}

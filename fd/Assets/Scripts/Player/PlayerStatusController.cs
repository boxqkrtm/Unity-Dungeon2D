using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusController : MonoBehaviour
{
    public UnitInfo unitInfo;
    GameManager gm;
    Image hpBar;
    Image spBar;
    Image expBar;
    TextMeshProUGUI LvText;
    public int Hp
    {
        get => unitInfo.Hp;
        set
        {
            unitInfo.Hp = value;
            if (unitInfo.Hp > MaxHp) unitInfo.Hp = MaxHp;
            if (unitInfo.Hp < 0) unitInfo.Hp = 0;
            hpBar.fillAmount = (float)unitInfo.Hp / unitInfo.MaxHp;
        }
    }
    public int Sp
    {
        get => unitInfo.Sp;
        set
        {
            unitInfo.Sp = value;
            if (unitInfo.Sp > MaxSp) unitInfo.Sp = MaxSp;
            spBar.fillAmount = (float)unitInfo.Sp / unitInfo.MaxSp;
        }
    }
    public int Exp
    {
        get => unitInfo.Exp;
        set
        {
            unitInfo.Exp = value;
            while (true)
            {
                if (unitInfo.Exp >= unitInfo.MaxExp)
                {
                    //level up
                    unitInfo.Exp -= unitInfo.MaxExp;
                    Lv += 1;
                    gm.NotyPlayerLevelUp();
                }
                else
                {
                    break;
                }
            }
            expBar.fillAmount = (float)unitInfo.Exp / unitInfo.MaxExp;
        }
    }
    public int Lv
    {
        get { return unitInfo.Lv; }
        set
        {
            unitInfo.Lv = value;
            LvText.text = unitInfo.Lv.ToString();
        }
    }
    //Todo 여기에 0대신 장비 능력치를 끼워넣어야 함
    public int Atk { get { return unitInfo.GetAtk(gm.GetPlayerEquipAtk()); } }
    public int Def { get { return unitInfo.GetDef(gm.GetPlayerEquipDef()); } }
    public int Satk { get { return unitInfo.GetSatk(gm.GetPlayerEquipSatk()); } }
    public int Sdef { get { return unitInfo.GetSdef(gm.GetPlayerEquipSdef()); } }
    //max 체력 스태미나 경험치
    public int MaxHp { get { return unitInfo.MaxHp; } }
    public int MaxSp { get { return unitInfo.MaxSp; } }
    public int MaxExp { get { return unitInfo.MaxExp; } }
    Image skillIcon;
    Image dashIcon;
    Image attackIcon;
    float skillCooldown;
    float skillMaxCooldown = 30f;
    float dashCooldown;
    float dashMaxCooldown = 0.6f;
    float attackCooldown;
    float attackMaxCooldown = 0.8f;
    public bool CooldownAttack()
    {
        if (attackCooldown == attackMaxCooldown)
        {
            attackCooldown = 0;
            return true;
        }
        return false;
    }
    public bool CooldownDash()
    {
        if (dashCooldown == dashMaxCooldown)
        {
            dashCooldown = 0;
            return true;
        }
        return false;
    }
    public bool CooldownSkill()
    {
        if (skillCooldown >= skillMaxCooldown)
        {
            skillCooldown = 0;
            return true;
        }
        return false;
    }
    IEnumerator CooldownHeal()
    {
        while (true)
        {
            yield return null;
            //초과시 max로 맞춰주고 아니면 증가시킴
            attackCooldown = attackCooldown >= attackMaxCooldown ? attackMaxCooldown : attackCooldown + Time.deltaTime;
            attackIcon.fillAmount = (float)attackCooldown / attackMaxCooldown;
            dashCooldown = dashCooldown >= dashMaxCooldown ? dashMaxCooldown : dashCooldown + Time.deltaTime;
            dashIcon.fillAmount = (float)dashCooldown / dashMaxCooldown;
            skillCooldown = skillCooldown >= skillMaxCooldown ? skillCooldown : skillMaxCooldown + Time.deltaTime;
            skillIcon.fillAmount = (float)skillCooldown / skillMaxCooldown;
        }
    }
    public void Set(UnitPool unitPool, int unitCode, int lv, Image hpBar, Image spBar, Image expBar, GameManager gm, TextMeshProUGUI LvText, Image skillIcon, Image dashIcon, Image attackIcon)
    {
        unitInfo = new UnitInfo(unitPool, unitCode, lv);
        this.gm = gm;
        this.hpBar = hpBar;
        this.spBar = spBar;
        this.expBar = expBar;
        this.LvText = LvText;

        //cooldown indicator with icon
        this.skillIcon = skillIcon;
        this.dashIcon = dashIcon;
        this.attackIcon = attackIcon;

        this.dashCooldown = dashMaxCooldown;
        this.attackCooldown = attackMaxCooldown;
        this.skillCooldown = skillMaxCooldown;
        //init bar
        Hp = Hp;
        Sp = Sp;
        Exp = Exp;
        Lv = Lv;
        StartCoroutine(CooldownHeal());
    }
}

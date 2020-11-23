using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//몬스터에 공통적으로 필요한 스크립트
//피격, 체력 등
public class AI : MonoBehaviour
{
    public GameManager gm;
    public UnitInfo unitInfo;
    Slider HpBar = null;
    int Hp
    {
        get => unitInfo.Hp; set
        {
            unitInfo.Hp = value;
            if (HpBar == null)
                HpBar = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
            HpBar.value = (float)value / unitInfo.MaxHp;
            if (Hp <= 0)
            {
                //몬스터 킬 처리
                List<DropCell> dropList = gm.unitPool.GetDropTable(unitInfo);
                foreach (DropCell cell in dropList)
                {
                    if (Random.Range(0f, 1f) < cell.dropRate)
                    {
                        Item item = cell.item;
                        gm.SpawnItem(transform.position, item);
                    }
                }
                gm.SpawnItem(transform.position, new Item(20, 1, 1, "?", Random.Range(gm.unitPool.GetDropGoldRange(unitInfo).x, gm.unitPool.GetDropGoldRange(unitInfo).y)));
                gm.PlayerGetExpByKill(unitInfo);
                Destroy(this.gameObject);
            }
        }
    }
    int Sp { get => unitInfo.Sp; set => unitInfo.Sp = value; }

    void OnCollisionEnter2D(Collision2D col)
    {
        AnyCollision(col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        AnyCollision(col.gameObject);
    }

    public virtual void AnyCollision(GameObject go)
    {
        if (go.CompareTag("PlayerAttack"))
        {
            int dmg = go.GetComponent<AttackData>().damage;
            AttackType aty = go.GetComponent<AttackData>().attackType;
            //Debug.Log("PlayerAttack 명중"); 
            if (aty == AttackType.AtkHit || aty == AttackType.AtkBow)
            {
                dmg -= unitInfo.GetDef();
                dmg = dmg > 0 ? dmg : 1;
                Hp -= dmg;
            }
            else
            {
                dmg -= unitInfo.GetSdef();
                dmg = dmg > 0 ? dmg : 1;
                Hp -= dmg;
            }
            //피격 이펙트
            Instantiate(gm.atkHitEffect, go.transform.position, Quaternion.identity);
            Destroy(go, 0.5f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public enum DamageType
{
    Atk, Satk
}
public class GameManager : MonoBehaviour
{
    bool isGameover = false;
    SEManager se;
    GameManager gm;
    MapGenerator mg;
    GameObject player;
    public bool isPlayerInteract;
    public PlayerStatusController psController;
    //GameLog
    public GameLog gameLog;
    //Debug Counter
    int spawnItemCounter = 0;
    int spawnMonsterCounter = 0;
    //item
    public GameObject droppedItem;
    //effects
    public GameObject slashAttack;
    public GameObject arrowAttack;
    public GameObject atkHitEffect;
    public GameObject levelUpEffect;
    public GameObject waterAttackEffect;
    public GameObject fireAttackEffect;
    public GameObject leafAttackEffect;
    //monster
    public GameObject slime;
    public GameObject[] unitPrefabList;
    //mapdata
    public int nowFloor = 1;
    //pool
    public ItemPool itemPool;
    public UnitPool unitPool;
    //delay
    public Image attackIcon;
    public Image dashIcon;
    public Image skillIcon;
    //inventory
    InventoryController invController;
    public Sprite emptyImage;
    public Sprite slotSprite;
    public Sprite slotSelectSprite;
    public GameObject inventoryUI;
    public GameObject invenSlotGrid;
    public TextMeshProUGUI itemInfo;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI stat;
    public GameObject InvenQuickSlot;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI crystalText;
    //statusBar
    public Image hpBar;
    public Image spBar;
    public Image expBar;
    public TextMeshProUGUI LvText;
    public TextMeshProUGUI floortext;
    void Start()
    {
        gm = this;
        se = GameObject.Find("SEManager").GetComponent<SEManager>();
        mg = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
        player = GameObject.FindGameObjectWithTag("Player");
        //player info init
        psController = gameObject.AddComponent<PlayerStatusController>();
        psController.Set(unitPool, 2, nowFloor, hpBar, spBar, expBar, gm, LvText, skillIcon, dashIcon, attackIcon);
        //invController need psController so only use this order
        invController = gameObject.AddComponent<InventoryController>();
        invController.Set(itemPool, new List<Item>(), emptyImage, slotSprite, slotSelectSprite, inventoryUI, invenSlotGrid, itemInfo, itemName, gm, stat, InvenQuickSlot, goldText, crystalText, se);

        Astar astar = GameObject.Find("Astar").GetComponent<Astar>();
        astar.targetTile = GameObject.FindGameObjectWithTag("AstarTarget").GetComponent<Tilemap>();
        PlayerGetItem(new Item(17, 1), false);
        StartCoroutine(AutoRemoveHealthAndStamina());
        mg.gm = this;
        mg.astar = GameObject.Find("Astar").GetComponent<Astar>();
        mg.player = GameObject.FindGameObjectWithTag("Player").transform;
        mg.wallcol = GameObject.Find("Wall").GetComponent<TilemapCollider2D>();
        mg.GenerateFloor();
        floortext.text = nowFloor.ToString() + "F";
    }

    IEnumerator AutoRemoveHealthAndStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (psController.Sp > 0)
            {
                var damageAmount = psController.MaxSp / 30;
                damageAmount = damageAmount <= 0 ? 1 : damageAmount;
                psController.Sp -= damageAmount;
                var spPercent = (float)psController.Sp / psController.MaxSp;
                if (spPercent == 0f)
                {
                    AddGameLog("배가 고파졌다.");
                }
            }
            else
            {
                var damageAmount = psController.MaxHp / 5;
                damageAmount = damageAmount <= 0 ? 1 : damageAmount;
                psController.Hp -= damageAmount;
                PlayerGetDamage(0, AttackType.AtkHit);
                AddGameLog("허기로 인한 데미지를 입었다.");
            }
        }
    }

    //Test Button Script
    public void TestItemSpawn()
    {
        spawnItemCounter++;
        AddGameLog("DEBUG: TestItemSpawn() " + spawnItemCounter.ToString());
        Vector2 playerPos = player.transform.position;
        GameObject dpItem = Instantiate(droppedItem, new Vector3(Mathf.Round(playerPos.x), Mathf.Round(playerPos.y), 0), Quaternion.identity);
        DroppedItem ditem = dpItem.GetComponent<DroppedItem>();
        //ditem.SetItem(new Item(Random.Range(1, 22), 1, 0));
        ditem.SetItem(new Item(21, 1, 0));
    }

    public void TestSpawnMob()
    {
        spawnMonsterCounter++;
        AddGameLog("Debug: TestSpawnMob()" + spawnMonsterCounter.ToString());
        Vector2 playerPos = player.transform.position;
        Instantiate(slime, new Vector3(Mathf.Round(playerPos.x), Mathf.Round(playerPos.y), 0), Quaternion.identity);
    }

    //SpawnItem 
    public void SpawnItem(Vector2 pos, Item item)
    {
        var spawnItem = Instantiate(droppedItem, pos, Quaternion.identity);
        var itemScript = spawnItem.GetComponent<DroppedItem>();
        itemScript.SetItem(item);
    }

    public void UpFloor()
    {
        se.Play(11);
        nowFloor++;
        floortext.text = nowFloor.ToString() + "F";
        mg.GenerateFloor();
        gm.AddGameLog(gm.nowFloor.ToString() + "층으로 올라왔다.");
    }

    //층에 맞는 아이템을알아서 랜덤생성
    public void SpawnItemWithFloor(Vector3 pos)
    {
        var ableItems = new List<Item>();
        for (int i = 0; i < itemPool.pool.Count; i++)
        {
            var item = itemPool.pool[i];
            if (nowFloor >= item.floor.x && item.floor.y >= nowFloor || (nowFloor >= 20 && 19 >= item.floor.x && item.floor.y >= 19))
            {
                if (i == 1)
                    ableItems.Add(new Item(i, 5));
                else ableItems.Add(new Item(i, 1));
            }
        }
        SpawnItem(pos, ableItems[Random.Range(0, ableItems.Count)]);
    }

    public void SpawnEquipItemWithFloor(Vector3 pos)
    {
        var equipItems = new List<Item>();
        for (var i = 0; i < gm.itemPool.pool.Count; i++)
        {
            ItemDB item = itemPool.pool[i];
            //장비이면서 지금 층에 생성 가능한 장비를 배열에 넣어둠
            if (item.itemType == ItemType.Equipment)
            {
                if (nowFloor >= item.floor.x && item.floor.y >= nowFloor || (nowFloor >= 20 && 19 >= item.floor.x && item.floor.y >= 19))
                {
                    equipItems.Add(new Item(i, 1));
                }
            }
        }
        SpawnItem(pos, equipItems[Random.Range(0, equipItems.Count)]);
    }
    //GameLog for user
    public void AddGameLog(string str)
    {
        Debug.Log("AddGameLog : " + str);
        gameLog.Add(str);
    }

    //UI Button Script
    public void InventoryOpen()
    {
        invController.Open();
    }
    public void InventoryClose()
    {
        invController.Close();
    }
    public void InventoryTrashClick()
    {
        invController.TrashSelectedItem();
    }
    public void InventoryUseClick()
    {
        invController.UseSelectedItem();
    }
    public void InventoryEquipClick()
    {
        invController.EquipSelectedItem();
    }

    //Inventory
    public void InventorySwap(int a, int b)
    {
        invController.Swap(a, b);
    }

    public bool PlayerGetItem(Item item, bool alert = true)
    {
        if (alert == true)
            se.Play(2);
        if (item.itemDurability == 0)
        {
            item.ItemDurability = gm.itemPool.GetItemDurability(item);
        }
        return invController.AddItem(item, alert);
    }

    public void CreateItemPlayerPos(Item item)
    {
        Vector3 playerPos = player.transform.position;
        SpawnItem(new Vector3(Mathf.Round(playerPos.x), Mathf.Round(playerPos.y), 0), item);
    }

    public bool PlayerGetGold(Item item)
    {
        se.Play(2);
        invController.AddGold(item);
        return true;
    }

    public int GetPlayerEquipAtk()
    {
        return invController.GetEquipAtk();
    }

    public int GetPlayerEquipDef()
    {
        return 0;
    }

    public int GetPlayerEquipSatk()
    {
        return invController.GetEquipSatk();
    }

    public int GetPlayerEquipSdef()
    {
        return 0;
    }

    public bool PlayerUseKey()
    {
        if (invController.UseKey(nowFloor))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerUseAttack(Vector2 latestAxis)
    {
        if (isGameover == true) return false;
        //Debug.Log("Debug: 공격력 " + psController.Atk.ToString() + " " + psController.Satk);
        if (invController.IsEquipVailed())
        {
            //무기 장착 확인
            //스태마나와 내구도를 체크 한다.
            //쿨타임을 확인한다.
            if (psController.CooldownAttack())
            {
                var equipItem = invController.GetEquipItem();
                var staminaAmount = itemPool.GetItemStamina(equipItem);
                switch (invController.GetEquipAttackType())
                {

                    case AttackType.AtkHit:
                        {
                            se.Play(6);
                            Vector2 pos = player.transform.position;
                            pos += latestAxis;
                            var obj = Instantiate(slashAttack, pos, Quaternion.identity);
                            var atd = obj.GetComponent<AttackData>();
                            atd.attackType = AttackType.AtkHit;
                            if (gm.PlayerUseStamina(staminaAmount)) atd.damage = psController.Atk;
                            else atd.damage = psController.Atk / 2;
                        }
                        break;

                    case AttackType.AtkBow:
                        {
                            se.Play(0);
                            //활의 경우 화살도 체크해야함
                            if (!invController.IsHasArrow())
                            {
                                AddGameLog("화살이 부족합니다.");
                                return false;
                            }
                            invController.RemoveFindItemOnce(1, 1);
                            Vector2 pos = player.transform.position;
                            pos += latestAxis;
                            var obj = Instantiate(arrowAttack, pos, Quaternion.identity);
                            var atd = obj.GetComponent<AttackData>();
                            atd.attackType = AttackType.AtkBow;
                            if (gm.PlayerUseStamina(staminaAmount)) atd.damage = psController.Atk;
                            else atd.damage = psController.Atk / 2;
                            atd.attackRotation = latestAxis;
                        }
                        break;
                    case AttackType.SatkFire:
                        {
                            se.Play(5);
                            Vector2 pos = player.transform.position;
                            pos += latestAxis;
                            var obj = Instantiate(fireAttackEffect, pos, Quaternion.identity);
                            var atd = obj.GetComponent<AttackData>();
                            atd.attackType = AttackType.SatkWater;
                            if (gm.PlayerUseStamina(staminaAmount)) atd.damage = psController.Satk;
                            else atd.damage = psController.Satk / 2;
                            atd.attackRotation = latestAxis;
                        }
                        break;
                    case AttackType.SatkLeaf:
                        {
                            se.Play(5);
                            Vector2 pos = player.transform.position;
                            pos += latestAxis;
                            var obj = Instantiate(leafAttackEffect, pos, Quaternion.identity);
                            var atd = obj.GetComponent<AttackData>();
                            atd.attackType = AttackType.SatkWater;
                            if (gm.PlayerUseStamina(staminaAmount)) atd.damage = psController.Satk;
                            else atd.damage = psController.Satk / 2;
                            atd.attackRotation = latestAxis;
                        }
                        break;
                    case AttackType.SatkWater:
                        {
                            se.Play(5);
                            Vector2 pos = player.transform.position;
                            pos += latestAxis;
                            var obj = Instantiate(waterAttackEffect, pos, Quaternion.identity);
                            var atd = obj.GetComponent<AttackData>();
                            atd.attackType = AttackType.SatkWater;
                            if (gm.PlayerUseStamina(staminaAmount)) atd.damage = psController.Satk;
                            else atd.damage = psController.Satk / 2;
                            atd.attackRotation = latestAxis;
                        }
                        break;
                    default:
                        AddGameLog("DEBUG: PlayerUseAttack 미구현 항목");
                        break;
                }
                //내구도 감소
                equipItem.ItemDurability -= 1;
                if (equipItem.ItemDurability <= 0)
                {
                    //장비 파괴
                    invController.RemoveEquipItem();
                    se.Play(1);
                }
                invController.UpdateUI();
                return true;
            }
        }
        else
        {
            gm.AddGameLog("유효한 장비가 장착되지 않았습니다");
        }
        return false;
    }

    public void PlayerUseDash()
    {
        if (isGameover == true) return;
        //&& gm.PlayerUseStamina(1) == true
        if (psController.CooldownDash() == true && gm.PlayerUseStamina(1) == true)
        {
            se.Play(12);
            player.GetComponent<Player>().isDashReady = true;
        }
        return;
    }

    public void PlayerUseInteract()
    {
        if (isGameover == true) return;
        isPlayerInteract = true;
        StartCoroutine(IsPlayerInteractOff());
    }
    IEnumerator IsPlayerInteractOff()
    {
        yield return new WaitForSeconds(0.2f);
        isPlayerInteract = false;
    }

    public bool PlayerHasStamina(int amount = 1)
    {
        if (psController.Sp >= amount)
        {
            return true;
        }
        return false;
    }
    public bool PlayerUseStamina(int amount = 1)
    {
        if (psController.Sp >= amount)
        {
            psController.Sp -= amount;
            return true;
        }
        return false;
    }


    //Player Take Damage
    public void PlayerGetDamage(int damage, AttackType attackType)
    {
        if (isGameover == true) return;
        se.Play(7);
        //데미지 공식 (적공격-방어력)
        //단 계산 후 데미지가 음수나 0일 경우 1로 취급
        if (damage != 0)
        {

            if (attackType == AttackType.AtkHit || attackType == AttackType.AtkBow)
            {
                int calDmg = damage - psController.Def;
                calDmg = calDmg <= 0 ? 1 : calDmg;
                psController.Hp -= calDmg;
            }
            else
            {
                int calDmg = damage - psController.Sdef;
                calDmg = calDmg <= 0 ? 1 : calDmg;
                psController.Hp -= calDmg;
            }
        }
        if (psController.Hp <= 0)
        {
            //Gameover
            AddGameLog("게임오버...");
            AddGameLog("10초 후 게임이 재시작 됩니다.");
            isGameover = true;
            player.GetComponent<SpriteRenderer>().color = Color.clear;
            player.GetComponent<Player>().enabled = false;
            player.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(Restart());
        }
    }
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("ProtoIngame");
    }

    public void PlayerGetExpByKill(UnitInfo unitInfo)
    {
        se.Play(10);
        var unitName = unitPool.GetUnitName(unitInfo);
        psController.Exp += unitInfo.Lv;
        AddGameLog(unitName + "을 쓰러뜨려 " + unitInfo.Lv + "EXP를 획득했다.");
    }

    public bool PlayerHpHeal(float amount, PotionPowerType type)
    {
        se.Play(9);
        if (psController.Hp == psController.MaxHp) return false;
        if (type == PotionPowerType.Integer)
        {
            psController.Hp += Mathf.RoundToInt(amount);
        }
        else if (type == PotionPowerType.Percentage)
        {
            //퍼센트의 경우 amount 0~100으로 값을 받게됨
            psController.Hp += Mathf.RoundToInt(amount * 0.01f * psController.MaxHp);
        }
        return true;
    }
    public bool PlayerSpHeal(float amount, PotionPowerType type)
    {
        se.Play(9);
        if (psController.Sp == psController.MaxSp) return false;
        if (type == PotionPowerType.Integer)
        {
            psController.Sp += Mathf.RoundToInt(amount);
        }
        else if (type == PotionPowerType.Percentage)
        {
            //퍼센트의 경우 amount 0~100으로 값을 받게됨
            psController.Sp += Mathf.RoundToInt(amount * 0.01f * psController.MaxSp);
            //AddGameLog(amount.ToString() + "회복");
        }
        return true;
    }

    public int PlayerAtk { get { return psController.Atk; } }
    public int PlayerDef { get { return psController.Def; } }
    public int PlayerSatk { get { return psController.Satk; } }
    public int PlayerSdef { get { return psController.Sdef; } }

    //Observe Noty
    public void NotyPlayerLevelUp()
    {
        se.Play(4);
        invController.UpdateUI();
        psController.Hp = psController.MaxHp;
        //psController.Sp = psController.MaxSp;
        Instantiate(levelUpEffect, player.transform.position, Quaternion.identity);
        AddGameLog("레벨 업");
        //Debug.Log("플레이어 레벨업");
        //levelup effect 
    }

    // Update is called once per frame
    void Update()
    {

    }
}

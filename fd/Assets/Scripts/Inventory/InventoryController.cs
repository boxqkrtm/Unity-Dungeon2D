using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    ItemPool itemPool;
    //inventory
    List<Item> myItems;
    int myItemsSelected = 0;
    float lastItemSelectedTime = 0f;
    int myItemsEquipped = -1;
    Sprite emptyImage;
    Sprite slotSprite;
    Sprite slotSelectSprite;
    GameObject inventoryUI;
    GameObject invenSlotGrid;//25
    GameObject invenQuickSlot;
    TextMeshProUGUI itemInfo;
    TextMeshProUGUI itemName;
    TextMeshProUGUI stat;
    TextMeshProUGUI goldText;
    TextMeshProUGUI crystalText;
    GameManager gm;
    SEManager se;
    int inventorySize;
    int playerGold;

    public void Set(ItemPool itemPool,
            List<Item> myItems,
            Sprite emptyImage,
            Sprite slotSprite,
            Sprite slotSelectSprite,
            GameObject inventoryUI,
            GameObject invenSlotGrid,
            TextMeshProUGUI itemInfo,
            TextMeshProUGUI itemName,
            GameManager gm,
            TextMeshProUGUI stat, GameObject invenQuickSlot,
            TextMeshProUGUI goldText, TextMeshProUGUI crystalText, SEManager se)
    {
        this.itemPool = itemPool;
        this.myItems = myItems;
        this.emptyImage = emptyImage;
        this.slotSprite = slotSprite;
        this.slotSelectSprite = slotSelectSprite;
        this.inventoryUI = inventoryUI;
        this.invenSlotGrid = invenSlotGrid;
        this.itemInfo = itemInfo;
        this.itemName = itemName;
        this.gm = gm;
        this.se = se;
        this.stat = stat;
        this.invenQuickSlot = invenQuickSlot;
        this.goldText = goldText;
        this.crystalText = crystalText;
        Init();

    }
    private void Init()
    {
        inventorySize = 25;
        for (int i = 0; i < 25; i++)
        {
            int x = i;
            invenSlotGrid.transform.GetChild(i).GetChild(0).gameObject.GetComponent<InventoryItemDrag>().myIndex = i;
            invenSlotGrid.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        //1초 이내에 같은 아이템을 두 번 클릭했을 때
                        if (myItemsSelected == x && (Time.time - lastItemSelectedTime < 0.3f))
                        {
                            AutoUseSelectedItem();
                        }
                        //Debug.Log("클릭 딜레이" + (Time.time - lastItemSelectedTime).ToString());
                        lastItemSelectedTime = Time.time;
                        myItemsSelected = x;
                        UpdateUI();
                        //print(x.ToString() + "번 째 아이템 선택 테스트");
                    });
            invenSlotGrid.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);//장비 여부 해제 E 아이콘
        }
        for (int i = 0; i < 3; i++)
        {
            int x = i;
            invenQuickSlot.transform.GetChild(i).GetChild(0).gameObject.GetComponent<InventoryItemDrag>().myIndex = i;
            invenQuickSlot.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        //1초 이내에 같은 아이템을 두 번 클릭했을 때
                        if (myItemsSelected == x && (Time.time - lastItemSelectedTime < 0.3f))
                        {
                            AutoUseSelectedItem();
                        }
                        //Debug.Log("클릭 딜레이" + (Time.time - lastItemSelectedTime).ToString());
                        lastItemSelectedTime = Time.time;
                        myItemsSelected = x;
                        UpdateUI();
                        //print(x.ToString() + "번 째 아이템 선택 테스트");
                    });
            invenQuickSlot.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);//장비 여부 해제 E 아이콘
        }
        myItems = new List<Item>();
        this.playerGold = 0;
        UpdateUI();
    }

    //아이템을 하나라도 먹었다면 true 반환
    //먹지 못한 나머지 아이템 개수는 다시 땅에 떨어뜨려둠
    public bool AddItem(Item item, bool alert = true)
    {
        bool isGetItemAtLeast = false;
        //들어가야 할 아이템의 개수 0이 될 때 까지 인벤토리에 나누어 넣음
        int maxNeedAmount = item.itemAmount;
        int needAmount = item.itemAmount;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (needAmount == 0) break;
            //중복 아이템 중에 스택이 가능한 항목 검색
            if (myItems[i].itemCode == item.itemCode)
            {
                if (myItems[i].itemAmount + item.itemAmount <= gm.itemPool.GetItemMaxStack(item))
                {
                    //전부 합쳐짐
                    myItems[i].itemAmount += item.itemAmount;
                    needAmount = 0;
                    isGetItemAtLeast = true;
                }
                else if (myItems[i].itemAmount + item.itemAmount >
                 gm.itemPool.GetItemMaxStack(item)
                 && myItems[i].itemAmount != gm.itemPool.GetItemMaxStack(item))
                //일부 합쳐짐
                {
                    needAmount -= gm.itemPool.GetItemMaxStack(item) - myItems[i].itemAmount;
                    myItems[i].itemAmount = gm.itemPool.GetItemMaxStack(item);
                    isGetItemAtLeast = true;
                }
                else
                {
                    //합침 실패 다음 아이템도 확인
                }
            }
        }

        if (needAmount == 0)
        //모두 완전히 획득 완료
        {
            if (isGetItemAtLeast == false)
            {
                isGetItemAtLeast = true;//0개의 아이템이 들어오는 경우
                Debug.LogError("DEBUG ERROR: 개수 0을 가진 아이템 획득");
            }
        }
        else
        //아직 넣어야 할 아이템이 있음
        {
            //더 이상 합칠 수 없으며 새 칸을 차지함
            if (myItems.Count + 1 <= inventorySize)
            {
                int invEmptyCount = inventorySize - myItems.Count;
                //인벤토리에 꽉 채워 들어가는 세트의 수
                //(ex 화살은 한 칸에 최대 10개까지 들어감, 화살 10개 = 화살 1세트)
                int sets = needAmount / gm.itemPool.GetItemMaxStack(item);
                //세트가 아닌 나머지
                int etc = needAmount - sets * gm.itemPool.GetItemMaxStack(item);
                Debug.Log("세트" + sets.ToString() + "개");
                Debug.Log("나머지" + etc.ToString() + "개");

                //세트와 나머지를 넣을 수 있는 만큼 넣는다.
                for (int i = 0; i < sets; i++)
                {
                    Item someItem = new Item(item.itemCode, 0, item.ItemDurability, item.fakename, item.itemMetadata);
                    if (invEmptyCount == 0) break;
                    someItem.ItemAmount = gm.itemPool.GetItemMaxStack(item);
                    needAmount -= gm.itemPool.GetItemMaxStack(item);
                    myItems.Add(someItem);
                    isGetItemAtLeast = true;
                }
                if (etc != 0 && invEmptyCount >= 1)
                {
                    Item someItem = new Item(item.itemCode, 0, item.ItemDurability, item.fakename, item.itemMetadata);
                    someItem.ItemAmount = etc;
                    needAmount -= etc;
                    myItems.Add(someItem);
                    isGetItemAtLeast = true;
                }
            }
            //넣지 못한 아이템들은 다시 버린다.
            if (needAmount != 0)
            {
                Item someItem = new Item(item.itemCode, 0, item.ItemDurability);
                someItem.ItemAmount = needAmount;
                gm.CreateItemPlayerPos(someItem);
            }
        }
        if (isGetItemAtLeast == false)
        {
            gm.AddGameLog("인벤토리가 가득 찼다.");
        }
        else
        {
            int getAmount = maxNeedAmount - needAmount;
            if (getAmount > 1 && alert == true)
            {
                gm.AddGameLog(item.fakename + " " + getAmount.ToString() + "개" + " 획득");
            }
            else if (alert == true)
            {
                gm.AddGameLog(item.fakename + " 획득");
            }
        }
        UpdateUI();
        return isGetItemAtLeast;
    }

    public void AddGold(Item item)
    {
        playerGold += item.itemMetadata;
        gm.AddGameLog(item.itemMetadata.ToString() + "Gold를 얻었다.");
        UpdateUI();
    }

    public bool AddItems(List<Item> items)
    {
        if (myItems.Count + items.Count > inventorySize) return false;
        for (int i = 0; i < items.Count; i++)
        {
            myItems.Add(new Item(Random.Range(0, 20)));
        }
        UpdateUI();
        return true;
    }

    public void UpdateUI()
    {
        //골드 표기
        goldText.text = playerGold.ToString();
        //인벤토리 구석 스탯 표시
        stat.text = "(S)ATK : (" + gm.PlayerSatk.ToString() + ")" + gm.PlayerAtk.ToString()
         + "\n(S)DEF : (" + gm.PlayerSdef.ToString() + ")" + gm.PlayerDef.ToString();
        //아이템 인벤토리에 표시
        for (int i = 0; i < 25; i++)
        {
            Sprite thumbnail;
            string itemCountTxt;
            if (i >= myItems.Count)
            {
                //빈 이미지 적용
                thumbnail = emptyImage;

                //빈칸 텍스트
                itemCountTxt = "";
                invenSlotGrid.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);//내구도 바
            }
            else
            {
                //아이템이 있음
                int itemcode = myItems[i].ItemCode;
                thumbnail = itemPool.pool[itemcode].itemThumbnail;

                //아이템 개수 텍스트
                if (myItems[i].itemAmount > 1)
                {
                    itemCountTxt = myItems[i].itemAmount.ToString();
                }
                else
                {
                    itemCountTxt = "";
                }

                //아이템 내구도 바
                if (myItems[i].ItemDurability > 0 && gm.itemPool.GetItemType(myItems[i]) == ItemType.Equipment)
                {
                    invenSlotGrid.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);//내구도
                    invenSlotGrid.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().fillAmount =
                        (float)myItems[i].ItemDurability / gm.itemPool.GetItemDurability(myItems[i]);
                }
                else
                {
                    invenSlotGrid.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);//내구도
                }
            }
            invenSlotGrid.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Image>().sprite = thumbnail;
            invenSlotGrid.transform.GetChild(i).transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = itemCountTxt;
            invenSlotGrid.transform.GetChild(i).GetComponent<Image>().sprite = slotSprite;
            invenSlotGrid.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);//장비 여부 E 아이콘
        }
        for (int i = 0; i < 3; i++)
        {
            Sprite thumbnail;
            string itemCountTxt;
            if (i >= myItems.Count)
            {
                //빈 이미지 적용
                thumbnail = emptyImage;

                //빈칸 텍스트
                itemCountTxt = "";
                invenQuickSlot.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);//내구도 바
            }
            else
            {
                //아이템이 있음
                int itemcode = myItems[i].ItemCode;
                thumbnail = itemPool.pool[itemcode].itemThumbnail;

                //아이템 개수 텍스트
                if (myItems[i].itemAmount > 1)
                {
                    itemCountTxt = myItems[i].itemAmount.ToString();
                }
                else
                {
                    itemCountTxt = "";
                }

                //아이템 내구도 바
                if (myItems[i].ItemDurability > 0 && gm.itemPool.GetItemType(myItems[i]) == ItemType.Equipment)
                {
                    invenQuickSlot.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);//내구도
                    invenQuickSlot.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().fillAmount =
                        (float)myItems[i].ItemDurability / gm.itemPool.GetItemDurability(myItems[i]);
                }
                else
                {
                    invenQuickSlot.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);//내구도
                }
            }
            invenQuickSlot.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Image>().sprite = thumbnail;
            invenQuickSlot.transform.GetChild(i).transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = itemCountTxt;
            invenQuickSlot.transform.GetChild(i).GetComponent<Image>().sprite = slotSprite;
            invenQuickSlot.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);//장비 여부 E 아이콘
        }
        //현재 커서가 선택하고 있는 아이템 설명, 장비, 사용, 버리기 버튼
        invenSlotGrid.transform.GetChild(myItemsSelected).GetComponent<Image>().sprite = slotSelectSprite;
        if (myItemsSelected < 3)
            invenQuickSlot.transform.GetChild(myItemsSelected).GetComponent<Image>().sprite = slotSelectSprite;
        //현재 착용하고 있는 장비류 아이템
        if (myItemsEquipped != -1)
        {
            invenSlotGrid.transform.GetChild(myItemsEquipped).GetChild(3).gameObject.SetActive(true);//장비 여부 E 아이콘
            if (myItemsEquipped < 3)
            {
                invenQuickSlot.transform.GetChild(myItemsEquipped).GetChild(3).gameObject.SetActive(true);//장비 여부 E 아이콘
                invenQuickSlot.transform.GetChild(myItemsEquipped).GetChild(3).gameObject.SetActive(true);//장비 여부 E 아이콘
            }
        }
        if (myItems.Count > myItemsSelected)
        {
            ItemDB selectItem = itemPool.pool[myItems[myItemsSelected].ItemCode];
            if (selectItem.relicType == RelicType.Key)
            {
                itemName.text = myItems[myItemsSelected].ItemMetadata.ToString() + "층의 열쇠";
                itemInfo.text = myItems[myItemsSelected].ItemMetadata.ToString() + "층에서 사용 할 수 있다. 가까히 가면 자동으로 열린다.";
            }
            else
            {
                itemName.text = selectItem.iname;
                itemInfo.text = selectItem.description;
            }
        }
        else
        {
            itemInfo.text = "";
        }

    }

    public void Open()
    {
        inventoryUI.SetActive(true);
    }

    public void Close()
    {
        inventoryUI.SetActive(false);
    }

    public void DropSelectedItem()
    {
        gm.CreateItemPlayerPos(myItems[myItemsSelected]);
        RemoveSelectedItem();
    }

    //아이템창 한칸 째로 제거하는 함수 아이템 제거시 장비 칸 흐트러짐 막음
    bool RemoveItem(int index)
    {
        if (myItems.Count > myItemsSelected)
        {
            myItems.RemoveAt(index);
            if (myItemsEquipped > index) myItemsEquipped -= 1;
            if (myItemsEquipped == index) myItemsEquipped = -1;
        }
        return false;
    }
    public void RemoveSelectedItem()
    {
        RemoveItem(myItemsSelected);
        UpdateUI();
    }

    public bool RemoveSelectedItemOnce(int amount)
    {
        return RemoveItemOnce(myItemsSelected, amount);
    }

    //아이템을 수량 단위로 삭제하는 메소드
    public bool RemoveItemOnce(int invIndex, int amount)
    {
        if (myItems.Count > invIndex)
        {
            if (myItems[invIndex].ItemAmount >= amount)
            {
                myItems[invIndex].ItemAmount -= amount;
                if (myItems[invIndex].ItemAmount == 0)
                {
                    RemoveItem(invIndex);
                }
                UpdateUI();
                return true;
            }
        }
        return false;
    }

    public bool RemoveFindItemOnce(int itemCode, int amount)
    {
        //인벤토리 창 기준 오른쪽 아래부터 사용함
        for (int i = myItems.Count - 1; i >= 0; i--)
        {
            if (myItems[i].ItemCode == itemCode)
            {
                //아이템 발견 및 1개 삭제
                RemoveItemOnce(i, amount);
                return true;
            }
        }
        return false;
    }

    public void TrashSelectedItem()
    {
        if (myItems.Count > myItemsSelected)
        {
            DropSelectedItem();
        }
    }

    //더블클릭용으로 사용하거나 장비하는 것을 자동으로 함
    public void AutoUseSelectedItem()
    {
        if (myItems.Count > myItemsSelected)
        {
            int selectedItemCode = myItems[myItemsSelected].ItemCode;
            ItemDB itemInfo = itemPool.pool[selectedItemCode];
            if (itemInfo.itemType == ItemType.Equipment)
            {
                EquipSelectedItem();
            }
            else if (itemInfo.itemType == ItemType.Potion)
            {
                UseSelectedItem();
            }
        }
    }

    public void UseSelectedItem()
    {
        if (myItems.Count > myItemsSelected)
        {
            int selectedItemCode = myItems[myItemsSelected].ItemCode;
            ItemDB itemInfo = itemPool.pool[selectedItemCode];
            //아이템의 분류 확인
            switch (itemInfo.itemType)
            {
                case ItemType.Potion:
                    Debug.Log("Potion류 아이템 사용");
                    if (itemInfo.potionType == PotionType.HealHp)
                    {
                        //게임 매니저에 체력 회복 요청
                        if (gm.PlayerHpHeal(itemInfo.potionPower, itemInfo.potionPowerType))
                        {
                            RemoveSelectedItemOnce(1);
                            gm.AddGameLog("포션을 사용했다.");
                        }
                        else
                        {
                            //포션 복용 실패
                            gm.AddGameLog("더 이상 마실 수 없다.");
                        }
                    }
                    else if (itemInfo.potionType == PotionType.HealStamina)
                    {
                        //게임 매니저에 스테미나 회복 요청
                        if (gm.PlayerSpHeal(itemInfo.potionPower, itemInfo.potionPowerType))
                        {
                            RemoveSelectedItemOnce(1);
                            gm.AddGameLog("음식을 먹었다.");
                        }
                        else
                        {
                            //포션 복용 실패
                            gm.AddGameLog("더 이상 먹을 수 없다.");
                        }
                    }
                    else
                    {
                        gm.AddGameLog("오류 : 미구현 포션");
                    }
                    break;
                case ItemType.Equipment:
                    gm.AddGameLog("이렇게 사용하는 것이 아닌 것 같다.");
                    break;
                case ItemType.Relic:
                    gm.AddGameLog("이렇게 사용하는 것이 아닌 것 같다.");
                    break;
                default:
                    gm.AddGameLog("에러: 알 수 없는 아이템");
                    break;
            }
        }
    }

    ///
    ///a, b 인덱스의 아이템을 서로 스왑한다.
    ///
    public void Swap(int a, int b)
    {
        if (a < myItems.Count)//빈칸 드래그 시도 방지
        {
            if (b >= myItems.Count)//a를 빈칸으로 드래그 한 경우
            {
                //a를 맨 마지막으로 옮김
                for (int i = a; i < myItems.Count - 1; i++)
                {
                    SwapNoUpdate(i, i + 1);
                }
            }
            else
            {
                //일반적인 아이템 스왑
                SwapNoUpdate(a, b);
            }
        }
        UpdateUI();
    }

    private void SwapNoUpdate(int a, int b)
    {
        //for bulk swap
        if (myItemsEquipped == a) myItemsEquipped = b;
        else if (myItemsEquipped == b) myItemsEquipped = a;
        var tmp = myItems[a];
        myItems[a] = myItems[b];
        myItems[b] = tmp;
    }

    public void EquipSelectedItem()
    {
        se.Play(8);
        if (myItems.Count > myItemsSelected)
        {
            if (myItemsEquipped == myItemsSelected) myItemsEquipped = -1;
            else myItemsEquipped = myItemsSelected;
            UpdateUI();
        }
    }

    public int GetEquipAtk()
    {
        if (myItemsEquipped == -1 || gm.itemPool.GetItemType(myItems[myItemsEquipped]) != ItemType.Equipment) return 0;
        var itemInfo = gm.itemPool.pool[myItems[myItemsEquipped].ItemCode];
        if (itemInfo.attackType == AttackType.AtkHit || itemInfo.attackType == AttackType.AtkBow)
            return itemInfo.attackPower;
        return 0;
    }

    public int GetEquipSatk()
    {
        if (myItemsEquipped == -1 || gm.itemPool.GetItemType(myItems[myItemsEquipped]) != ItemType.Equipment) return 0;
        var itemInfo = gm.itemPool.pool[myItems[myItemsEquipped].ItemCode];
        if (!(itemInfo.attackType == AttackType.AtkHit || itemInfo.attackType == AttackType.AtkBow))
            return itemInfo.attackPower;
        return 0;
    }

    public bool UseKey(int floor)
    {
        for (int i = 0; i < myItems.Count; i++)
        {
            var itemInfo = gm.itemPool.pool[myItems[i].itemCode];
            if (myItems[i].ItemMetadata == floor && itemInfo.itemType == ItemType.Relic && itemInfo.relicType == RelicType.Key)
            {
                if (i == myItemsEquipped)
                {
                    myItemsEquipped = -1;
                }
                RemoveItem(i);
                UpdateUI();
                return true;
            }
        }
        return false;
    }

    public bool IsEquipVailed()
    {
        if (GetEquipItem() == null) return false;
        if (gm.itemPool.GetItemType(GetEquipItem()) == ItemType.Equipment && gm.GetPlayerEquipAtk() > 0 || gm.GetPlayerEquipSatk() > 0)
        {
            return true;
        }
        return false;

    }

    public bool IsHasArrow()
    {
        foreach (var item in myItems)
        {
            if (item.itemCode == 1)
            {
                //화살 발견
                return true;
            }
        }
        return false;
    }

    public Item GetEquipItem()
    {
        if (myItemsEquipped == -1)
        {
            return null;
        }
        return myItems[myItemsEquipped];
    }
    public AttackType GetEquipAttackType()
    {
        return gm.itemPool.GetItemAttackType(GetEquipItem());
    }

    public void RemoveEquipItem()
    {
        RemoveItem(myItemsEquipped);
        UpdateUI();
        gm.AddGameLog("장비가 파괴되었다.");
    }

    void Update()
    {
        //qucikslot shortcut
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            myItemsSelected = 0;
            AutoUseSelectedItem();
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            myItemsSelected = 1;
            AutoUseSelectedItem();
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            myItemsSelected = 2;
            AutoUseSelectedItem();
            UpdateUI();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.activeSelf == true)
            {
                inventoryUI.SetActive(false);
            }
            else
            {
                inventoryUI.SetActive(true);
            }
        }
    }
}

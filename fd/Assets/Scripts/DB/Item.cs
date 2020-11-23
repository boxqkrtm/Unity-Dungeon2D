[System.Serializable]
public class Item
{
    public Item(int itemCode = 2, int itemAmount = 1, int itemDurability = 0, string fakename = "?", int itemMetaData = 0)
    {
        this.itemCode = itemCode;
        this.itemMetadata = itemMetaData;
        this.itemDurability = itemDurability;
        this.itemAmount = itemAmount;
        this.fakename = fakename;
        //Debug.Log("created item " + this.itemCode);
    }
    public int itemCode;//ItemPool의 해당 아이템 번호
    public int itemMetadata;//ex N층 열쇄의 N층 데이터
    public int itemDurability;//내구성
    public int itemAmount;//수량
    public string fakename;//이름 값 모르는 아이템에는 랜덤설정 아는 아이템은 실제 아이템 값으로 표기
    public bool isPublicInfo;

    //
    public int ItemCode { get => itemCode; }
    public int ItemMetadata { get => itemMetadata; }
    public int ItemDurability { get => itemDurability; set => itemDurability = value; }
    public int ItemAmount { get => itemAmount; set => itemAmount = value; }
    public string Fakename { get => fakename; set => fakename = value; }
    public bool IsPublicInfo { get => isPublicInfo; set => isPublicInfo = value; }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    // Start is called before the first frame update
    public List<ItemDB> pool;

    public string GetItemName(Item item)
    {
        return pool[item.ItemCode].iname;
    }

    public Sprite GetItemSprite(Item item)
    {

        return pool[item.ItemCode].itemThumbnail;
    }

    public bool GetItemIsPublic(Item item)
    {
        return pool[item.ItemCode].isPublicInfo;
    }

    public int GetItemDurability(Item item)
    {
        return pool[item.ItemCode].durability;
    }

    public ItemType GetItemType(Item item)
    {
        return pool[item.ItemCode].itemType;
    }
    public AttackType GetItemAttackType(Item item)
    {
        return pool[item.ItemCode].attackType;
    }
    public int GetItemMaxStack(Item item)
    {
        return pool[item.ItemCode].maxStack;
    }
    public int GetItemStamina(Item item)
    {
        return pool[item.ItemCode].attackStamina;
    }
}
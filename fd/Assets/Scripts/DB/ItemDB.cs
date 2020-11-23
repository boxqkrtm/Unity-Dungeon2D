using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GD.MinMaxSlider;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemDB : ScriptableObject
{
    public string iname;
    public string description;
    public int price = 1;
    [MinMaxSlider(1, 20)]
    public Vector2Int floor;
    public bool isPublicInfo;
    public ItemType itemType;
    public int maxStack;
    public Sprite itemThumbnail;
    //포션
    public PotionPowerType potionPowerType;
    public PotionType potionType;
    public float potionDelay;
    public float potionPower;
    //장비
    public AttackType attackType;
    public int attackPower;
    public float attackDelay;
    public int attackStamina;
    public int attackRange;
    public int durability;
    //유물
    public RelicType relicType;
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemDB))]
public class ItemDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as ItemDB;
        EditorUtility.SetDirty(myScript);
        myScript.iname = EditorGUILayout.TextField("Name", myScript.iname);
        myScript.description = EditorGUILayout.TextField("Description", myScript.description);
        myScript.price = EditorGUILayout.IntField("Price", myScript.price);
        myScript.floor = EditorGUILayout.Vector2IntField("Floor(X~Y)", myScript.floor);
        myScript.itemType = (ItemType)EditorGUILayout.EnumPopup("ItemType", myScript.itemType);
        myScript.isPublicInfo = EditorGUILayout.Toggle("is Public Info", myScript.isPublicInfo);
        myScript.maxStack = EditorGUILayout.IntSlider("Max Stack", myScript.maxStack, 1, 10);
        myScript.itemThumbnail = (Sprite)EditorGUILayout.ObjectField("Thumbnail", myScript.itemThumbnail, typeof(Sprite), false);
        if (myScript.itemType == ItemType.Potion)
        {
            myScript.potionType = (PotionType)EditorGUILayout.EnumPopup("Potion Type", myScript.potionType);
            myScript.potionPowerType = (PotionPowerType)EditorGUILayout.EnumPopup("Potion Power Type", myScript.potionPowerType);
            myScript.potionPower = EditorGUILayout.Slider("Potion Power", myScript.potionPower, 1, 1000);
            myScript.potionDelay = EditorGUILayout.Slider("Potion Delay", myScript.potionDelay, 0, 10);
        }
        else if (myScript.itemType == ItemType.Equipment)
        {
            myScript.attackType = (AttackType)EditorGUILayout.EnumPopup("Attack Type", myScript.attackType);
            myScript.attackPower = EditorGUILayout.IntSlider("Attack Power", myScript.attackPower, 1, 100);
            myScript.attackRange = EditorGUILayout.IntSlider("Attack Range", myScript.attackRange, 1, 10);
            myScript.attackStamina = EditorGUILayout.IntSlider("Attack Stamina", myScript.attackStamina, 1, 10);
            myScript.attackDelay = EditorGUILayout.Slider("Attack Delay", myScript.attackDelay, 0, 5);
            myScript.durability = EditorGUILayout.IntSlider("Durability", myScript.durability, 20, 200);
        }
        else if (myScript.itemType == ItemType.Relic)
        {
            myScript.relicType = (RelicType)EditorGUILayout.EnumPopup("RelicType", myScript.relicType);
        }
    }
}
#endif
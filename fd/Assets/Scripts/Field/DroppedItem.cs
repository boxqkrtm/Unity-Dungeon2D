using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    // Start is called before the first frame update
    public Item item;
    public bool isRandomName = true;
    GameObject player;
    GameManager gm;
    bool isInRangeForOpen = false;
    void Start()
    {
        SetDataToObject();
        StartCoroutine(PlayerCheck());
    }

    public void SetItem(Item item)
    {
        this.item = item;
        SetDataToObject();
    }

    private void SetDataToObject()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (gm.itemPool.GetItemIsPublic(item) || item.IsPublicInfo == true)
        {
            item.Fakename = gm.itemPool.GetItemName(item);
            gameObject.GetComponent<SpriteRenderer>().sprite = gm.itemPool.GetItemSprite(item);
        }
        else
        {
            item.Fakename = "무언가";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRangeForOpen)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            if (gm.isPlayerInteract == true)
            {
                if (item.ItemCode == 20)
                {
                    //GOld 돈의 아이템 코드 바로 지급됨
                    gm.PlayerGetGold(item);
                    Destroy(gameObject);
                }
                else if (gm.PlayerGetItem(item))
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    IEnumerator PlayerCheck()
    {
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);
        //Debug.Log(distance);
        if (distance < 1f)
        {
            isInRangeForOpen = true;
        }
        else
        {
            isInRangeForOpen = false;
        }
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(PlayerCheck());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    GameManager gm;
    public int myIndex = 0;
    private Canvas canvas;
    GraphicRaycaster graphicRaycaster;
    GameObject dragPreview;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvas = GetComponentInParent<Canvas>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        dragPreview = gm.inventoryUI.transform.Find("InventoryDragPreview").gameObject;
    }
    public void OnBeginDrag(PointerEventData ped)
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        dragPreview.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
        dragPreview.SetActive(true);
    }
    public void OnEndDrag(PointerEventData ped)
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        dragPreview.SetActive(false);

        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(ped, results);
        // Check all hits.
        var targetIndex = 1;
        foreach (var hit in results)
        {
            var target = hit.gameObject.GetComponent<InventoryItemDrag>();
            if (target != null)
            {
                targetIndex = target.myIndex;
                break;
            }
        }
        gm.InventorySwap(myIndex, targetIndex);

        //ic에 자신의 인덱스번호와 도착 인덱스를 보냄
    }
    public void OnDrag(PointerEventData ped)
    {
        dragPreview.transform.position = ped.position;
    }

}

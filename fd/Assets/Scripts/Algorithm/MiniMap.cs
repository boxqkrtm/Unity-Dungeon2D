using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    // Start is called before the first frame update
    Tilemap targetTile;
    public GameObject minimapDot;
    public GameObject mapbg;
    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetTile = GameObject.FindGameObjectWithTag("AstarTarget").GetComponent<Tilemap>();
        InvokeRepeating("LoadTile", 0.0f, 0.1f);
    }

    public void LoadTile()
    {
        Vector2Int playerPos = Vector2Int.RoundToInt((Vector2)player.transform.position);
        var maxi = 30;
        var maxj = 50;
        for (int i = 0; i < maxi; i++)
        {
            for (int j = 0; j < maxj; j++)
            {
                if (i == maxi / 2 && j == maxj / 2)
                {
                    mapbg.transform.GetChild(j + (i * maxj)).gameObject.GetComponent<Image>().color = Color.red;
                    continue;
                }
                bool isWall = !(targetTile.HasTile(new Vector3Int(playerPos.x + j - (maxj / 2), playerPos.y - i + (maxi / 2), 0)));
                if (isWall == false)
                {
                    mapbg.transform.GetChild(j + (i * maxj)).gameObject.GetComponent<Image>().color = Color.white;
                }
                else
                {

                    mapbg.transform.GetChild(j + (i * maxj)).gameObject.GetComponent<Image>().color = Color.clear;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

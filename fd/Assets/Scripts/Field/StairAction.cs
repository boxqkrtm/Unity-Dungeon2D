using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StairAction : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gm;
    GameObject player;
    bool isInRangeForOpen = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(PlayerCheck());
    }

    IEnumerator PlayerCheck()
    {
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);
        if (distance < 0.3f)
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

    // Update is called once per frame
    void Update()
    {
        if (isInRangeForOpen)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            if (gm.isPlayerInteract == true)
            {
                gm.UpFloor();
                Destroy(gameObject);
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

    }
}

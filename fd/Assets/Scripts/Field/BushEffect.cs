using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Tilemaps;

public class BushEffect : MonoBehaviour
{
    GameObject player;
    bool isInRangeForOpen = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRangeForOpen)
        {
            gameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            gameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        isInRangeForOpen = false; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        isInRangeForOpen = true; 
    }
}

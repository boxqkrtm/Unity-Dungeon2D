using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
       gm = GameObject.Find("GameManager").GetComponent<GameManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("onCollisionEnter2D");
        if(col.gameObject.CompareTag("Player"))
        {
            if(gm.PlayerUseKey())
            {
                gm.AddGameLog("열쇠로 문을 열었다.");
                Destroy(gameObject);
            }
            else
            {
                gm.AddGameLog("층에 맞는 열쇠가 없다...");
            }
        }
    }
}

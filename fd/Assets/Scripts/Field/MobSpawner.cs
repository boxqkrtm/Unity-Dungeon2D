using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    GameManager gm;
    GameObject myUnit;
    GameObject player;
    public bool spawnAnyMob = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gm.nowFloor >= 20)
        {
            spawnAnyMob = true;
        }
        InstantSpawn();
        StartCoroutine(AutoSpawn());
    }

    IEnumerator AutoSpawn()
    {
        while (true)
        {
            if (myUnit == null)
            {
                yield return new WaitForSeconds(15f);
                var playerRange = Vector2.Distance(transform.position, player.transform.position);
                //플레이어가 8블록 밖에 있을 때 까지 기다렸다가 리스폰 한다.
                while (playerRange < 8f)
                {
                    playerRange = Vector2.Distance(transform.position, player.transform.position);
                    yield return new WaitForSeconds(0.5f);
                }
                InstantSpawn();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void InstantSpawn()
    {
        List<int> ableUnitCode = new List<int>();
        for (var i = 0; i < gm.unitPool.pool.Count; i++)
        {
            var unit = gm.unitPool.pool[i];
            if (i == 0 || i == 1 || i == 2 || i == 6 || i == 7) continue;
            if (spawnAnyMob || (unit.spawnFloor.x <= gm.nowFloor && unit.spawnFloor.y >= gm.nowFloor))
            {
                ableUnitCode.Add(i);
            }
        }
        var selectedUnitCode = ableUnitCode[Random.Range(0, ableUnitCode.Count)];
        var selectedUnitPrefab = gm.unitPrefabList[selectedUnitCode];
        myUnit = Instantiate(selectedUnitPrefab, transform.position, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MagicalGoblinAIMode
{
    Idle, Dodge, Skill
}
public class MagicalGoblinAI : AI
{
    public MagicalGoblinAIMode aiMode;

    //speed
    float sightRange = 5f;
    float speed = 3f;

    //skill
    float skillDelay = 5f;
    float skillDelayTimer = 0f;
    public GameObject fireAttack;
    SEManager se;
    Coroutine moveRoutine = null;

    // Start is called before the first frame update
    void Start()
    {
        se = GameObject.Find("SEManager").GetComponent<SEManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        unitInfo = new UnitInfo(gm.unitPool, 3, gm.nowFloor);
        aiMode = MagicalGoblinAIMode.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isSeeEnemy = false;
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        float playerDistance = Vector2.Distance(playerPos, gameObject.transform.position);
        if (playerDistance <= sightRange) isSeeEnemy = true;
        skillDelayTimer -= Time.deltaTime;
        if (aiMode == MagicalGoblinAIMode.Idle)
        {
            if (isSeeEnemy == true)
                aiMode = MagicalGoblinAIMode.Dodge;
        }
        else if (aiMode == MagicalGoblinAIMode.Dodge)
        {
            if (skillDelayTimer <= 0)
            {
                //스킬 쿨이 있으면 3칸 안으로 다가와서 때림
                if (moveRoutine == null && playerDistance > 3)
                {
                    Vector2 nextPos = GameObject.Find("Astar").GetComponent<Astar>().PathFinding(gameObject.transform.position, playerPos);
                    moveRoutine = StartCoroutine(MoveTo(nextPos));
                }
                else if (playerDistance <= 3)
                {
                    aiMode = MagicalGoblinAIMode.Skill;
                }
            }
            //스킬 쿨이 없으면 3칸 거리두기 시도
            else if (playerDistance < 3f)
            {
                if (moveRoutine == null)
                {
                    Vector2 nextPos = GameObject.Find("Astar").GetComponent<Astar>().PathFinding(gameObject.transform.position, playerPos);
                    moveRoutine = StartCoroutine(MinusMoveTo(nextPos));
                }
            }
        }
        else if (aiMode == MagicalGoblinAIMode.Skill)
        {
            if (moveRoutine == null)
            {
                if (skillDelayTimer <= 0)
                {
                    skillDelayTimer = skillDelay;
                    moveRoutine = StartCoroutine(TripleFireBall());
                }
            }
        }
    }

    IEnumerator TripleFireBall()
    {
        yield return null;
        for (var i = 0; i < 3; i++)
        {
            se.Play(5);
            var playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            var obj = Instantiate(fireAttack, transform.position, Quaternion.identity);
            var atd = obj.GetComponent<AttackData>();
            atd.attackType = AttackType.SatkFire;
            atd.damage = unitInfo.GetSatk(1); // 스킬 1atk 추가연산
            atd.attackRotation = ((Vector3)playerPos - transform.position).normalized;
            aiMode = MagicalGoblinAIMode.Dodge;
            yield return new WaitForSeconds(0.4f);
        }
        aiMode = MagicalGoblinAIMode.Dodge;
        moveRoutine = null;
    }

    IEnumerator RandomMove()
    {
        float moveTime = 0f;
        Vector2 movVec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        while (moveTime < 1f)
        {
            moveTime += Time.deltaTime;
            transform.Translate(movVec * speed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        aiMode = MagicalGoblinAIMode.Idle;
    }

    IEnumerator MinusMoveTo(Vector2 target)
    {
        float timeout = 2f;
        while (Vector2.Distance(target, transform.position) > 0.1f && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target, -speed * Time.deltaTime);
            yield return null;
        }
        moveRoutine = null;
    }
    IEnumerator MoveTo(Vector2 target)
    {
        float timeout = 2f;
        while (Vector2.Distance(target, transform.position) > 0.1f && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        moveRoutine = null;
    }
}

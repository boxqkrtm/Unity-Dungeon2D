using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkeletonAIMode
{
    Idle, Dodge, Skill
}
public class SkeletonAI : AI
{
    public SkeletonAIMode aiMode;

    //speed
    float sightRange = 5f;
    float speed = 3f;

    //skill
    float skillDelay = 3f;
    float skillDelayTimer = 0f;
    public GameObject boneAttack;

    //timer

    Coroutine moveRoutine = null;
    Animator myAnim;

    SEManager se;
    //플레이어 발견->3개의 불구슬 투사체 공격->5초 회피->1초 공격반사->5초 회피->불구슬 루프
    // Start is called before the first frame update
    void Start()
    {
        se = GameObject.Find("SEManager").GetComponent<SEManager>();
        myAnim = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        unitInfo = new UnitInfo(gm.unitPool, 4, gm.nowFloor);
        aiMode = SkeletonAIMode.Idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isSeeEnemy = false;
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        float playerDistance = Vector2.Distance(playerPos, gameObject.transform.position);
        if (playerDistance <= sightRange) isSeeEnemy = true;
        skillDelayTimer -= Time.deltaTime;
        if (aiMode == SkeletonAIMode.Idle)
        {
            if (isSeeEnemy == true)
                aiMode = SkeletonAIMode.Dodge;
        }
        else if (aiMode == SkeletonAIMode.Dodge)
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
                    aiMode = SkeletonAIMode.Skill;
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
        else if (aiMode == SkeletonAIMode.Skill)
        {
            skillDelayTimer = skillDelay;
            var obj = Instantiate(boneAttack, transform.position, Quaternion.identity);
            var atd = obj.GetComponent<AttackData>();
            atd.attackType = AttackType.AtkBow;
            atd.damage = unitInfo.GetAtk(1); // 스킬 1atk 추가연산
            atd.attackRotation = ((Vector3)playerPos - transform.position).normalized;
            se.Play(5);
            aiMode = SkeletonAIMode.Dodge;
            skillDelayTimer = skillDelay;
        }
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
        aiMode = SkeletonAIMode.Idle;
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

        myAnim.SetBool("isMove", true);
        float timeout = 2f;
        while (Vector2.Distance(target, transform.position) > 0.1f && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        moveRoutine = null;
        myAnim.SetBool("isMove", false);
    }
}

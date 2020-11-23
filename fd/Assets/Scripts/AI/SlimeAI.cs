using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SlimeAIMode
{
    Idle, Chase, Attack
}
public class SlimeAI : AI
{
    public SlimeAIMode aiMode;

    //speed
    float sightRange = 4f;
    float speed = 2f;
    float attackDelay = 0.4f;

    //skill
    float skillDelay = 30f;
    float skillDelayTimer = 0f;

    //timer
    float idleTime;
    float attackTime;

    Coroutine moveRoutine = null;

    /*

     아이들 상태 -5블록 안에 적 발견 시> 추적모드
     아이들 상태 -스킬 쿨타임 0-> 스킬 사용
     아이들 상태 -> 2초뒤 랜덤 이동
     추적모드 -1블록 안에 적 발견 시> 공격모드 (딜레이 2초, SATK 공격, 30% 확률로 산성 상태이상)

     */
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        unitInfo = new UnitInfo(gm.unitPool, 5, gm.nowFloor);
        aiMode = SlimeAIMode.Idle;
        idleTime = 0f;
        attackTime = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isSeeEnemy = false;
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        float playerDistance = Vector2.Distance(playerPos, gameObject.transform.position);
        if (playerDistance <= sightRange) isSeeEnemy = true;
        skillDelayTimer -= Time.deltaTime;
        if (skillDelayTimer <= 0)
        {
            skillDelayTimer = 0;
            //회복 스킬
            if (unitInfo.Hp != unitInfo.MaxHp)
            {
                int healAmount = unitInfo.MaxHp / 10;
                healAmount = healAmount <= 0 ? 1 : healAmount;
                unitInfo.Hp += healAmount;
                skillDelayTimer = skillDelay;
            }
        }
        if (aiMode == SlimeAIMode.Idle)
        {
            if (aiMode == SlimeAIMode.Idle && isSeeEnemy)
            {
                //적 발견 추격모드로 전환
                aiMode = SlimeAIMode.Chase;
            }
            else if (aiMode == SlimeAIMode.Idle && idleTime >= 2f)
            {
                //랜덤이동
                StartCoroutine(RandomMove());
                idleTime = 0;
            }
            else
            {
                //가만히 있음
                idleTime += Time.deltaTime;
            }
        }
        else if (aiMode == SlimeAIMode.Chase)
        {
            if (playerDistance <= 1f)
            {
                //탐색 중 거리 내 발견 공격모드로 전환
                aiMode = SlimeAIMode.Attack;
                attackTime = 0;
            }
            else
            {
                //탐색 진행 안보이면 대기모드로 전환

                if (moveRoutine == null)
                {
                    Vector2 nextPos = GameObject.Find("Astar").GetComponent<Astar>().PathFinding(gameObject.transform.position, playerPos);
                    moveRoutine = StartCoroutine(MoveTo(nextPos));
                }
                if (isSeeEnemy == false) aiMode = SlimeAIMode.Idle;
            }
        }
        else if (aiMode == SlimeAIMode.Attack)
        {
            //공격모드
            if (attackTime >= attackDelay)
            {
                //Debug.Log("Slime의 공격");
                attackTime = 0;
                if (playerDistance < 1f)
                {
                    gm.PlayerGetDamage(unitInfo.GetSatk(), AttackType.SatkWater);
                }
            }
            else if (playerDistance > 1f)
            {
                //1칸에서 더 멀어지면 다시 추격모드로 전환
                aiMode = SlimeAIMode.Chase;
            }
            else
            {
                //공격 준비
                attackTime += Time.deltaTime;
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
            aiMode = SlimeAIMode.Idle;
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
}

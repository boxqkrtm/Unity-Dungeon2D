using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 3f;
    Rigidbody2D myRigidbody;
    GameManager gm;
    Animator myAnim;
    SpriteRenderer mySprite;
    Vector2 latestAxis;
    public bool isDashReady;

    float kbdAttackKeyDownDelay = 0.5f;
    float kbdAttackKeyDownTimer = 0f;

    public TouchPad touchpad;
    // Start is called before the first frame update
    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        myAnim = GetComponent<Animator>();
        isDashReady = false;
        latestAxis = Vector2.zero;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        GameObject.Find("GetItemButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                gm.PlayerUseInteract();
            }
        );
        GameObject.Find("AttackCoolDown").GetComponent<Button>().onClick.AddListener(() =>
        {
            gameObject.GetComponent<Player>().Attack();
        });
        GameObject.Find("DashCoolDown").GetComponent<Button>().onClick.AddListener(() =>
        {
            gameObject.GetComponent<Player>().Dash();
        });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool isMoveAnim = false;
        float hori = 0f;
        float vert = 0f;
        if (touchpad._buttonPressed == true)
        {
            hori = touchpad.intDiff.x;
            vert = touchpad.intDiff.y;
        }
        else
        {
            hori = Input.GetAxisRaw("Horizontal");
            vert = Input.GetAxisRaw("Vertical");
        }
        if (hori != 0 || vert != 0)
        {
            latestAxis.x = hori;
            latestAxis.y = vert;
            isMoveAnim = true;
        }
        Vector3 position = myRigidbody.position;
        position.x += hori * Time.deltaTime * speed;
        position.y += vert * Time.deltaTime * speed;
        myRigidbody.MovePosition(position);
        if (isDashReady == true)
        {
            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            position = myRigidbody.position;
            position.x += hori * 1;
            position.y += vert * 1;
            myRigidbody.MovePosition(position);
            isDashReady = false;
        }
        myAnim.SetBool("isMove", isMoveAnim);
        //좌우 방향 전환
        if (latestAxis.x < 0)
        {
            mySprite.flipX = true;
        }
        else
        {
            mySprite.flipX = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        kbdAttackKeyDownTimer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            if (kbdAttackKeyDownTimer <= 0)
            {
                Attack();
                kbdAttackKeyDownTimer = kbdAttackKeyDownDelay;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Dash();
        }

        if (Input.GetKey(KeyCode.F))
        {
            Interact();
        }

        //get stick pos TODO touchpad
    }

    public void Interact()
    {
        gm.PlayerUseInteract();
    }
    public void Dash()
    {
        gm.PlayerUseDash();
    }

    public void Attack()
    {
        if (gm.PlayerUseAttack(latestAxis))
        {
            myAnim.SetTrigger("Doing");
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        AnyCollision(col.gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        AnyCollision(col.gameObject);
    }

    public virtual void AnyCollision(GameObject go)
    {
        if (go.CompareTag("EnemyAttack"))
        {
            int dmg = go.GetComponent<AttackData>().damage;
            AttackType aty = go.GetComponent<AttackData>().attackType;
            gm.PlayerGetDamage(dmg, aty);
            //피격 이펙트
            Instantiate(gm.atkHitEffect, go.transform.position, Quaternion.identity);
            Destroy(go, 0.5f);
        }
    }
}

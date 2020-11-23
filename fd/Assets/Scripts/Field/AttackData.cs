using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData : MonoBehaviour
{
    public AttackType attackType;
    public int damage;
    public Vector2 attackRotation;
    private float movedRange;
    public bool forceBowStyleAttack = false;

    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {
        if (forceBowStyleAttack == true)
        {
            while (true)
            {
                float arrowSpeed = 6f;
                yield return new WaitForFixedUpdate();
                movedRange += arrowSpeed * Time.deltaTime;
                transform.position += (Vector3)attackRotation * Time.deltaTime * arrowSpeed;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(attackRotation.y, attackRotation.x) * Mathf.Rad2Deg));
                Debug.Log(Mathf.Atan2(attackRotation.y, attackRotation.x) * Mathf.Rad2Deg);
                if (movedRange >= 3)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {

            while (true)
            {
                switch (attackType)
                {
                    case AttackType.AtkHit:
                        {
                            yield return new WaitForSeconds(0.2f);
                            Destroy(gameObject);
                            break;
                        }
                    case AttackType.AtkBow:
                        {
                            float arrowSpeed = 6f;
                            yield return new WaitForFixedUpdate();
                            movedRange += arrowSpeed * Time.deltaTime;
                            transform.position += (Vector3)attackRotation * Time.deltaTime * arrowSpeed;
                            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(attackRotation.y, attackRotation.x) * Mathf.Rad2Deg));
                            //Debug.Log(Mathf.Atan2(attackRotation.y, attackRotation.x) * Mathf.Rad2Deg);
                            if (movedRange >= 3)
                            {
                                Destroy(gameObject);
                            }
                        }
                        break;

                    case AttackType.SatkLeaf:
                    case AttackType.SatkFire:
                    case AttackType.SatkWater:
                        {
                            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(attackRotation.y, attackRotation.x) * Mathf.Rad2Deg - 90f));
                            yield return new WaitForSeconds(1f);
                            Destroy(gameObject);
                        }
                        break;
                    default:
                        Debug.LogError("미구현 공격");
                        break;
                }
            }
        }
    }
}

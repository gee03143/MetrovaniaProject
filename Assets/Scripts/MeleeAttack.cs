using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackLength;
    public int attackDamage;

    public void DoAttack(Vector2 targetPos)
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, targetPos, attackLength);
        Debug.DrawLine(transform.position, (Vector2)transform.position + targetPos * attackLength, Color.red);
        for (int i = 0; i < hit.Length; i++)
        {
            int launchDirection = 1;
            if (transform.position.x > hit[i].collider.transform.position.x)
            {
                launchDirection = -1;
            }
            EnemyBase target = hit[i].collider.GetComponent<EnemyBase>();
            if (target != null)
            {
                Debug.Log(hit[i].collider.name + "에게 " + attackDamage + "만큼의 피해를 주었습니다");
                target.GetDamage(attackDamage, launchDirection);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool hasDirection = false;
    private Vector3 direction;

    public int damage = 5;
    public int moveSpeed = 5;
    public float lifespan = 5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasDirection)
        {
            transform.Translate(direction * Time.deltaTime * moveSpeed);
            Destroy(gameObject, lifespan);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            enemy.GetDamage(damage, (int)direction.x);
        }
        Destroy(gameObject);
    }

    public void Launch(Vector3 targetPos)
    {
        direction = targetPos;
        hasDirection = true;
    }
}
